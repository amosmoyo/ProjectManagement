using Application.DTOs;
using Application.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class DeveloperServices : IDeveloperServices
    {
        private readonly ILogger<DeveloperServices> _logger;
        private readonly ApplicationDbContext _context;
        public DeveloperServices(ILogger<DeveloperServices> logger, ApplicationDbContext dbContext)
        {
            this._logger = logger;
            this._context = dbContext;
        }

        public async Task<IEnumerable<DeveloperDTO?>> GetAllAsync()
        {
            _logger.LogInformation("Getting all developers");

            try
            {
                var developers = await this._context.Developers
                    .Include(dev => dev.User)
                    .Include(dev => dev.ProjectManagerDevelopers)
                    .ThenInclude(pm => pm.ProjectManager)
                    .ThenInclude(u => u.User).ToListAsync();

                return developers.Select(dev => new DeveloperDTO
                {
                    Id = dev.Id,
                    Email = dev.User.Email,
                    FirstName = dev.User.FirstName,
                    LastName = dev.User.LastName,
                    SkillLevel = dev.SkillLevel,
                    Specialization = dev.Specialization,
                    YearsOfExpirience = dev.YearsOfExpirience,
                    Department = dev.Department,
                    ProjectManagers = dev.ProjectManagerDevelopers?.Select(pm => new ProjectManagerDTO
                    {
                        Email = pm.ProjectManager.User.Email,
                        FirstName = pm.ProjectManager.User.FirstName,
                        LastName = pm.ProjectManager.User.LastName,
                        Department = pm.ProjectManager.Department
                    }).ToList()
                });
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "An error occurred while getting all developers");
                return null;
            }
        }

        public async Task<DeveloperDTO?> GetByIdAsync(string id)
        {
            try
            {
                var developer = await this._context.Developers
                    .Include(dev => dev.User)
                    .Include(dev => dev.ProjectManagerDevelopers)
                    .ThenInclude(pm => pm.ProjectManager)
                    .ThenInclude(u => u.User)
                    .FirstOrDefaultAsync(dev => dev.Id == id);


                return developer == null? null: new DeveloperDTO
                {
                    Id = developer.Id,
                    Email = developer.User.Email,
                    FirstName = developer.User.FirstName,
                    LastName = developer.User.LastName,
                    SkillLevel = developer.SkillLevel,
                    Specialization = developer.Specialization,
                    YearsOfExpirience = developer.YearsOfExpirience,
                    Department = developer.Department,
                    ProjectManagers = developer.ProjectManagerDevelopers?.Select(pm => new ProjectManagerDTO
                    {
                        Email = pm.ProjectManager.User.Email,
                        FirstName = pm.ProjectManager.User.FirstName,
                        LastName = pm.ProjectManager.User.LastName,
                        Department = pm.ProjectManager.Department
                    }).ToList()
                };
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"An error occurred while getting developer with id {id}");
                return null;
            }
        }

        public async Task<IEnumerable<ProjectManagerDTO?>> GetProjectManagersAsync(string developerId)
        {
            try
            {
                var projectManagers = await this._context.ProjectManagerDevelopers
                    .Where(pmd => pmd.DeveloperId == developerId)
                    .Include(pmd => pmd.ProjectManager)
                    .ThenInclude(pm => pm.User)
                    .ToListAsync();

                return projectManagers.Select(pm => new ProjectManagerDTO
                {
                    Id = pm.ProjectManager.Id,
                    Email = pm.ProjectManager.User.Email,
                    FirstName = pm.ProjectManager.User.FirstName,
                    LastName = pm.ProjectManager.User.LastName,
                    SkillLevel = pm.ProjectManager.SkillLevel,
                    YearsOfExpirience = pm.ProjectManager.YearsOfExpirience,
                    Department = pm.ProjectManager.Department
                });
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"An error occurred while getting project managers for developer with id {developerId}");
                return null;
            }
        }
    }
}
