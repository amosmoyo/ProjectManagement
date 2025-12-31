using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface IDeveloperServices
    {
        Task<IEnumerable<DeveloperDTO>> GetAllAsync();
        Task<DeveloperDTO?> GetByIdAsync(int id);
        Task<DeveloperDTO?> GetByUserIdAsync(string userId);
        Task<IEnumerable<ProjectManagerDTO>> GetProjectManagersAsync(int developerId);
    }
}
