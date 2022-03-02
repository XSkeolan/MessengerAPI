namespace MessengerAPI
{
    public static class ResponseErrors
    {
        public const string PHONENUMBER_INCORRECT = "Phone number has an incorrect format";
        public const string ALREADY_EXISTS = "Some fields already exists";
        public const string INVALID_PASSWORD = "Password length must be from 10 to 32 chars";
        public const string FIELD_LENGTH_IS_LONG = "One or more fields is very long";
        public const string USER_NOT_FOUND = "User not found";
        public const string SESSION_NOT_FOUND = "Session not found";
        public const string INVALID_FIELDS = "Some fields is invalid";
        public const string INVALID_INVITE_USER = "One of users can not be invite";
        public const string EMPTY_MESSAGE = "Message is empty";
        public const string MESSAGE_NOT_FOUND = "Message not found";
    }
}
