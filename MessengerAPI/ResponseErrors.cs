namespace MessengerAPI
{
    public static class ResponseErrors
    {
        #region User
        public const string USER_NOT_FOUND = "User not found";
        public const string USER_HAS_NOT_ACCESS = "You have not access to message";
        public const string USER_ALREADY_HAS_CODE = "You already have a code that is not used. if you want to poison the code again, use the method resendCode";
        public const string USER_HAS_UNCONFIRMED_EMAIL = "You have an unconfirmed email. Confirm your email first for sending code";
        public const string USER_ALREADY_AUTHORIZE = "You already authorize in this device. Use signOut method and after that use this method again";
        #endregion
        public const string PHONENUMBER_INCORRECT = "Phone number has an incorrect format";
        public const string ALREADY_EXISTS = "Some fields already exists";
        public const string INVALID_PASSWORD = "Password length must be from 10 to 32 chars";
        public const string FIELD_LENGTH_IS_LONG = "One or more fields is very long";
        
        public const string SESSION_NOT_FOUND = "Session not found";
        public const string INVALID_FIELDS = "Some fields is invalid";
        public const string INVALID_INVITE_USER = "One of users can not be invite";
        public const string EMPTY_MESSAGE = "Message is empty";
        public const string MESSAGE_NOT_FOUND = "Message not found";
        public const string DESTINATION_NOT_FOUND = "Destination of message not found";
        public const string TOKEN_EXPIRED = "The token has expired.Update Token";
        public const string UNUSED_CODE_NOT_EXIST = "This unused code does not exist. If you want send new code, use the method sendCode";
        
        public const string UNAUTHORIZE = "Unauthorize! To access this resource, sign in to the system(use signIn method)";
    }
}
