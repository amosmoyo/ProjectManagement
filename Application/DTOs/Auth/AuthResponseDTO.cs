using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.Auth
{
    public class AuthResponseDTO
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? Token { get; set; }
        public string? UserId { get; set; }
        public string? Email { get; set; }
        public string? UserType { get; set; }
        public IList<string>? Roles { get; set; }
    }
}
