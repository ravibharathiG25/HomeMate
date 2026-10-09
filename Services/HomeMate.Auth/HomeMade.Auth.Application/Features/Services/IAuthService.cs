using HomeMate.Application.Features.Authentication.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace HomeMate.Application.Features.Services
{
    public interface IAuthService
    {
        Task RegisterAsyc(RegisterRequest request);
    }
}
