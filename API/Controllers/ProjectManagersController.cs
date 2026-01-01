using Application.DTOs;
using Application.Interfaces;
using Domain.Enums;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/v1/projectManagers")]
    [ApiController]
    public class ProjectManagersController : ControllerBase
    {
        private readonly ILogger<ProjectManagersController> _logger;
        private readonly IProjectManagerServices _projectManagerService;
        public ProjectManagersController(ILogger<ProjectManagersController> logger, IProjectManagerServices projectManagerServices)
        {
            this._logger = logger;
            this._projectManagerService = projectManagerServices;
        }
        [HttpPost]
        [Authorize(Roles = UserRoles.ProjectManager)]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("GetAll ProjectManagers endpoint called.");
            var projectManagers = await this._projectManagerService.GetAllAsync();

            if (projectManagers == null || !projectManagers.Any())
            {
                _logger.LogWarning("No Project Managers found.");
                return NotFound(new { Status = false, Message = "No Project Managers found." });
            }

            return Ok(new { Status = true, Message = "Project Managers retrieved successfully.", Data = projectManagers });
        }
        [HttpPost]
        [Authorize(Roles = UserRoles.ProjectManager)]
        [Route("get-by-id/{id}")]
        public async Task<IActionResult> GetById([FromRoute] string id)
        {
            this._logger.LogInformation("GetById ProjectManager endpoint called. Id: {id}", id);

            var projectManager = await this._projectManagerService.GetByIdAsync(id);

            if (projectManager == null)
            {
                this._logger.LogWarning("Project Manager not found. Id: {id}", id);

                return NotFound(new { Status = false, Message = "Project Manager not found." });
            }

            return Ok(new { Status = true, Message = "Project Manager retrieved successfully.", Data = projectManager });
        }

        [HttpPost("assign-developers")]
        [Authorize(Roles = UserRoles.ProjectManager)]
        public async Task<IActionResult> AssignDevelopersToProject([FromBody] AssignDeveloperDTO assignDto)
        {
            _logger.LogInformation("AssignDevelopersToProject endpoint called. {@request}", assignDto);

            // Implementation for assigning developers to a project would go here.
            if (assignDto.ProjectManagerId == null || assignDto.DeveloperId == null)
            {
                _logger.LogWarning("Invalid request data. {@request}", assignDto);

                return BadRequest(new { Status = false, Message = "ProjectManagerId and DeveloperId are required." });
            }

            var results = await this._projectManagerService.AssignDeveloperAsync(assignDto);

            if (!results)
            {
                _logger.LogError("Failed to assign developers. {@request}", assignDto);
                return StatusCode(StatusCodes.Status500InternalServerError, new { Status = false, Message = "Failed to assign developers to project." });
            }

            return Ok(new { Status = true, Message = "Developers assigned to project successfully." });
        }

        [HttpPost("remove-assigned-developer")]
        [Authorize(Roles = UserRoles.ProjectManager)]
        public async Task<IActionResult> RemoveAssignDevelopersToProject([FromBody] AssignDeveloperDTO assignDto)
        {
            _logger.LogInformation("RemoveAssignDevelopersToProject endpoint called. {@request}", assignDto);

            // Implementation for assigning developers to a project would go here.
            if (assignDto.ProjectManagerId == null || assignDto.DeveloperId == null)
            {
                _logger.LogWarning("Invalid request data. {@request}", assignDto);

                return BadRequest(new { Status = false, Message = "ProjectManagerId and DeveloperId are required." });
            }

            var results = await this._projectManagerService.RemoveDeveloperAsync(assignDto);

            if (!results)
            {
                _logger.LogError("Failed to remove assignment to developer. {@request}", assignDto);
                return StatusCode(StatusCodes.Status500InternalServerError, new { Status = false, Message = "Failed to remove assignment to developer from project." });
            }

            return Ok(new { Status = true, Message = "Developers assignment  to project revoked successfully." });
        }

        [HttpGet("developers/{projectManagerId}")]
        [Authorize(Roles = UserRoles.ProjectManager)]
        public async Task<IActionResult> GetDevelopersByProjectManagerId([FromRoute] string projectManagerId)
        {
            _logger.LogInformation("GetDevelopersByProjectManagerId endpoint called. ProjectManagerId: {projectManagerId}", projectManagerId);

            var developers = await this._projectManagerService.GetDevelopersAsync(projectManagerId);

            if (developers == null || !developers.Any())
            {
                _logger.LogWarning("No developers found for Project Manager Id: {projectManagerId}", projectManagerId);
                return NotFound(new { Status = false, Message = "No developers found for the specified Project Manager." });
            }
            return Ok(new { Status = true, Message = "Developers retrieved successfully.", Data = developers });
        }
    }
}
