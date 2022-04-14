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

        public MessageController(IMessageService messageService, IChatService chatService)
        {
            _messageService = messageService;
            _chatService = chatService;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> SendMessage(MessageRequest request)
        {
            // добавить проверку на совместную пустоту(см сервис)
            if (string.IsNullOrWhiteSpace(request.Message))
            {
                return BadRequest(ResponseErrors.EMPTY_MESSAGE);
            }
            if (request.Attachment != null)
            {
                if(request.Attachment.Count()>5)
                {
                    return BadRequest(ResponseErrors.COUNT_FILES_VERY_LONG);
                }
                if (request.Attachment.All(file => file.Length == 0))
                {
                    return BadRequest(ResponseErrors.FILE_IS_EMPTY);
                }
            }

            try
            {
                Message message = new Message
                {
                    Destination = request.Destination,
                    Text = request.Message
                };

                await _messageService.SendMessageAsync(message);
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
        public async Task<IActionResult> GetHistory(Guid chatId, DateTime? dateStart, DateTime? dateEnd)
        {
            if(!(dateStart.HasValue && dateEnd.HasValue))
            {
                return BadRequest(ResponseErrors.INVALID_FIELDS);
            }
            if(dateStart>=dateEnd)
            {
                return BadRequest(ResponseErrors.INVALID_FIELDS);
            }

            IEnumerable<Message> messages = await _messageService.GetHistoryAsync(chatId, dateStart, dateEnd);
            return Ok(messages.Select(message => new MessageResponse 
            {
                MessageId = message.Id,
                Message = message.Text,
                Date = message.DateSend,
                FromId = message.From,
                IsPinned = message.IsPinned,
            }));
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

            return Ok(await _chatService.GetDialogsAsync(offsetId, count));
        }

        [HttpPost]
        [Authorize]
        [Route("forwardMessage")]
        public async Task<IActionResult> ForwardMessage(ForwardMessageRequest request)
        {
            if(!await _messageService.MessageIsAvaliableAsync(request.MessageId))
            {
                return BadRequest(ResponseErrors.MESSAGE_NOT_FOUND);
            }

            //федеральное правило
            if(!await _chatService.ChatIsAvaliableAsync(request.ChatId))
            {
                return BadRequest(ResponseErrors.USER_NOT_PARTICIPANT);
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
            if(!await _messageService.MessageIsAvaliableAsync(request.ReplyMessageId))
            {
                return BadRequest(ResponseErrors.MESSAGE_NOT_FOUND);
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
            if(!await _messageService.MessageIsAvaliableAsync(messageId))
            {
                return BadRequest(ResponseErrors.MESSAGE_NOT_FOUND);
            }

            return Ok(await _messageService.GetMessageAsync(messageId));
        }
    }
}
