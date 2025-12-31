using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class ProjectManagerDeveloper
    {
        public string ProjectManagerId { get; set; }
        public ProjectManager? ProjectManager { get; set; } = null;
        public string DeveloperId { get; set; }
        public Developers? Developer { get; set; } = null;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
