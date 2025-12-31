using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs
{
    public class DeveloperDTO
    {
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string SkillLevel { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        public int YearsOfExpirience { get; set; }
        public string Department { get; set; } = string.Empty;
    }
}
