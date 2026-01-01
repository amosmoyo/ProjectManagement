using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface IProjectManagerServices
    {
        Task<IEnumerable<ProjectManagerDTO?>> GetAllAsync();
        Task<ProjectManagerDTO?> GetByIdAsync(string id);
        //Task<ProjectManagerDTO?> GetByUserIdAsync(string userId);
        Task<bool> AssignDeveloperAsync(AssignDeveloperDTO assignDto);
        Task<bool> RemoveDeveloperAsync(AssignDeveloperDTO assignDto);
        Task<IEnumerable<DeveloperDTO?>> GetDevelopersAsync(string projectManagerId);
    }
}
