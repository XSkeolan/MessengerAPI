﻿using System.Runtime.Serialization;

namespace MessengerAPI
{
    public static class ResponseErrors
    {
        #region Users
        public const string USER_NOT_FOUND = "User not found";
        public const string USER_HAS_NOT_ACCESS = "You have not access to this operation";
        public const string USER_ALREADY_HAS_CODE = "You already have a code that is not used. if you want to poison the code again, use the method resendCode";
        public const string USER_HAS_UNCONFIRMED_EMAIL = "You have an unconfirmed email. Confirm your email first for sending code";
        public const string USER_ALREADY_AUTHORIZE = "You already authorize in this device. Use signOut method and after that use this method again";
        public const string USER_NOT_PARTICIPANT = "This user is not a member of this chat";
        public const string USER_EMAIL_NOT_SET = "You must first add an email to your profile, then confirm it";
        public const string USER_HAS_NOT_CODE = "You have not confirmation code";
        public const string PASSWORD_ALREADY_SET = "This password already set";
        public const string EMAIL_ALREADY_EXIST = "This email already exist";
        #endregion

        #region Chats
        public const string CHAT_NOT_FOUND = "Chat not found";
        public const string CHAT_ADMIN_REQUIRED = "You must be an admin in this chat to do this.";
        public const string CHAT_ADMIN_OR_MODER_REQUIRED = "You must be an admin or moderator in this chat to do this.";
        public const string CHAT_ADMIN_NOT_DELETED = "You must be an creator of this chat for kick admin";
        public const string CHAT_MODER_NOT_DELETED = "You must be an administrator of this chat for kick moderator";
        public const string CHAT_PRIVATE = "This chat is private";
        public const string CHAT_ROLE_NOT_FOUND = "Chat role not found";
        public const string USER_ALREADY_IN_CHAT = "One or more users from the invited list are already in the chat";
        public const string USER_LIST_CHATS_IS_EMPTY = "User has zero chat";
        #endregion

        #region Messages
        public const string MESSAGE_NOT_FOUND = "Message not found";
        public const string EMPTY_MESSAGE = "Message is empty";
        public const string MESSAGE_ALREADY_SENT = "Message already sent in destination";
        public const string DESTINATION_NOT_FOUND = "Destination of message not found";
        public const string USER_NOT_SENDER = "You is not sender of this message";
        #endregion

        #region Files
        public const string FILE_IS_EMPTY = "Uploaded file is empty!";
        public const string FILE_NOT_FOUND = "File not found";
        public const string COUNT_FILES_VERY_LONG = "Count files in this request is very long! The maximum number of files in request is five";
        #endregion

        #region Invalid Field
        public const string PHONENUMBER_INCORRECT = "Phonenumber has an incorrect format";
        public const string ALREADY_EXISTS = "Some fields already exists";
        public const string INVALID_PASSWORD = "Password length must be from 10 to 32 chars";
        public const string INVALID_CODE = "Code length must be 6 chars";
        public const string FIELD_LENGTH_IS_LONG = "One or more fields is very long";
        public const string INVALID_FIELDS = "Some fields is invalid";
        public const string PHONENUMBER_PASSWORD_INCORRECT = "Phonenumber or password are incorrect";
        #endregion

        public const string INVALID_INVITE_USER = "One of users can not be invite";
        public const string INVITE_USERS_LIST_EMPTY = "Invite users list is empty";
        public const string SESSION_NOT_FOUND = "Session not found";
        
        public const string TOKEN_EXPIRED = "The token has expired. Update Token";
        public const string UNUSED_CODE_NOT_EXIST = "This unused code does not exist. If you want send new code, use the method sendCode";
        public const string PERMISSION_DENIED = "Your rights do not allow you to perform this operation";

        public const string UNAUTHORIZE = "Unauthorize! To access this resource, sign in to the system(use signIn method)";
        public const string INVALID_ROLE_FOR_OPENATION = "You have invalid role for this operation under this user";
    }
}
