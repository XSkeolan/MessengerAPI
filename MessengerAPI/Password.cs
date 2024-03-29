﻿using System.Security.Cryptography;

namespace MessengerAPI
{
    public static class Password
    {
        public static string GetHasedPassword(string password)
        {
            byte[] salt = new byte[16];
            var rng = RandomNumberGenerator.Create();
            rng.GetBytes(salt);

            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);

            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);

            return Convert.ToBase64String(hashBytes);
        }

        public static void VerifyHashedPassword(string hashedPassword, string password)
        {
            /* Fetch the stored value */
            string savedPasswordHash = hashedPassword;
            /* Extract the bytes */
            byte[] hashBytes = Convert.FromBase64String(savedPasswordHash);
            /* Get the salt */
            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);
            /* Compute the hash on the password the user entered */
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);
            /* Compare the results */
            for (int i = 0; i < 20; i++)
                if (hashBytes[i + 16] != hash[i])
                    throw new UnauthorizedAccessException(ResponseErrors.PHONENUMBER_PASSWORD_INCORRECT);
        }
    }
}
