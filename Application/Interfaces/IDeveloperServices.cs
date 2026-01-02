using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface IDeveloperServices
    {
        Task<IEnumerable<DeveloperDTO?>> GetAllAsync();
        Task<DeveloperDTO?> GetByIdAsync(string id);

        //Task<DeveloperDTO?> GetByUserIdAsync(string userId);
        Task<IEnumerable<ProjectManagerDTO?>> GetProjectManagersAsync(string developerId);
    }
}
