﻿using MessengerAPI.DTOs;

namespace MessengerAPI.Interfaces
{
    public interface ISendCodeService
    {
        Task<SendCodeResponse> SendCodeAsync(Guid userId);
    }
}