﻿namespace MessengerAPI.Interfaces
{
    public interface ISignOutService
    {
        Task SignOut(Guid sessionId, Guid userId);
    }
}