using MessengerAPI.DTOs;
using MessengerAPI.Interfaces;
using MessengerAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MessengerAPI.Controllers
{
    [Route("api/private/messages")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _messageService;
        private readonly IChatService _chatService;

        public MessageController(IMessageService messageService, IChatService chatService, IFileService fileService)
        {
            _messageService = messageService;
            _chatService = chatService;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> SendMessage([FromForm] MessageRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Message))
            {
                return BadRequest(ResponseErrors.EMPTY_MESSAGE);
            }
            if (request.Files.Count > 0)
            {
                if (request.Files.Count > 5)
                {
                    return BadRequest(ResponseErrors.COUNT_FILES_VERY_LONG);
                }

                foreach (var file in request.Files)
                {
                    if (file.Length > 5 * 1024 * 1024)
                    {
                        return BadRequest(ResponseErrors.FILE_MAX_SIZE);
                    }
                }
            }

            try
            {
                Message message = new Message
                {
                    Destination = request.Destination,
                    Text = request.Message,
                };

                await _messageService.SendMessageAsync(message);

                foreach (var file in request.Files)
                {
                    if (file.Length > 0)
                    {
                        await _messageService.SendAttachment(message.Id, file);
                    }
                }

                return Created($"api/private/messages?id={message.Id}", null);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Authorize]
        [Route("getHistory")]
        public async Task<IActionResult> GetHistory(Guid chatId, DateTime dateStart, DateTime dateEnd)
        {
            if (dateStart >= dateEnd)
            {
                return BadRequest(ResponseErrors.INVALID_FIELDS);
            }

            IEnumerable<Message> messages = await _messageService.GetHistoryAsync(chatId, dateStart, dateEnd);
            List<MessageResponse> responses = new List<MessageResponse>();
            foreach (Message message in messages)
            {
                responses.Add(new MessageResponse
                {
                    MessageId = message.Id,
                    Message = message.Text,
                    Date = message.DateSend,
                    FromId = message.FromWhom,
                    IsPinned = message.IsPinned,
                    Attachment = await _messageService.GetMessageAttachments(message.Id)
                });
            }
            return Ok(responses);
        }

        [HttpGet]
        [Authorize]
        [Route("getDialogs")]
        public async Task<IActionResult> GetDialogs(Guid? offsetId, int count)
        {
            if (count <= 0)
            {
                return BadRequest(ResponseErrors.INVALID_FIELDS);
            }

            IEnumerable<Chat> dialogs;
            try
            {
                dialogs = await _chatService.GetDialogsAsync(offsetId, count);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }

            IEnumerable<Message?> lastMessages = await Task.WhenAll(dialogs.Select(async (dialog) =>
            {
                return await _messageService.GetLastMessageAsync(dialog.Id);
            }));

            List<DialogInfoResponse> responses = new List<DialogInfoResponse>();
            using (var dialogEnumearator = dialogs.GetEnumerator())
            using (var messageEnmerator = lastMessages.GetEnumerator())
            {
                while (dialogEnumearator.MoveNext() && messageEnmerator.MoveNext())
                {
                    if (messageEnmerator.Current != null)
                    {
                        responses.Add(new DialogInfoResponse
                        {
                            Id = dialogEnumearator.Current.Id,
                            Name = dialogEnumearator.Current.Name,
                            Photo = dialogEnumearator.Current.PhotoId,
                            LastMessageText = messageEnmerator.Current.Text,
                            LastMessageDateSend = messageEnmerator.Current.DateSend
                        });
                    }
                    else
                    {
                        responses.Add(new DialogInfoResponse
                        {
                            Id = dialogEnumearator.Current.Id,
                            Name = dialogEnumearator.Current.Name,
                            Photo = dialogEnumearator.Current.PhotoId,
                            LastMessageText = "",
                            LastMessageDateSend = null
                        });
                    }
                }
            }

            return Ok(responses);
        }

        [HttpPost]
        [Authorize]
        [Route("forwardMessage")]
        public async Task<IActionResult> ForwardMessage(ForwardMessageRequest request)
        {
            try
            {
                await _chatService.GetChatAsync(request.ChatId);
                await _messageService.GetMessageAsync(request.MessageId);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }

            Message message = new Message
            {
                Destination = request.ChatId,
                OriginalMessageId = request.MessageId,
                Text = request.Message
            };

            try
            {
                await _messageService.SendMessageAsync(message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Created($"api/private/messages?id={message.Id}", null);
        }

        [HttpPost]
        [Authorize]
        [Route("replyOnMessage")]
        public async Task<IActionResult> ReplyOnMessage(ReplyMessageRequest request)
        {
            try
            {
                await _messageService.GetMessageAsync(request.ReplyMessageId);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }

            Message message = new Message
            {
                ReplyMessageId = request.ReplyMessageId,
                Text = request.ReplyMessage
            };

            try
            {
                await _messageService.SendMessageAsync(message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Created($"api/private/messages?id={message.Id}", null);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetMessage(Guid messageId)
        {
            try
            {
                Message message = await _messageService.GetMessageAsync(messageId);
                IEnumerable<byte[]> attachments = await _messageService.GetMessageAttachments(messageId);
                return Ok(new MessageResponse
                {
                    FromId = message.FromWhom,
                    MessageId = message.Id,
                    Date = message.DateSend,
                    IsPinned = message.IsPinned,
                    Message = message.Text,
                    Attachment = attachments
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch]
        [Authorize]
        public async Task<IActionResult> EditMessage(EditMessageRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.ModifiedText))
            {
                return BadRequest(ResponseErrors.EMPTY_MESSAGE);
            }

            try
            {
                await _messageService.EditMessageAsync(request.EditableMessageId, request.ModifiedText);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok();
        }

        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> DeleteMessage(Guid messageId)
        {
            try
            {
                await _messageService.GetMessageAsync(messageId);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }

            try
            {
                await _messageService.DeleteMessageAsync(messageId);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok();
        }
    }
}