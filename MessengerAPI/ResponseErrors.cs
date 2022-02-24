namespace MessengerAPI
{
    public static class ResponseErrors
    {
        public const string PHONENUMBER_INCORRECT = "Phone number has an incorrect format";
        public const string ALREADY_EXISTS = "Some fields already exists";
        public const string INVALID_PASSWORD = "Password length must be from 10 to 32 chars";
        public const string FIELD_LENGTH_IS_LONG = "One or more fields is very long";
        public const string USER_NOT_FOUND = "User not found";
    }
}
