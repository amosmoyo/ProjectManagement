using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface IProjectManagerServices
    {
        Task<IEnumerable<ProjectManagerDTO>> GetAllAsync();
        Task<ProjectManagerDTO?> GetByIdAsync(int id);
        Task<ProjectManagerDTO?> GetByUserIdAsync(string userId);
        Task<bool> AssignDeveloperAsync(AssignDeveloperDTO assignDto);
        Task<bool> RemoveDeveloperAsync(int projectManagerId, int developerId);
        Task<IEnumerable<DeveloperDTO>> GetDevelopersAsync(int projectManagerId);
    }
}
