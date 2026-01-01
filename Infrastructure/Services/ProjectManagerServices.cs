using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
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
    public class ProjectManagerServices : IProjectManagerServices
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ProjectManagerServices> _logger;
        public ProjectManagerServices(ApplicationDbContext dbContext, ILogger<ProjectManagerServices> logger)
        {
            this._context = dbContext;
            this._logger = logger;
        }

        public async Task<IEnumerable<ProjectManagerDTO?>> GetAllAsync()
        {
            
            try
            {
                var projectManagers = await _context.ProjectManagers
                    .Include(pm => pm.User)
                    .Include(pm => pm.ProjectManagerDevelopers)
                    .ThenInclude(pmd => pmd.Developer)
                    .ThenInclude(d => d.User)
                    .ToListAsync();


                return projectManagers.Select(pm => new ProjectManagerDTO
                {
                    Id = pm.UserId,
                    Email = pm.User.Email!,
                    FirstName = pm.User.FirstName,
                    LastName = pm.User.LastName,
                    SkillLevel = pm.SkillLevel,
                    YearsOfExpirience = pm.YearsOfExpirience,
                    Department = pm.Department,
                    Developers = pm.ProjectManagerDevelopers?
                        .Where(pmd => pmd.IsActive)
                        .Select(pmd => new DeveloperDTO
                        {
                            Email = pmd.Developer.User.Email!,
                            FirstName = pmd.Developer.User.FirstName,
                            LastName = pmd.Developer.User.LastName,
                            SkillLevel = pmd.Developer.SkillLevel,
                            Specialization = pmd.Developer.Specialization,
                            YearsOfExpirience = pmd.Developer.YearsOfExpirience,
                            Department = pmd.Developer.Department
                        }).ToList()
                });
            }
            catch (Exception ex) 
            { 
                this._logger.LogError(ex, "Error retrieving all the data");
                return null;
            }

        }

        public async Task<ProjectManagerDTO?> GetByIdAsync(string id)
        {
            try
            {
                var projectManager = await _context.ProjectManagers
                    .Include(pm => pm.User)
                    .Include(pm => pm.ProjectManagerDevelopers)
                    .ThenInclude(pmd => pmd.Developer)
                    .ThenInclude(d => d.User)
                    .FirstOrDefaultAsync(pm => pm.UserId == id);

                return projectManager == null ? null : new ProjectManagerDTO
                {
                    Id = projectManager.UserId,
                    Email = projectManager.User.Email!,
                    FirstName = projectManager.User.FirstName,
                    LastName = projectManager.User.LastName,
                    SkillLevel = projectManager.SkillLevel,
                    YearsOfExpirience = projectManager.YearsOfExpirience,
                    Department = projectManager.Department,
                    Developers = projectManager.ProjectManagerDevelopers?
                        .Where(pmd => pmd.IsActive)
                        .Select(pmd => new DeveloperDTO
                        {
                            Email = pmd.Developer.User.Email!,
                            FirstName = pmd.Developer.User.FirstName,
                            LastName = pmd.Developer.User.LastName,
                            SkillLevel = pmd.Developer.SkillLevel,
                            Specialization = pmd.Developer.Specialization,
                            YearsOfExpirience = pmd.Developer.YearsOfExpirience,
                            Department = pmd.Developer.Department
                        }).ToList()
                };
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error retrieving project manager with ID {ProjectManagerId}", id);
                return null;
            }
        }

        public async Task<bool> AssignDeveloperAsync(AssignDeveloperDTO assignDto)
        {

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var projectManager = await _context.ProjectManagers.FirstOrDefaultAsync(pm => pm.Id == assignDto.ProjectManagerId);

                if (projectManager == null)
                {
                    _logger.LogWarning("Project manager with ID {ProjectManagerId} not found", assignDto.ProjectManagerId);
                    return false;
                }

                var developer = await _context.Developers
                    .FirstOrDefaultAsync(d => d.Id == assignDto.DeveloperId);
                if (developer == null)
                {
                    _logger.LogWarning("Developer with ID {DeveloperId} not found", assignDto.DeveloperId);
                    return false;
                }

                var existingAssignment = await _context.ProjectManagerDevelopers
                    .FirstOrDefaultAsync(pmd => pmd.ProjectManagerId == assignDto.ProjectManagerId && pmd.DeveloperId == assignDto.DeveloperId);

                if (existingAssignment == null)
                {
                    var assignment = new ProjectManagerDeveloper
                    {
                        ProjectManagerId = assignDto.ProjectManagerId,
                        DeveloperId = assignDto.DeveloperId,
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true
                    };

                    _context.ProjectManagerDevelopers.Add(assignment);
                   // await _context.SaveChangesAsync();
                }
                else
                {
                    _logger.LogInformation("Developer with ID {DeveloperId} is already assigned to project manager with ID {ProjectManagerId}", assignDto.DeveloperId, assignDto.ProjectManagerId);
                    existingAssignment.IsActive = true;
                    existingAssignment.UpdatedAt = DateTime.UtcNow;
                }
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning developer with ID {DeveloperId} to project manager with ID {ProjectManagerId}", assignDto.DeveloperId, assignDto.ProjectManagerId);

                await transaction.RollbackAsync();

                return false;
            }
            return true;
        }

        public async Task<bool> RemoveDeveloperAsync(AssignDeveloperDTO assignDto)
        {

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var projectManager = await _context.ProjectManagers.FirstOrDefaultAsync(pm => pm.Id == assignDto.ProjectManagerId);

                if (projectManager == null)
                {
                    _logger.LogWarning("Project manager with ID {ProjectManagerId} not found", assignDto.ProjectManagerId);
                    return false;
                }

                var developer = await _context.Developers
                    .FirstOrDefaultAsync(d => d.Id == assignDto.DeveloperId);
                if (developer == null)
                {
                    _logger.LogWarning("Developer with ID {DeveloperId} not found", assignDto.DeveloperId);
                    return false;
                }

                var existingAssignment = await _context.ProjectManagerDevelopers
                    .FirstOrDefaultAsync(pmd => pmd.ProjectManagerId == assignDto.ProjectManagerId && pmd.DeveloperId == assignDto.DeveloperId);

                if (existingAssignment != null)
                {
                    existingAssignment.IsActive = false;
                    existingAssignment.UpdatedAt = DateTime.UtcNow;

                    await _context.SaveChangesAsync();
                }
                else
                {
                    _logger.LogInformation("Developer with ID {DeveloperId} is not  assigned to project manager with ID {ProjectManagerId}", assignDto.DeveloperId, assignDto.ProjectManagerId);
                    return false;
                }


                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing assignment to developer with ID {DeveloperId} to project manager with ID {ProjectManagerId}", assignDto.DeveloperId, assignDto.ProjectManagerId);

                await transaction.RollbackAsync();

                return false;
            }
            return true;
        }

        public async Task<IEnumerable<DeveloperDTO?>> GetDevelopersAsync(string projectManagerId)
        {
            try
            {
                var pmExit = await _context.ProjectManagers.FirstOrDefaultAsync(pm => pm.Id == projectManagerId);

                if (pmExit == null)
                {
                    _logger.LogWarning("Project manager with ID {ProjectManagerId} not found", projectManagerId);
                    return null;
                }

                var developers = await _context.ProjectManagerDevelopers
                    .Where(pmd => pmd.ProjectManagerId == projectManagerId && pmd.IsActive)
                    .Include(pmd => pmd.Developer)
                    .ThenInclude(d => d.User)
                    .ToListAsync();

                return developers.Select(pmd => new DeveloperDTO
                    {
                        Email = pmd.Developer.User.Email!,
                        FirstName = pmd.Developer.User.FirstName,
                        LastName = pmd.Developer.User.LastName,
                        SkillLevel = pmd.Developer.SkillLevel,
                        Specialization = pmd.Developer.Specialization,
                        YearsOfExpirience = pmd.Developer.YearsOfExpirience,
                        Department = pmd.Developer.Department
                    });

            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error retrieving developers for project manager with ID {ProjectManagerId}", projectManagerId);
                return null;
            }
        }
    }
}
