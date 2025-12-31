using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class ProjectManager
    {
        public string Id { get; set; }

        public string UserId { get; set; } = string.Empty;

        public string SkillLevel { get; set; } = string.Empty;

        public int YearsOfExpirience { get; set; }

        public string Department { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; }


        //Navihgation Properties

        public ApplicationUser User { get; set; } = null!;

        public ICollection<ProjectManagerDeveloper> ProjectManagerDevelopers { get; set; } = new List<ProjectManagerDeveloper>();
    }
}
