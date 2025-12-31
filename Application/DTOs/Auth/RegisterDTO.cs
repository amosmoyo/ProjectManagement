using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.Auth
{
    public class RegisterDTO
    {
        public string Email { get; set; }

        public string Password { get; set; }

        public string? FirstName { get; set; } = string.Empty;

        public string? LastName { get; set; }

        public string UserType { get; set; } = string.Empty;

        public string Department { get; set; } = string.Empty;

        public string SkillLevel { get; set; } = string.Empty;

        public int? YearNumberOfExpirience { get; set; }


        // When Registering Developer
        public string? Specialization { get; set; } = string.Empty;


    }
}
