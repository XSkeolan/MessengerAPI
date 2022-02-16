namespace MessengerAPI
{
    public static class ResponseErrors
    {
        public const string PHONENUMBER_INCORRECT = "Phone number has an incorrect format";
        public const string PHONENUMBER_ALREADY_EXISTS = "User with this nickname already exists";
        public const string NICKNAME_ALREADY_EXISTS = "User with this phonenumber already exists";
        public const string INVALID_PASSWORD = "Password length must be from 10 to 32 chars";
    }
}
