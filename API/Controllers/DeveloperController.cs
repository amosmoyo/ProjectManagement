using Application.Interfaces;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/v1/Developers")]
    [ApiController]
    public class DeveloperController : ControllerBase
    {
        private readonly ILogger<DeveloperController> _logger;
        private readonly IDeveloperServices _developerServices;
        public DeveloperController(ILogger<DeveloperController> logger, IDeveloperServices developerServices)
        {
            this._logger = logger;
            this._developerServices = developerServices;
        }

        [HttpGet]
        [Authorize(Roles = UserRoles.ProjectManager)]
        public async Task<IActionResult> GetAllDevelopers()
        {
            this._logger.LogInformation("GET: api/v1/Developers called");

            var developers = await this._developerServices.GetAllAsync();

            if (developers == null)
            {
                this._logger.LogWarning("No developers found");
                return NotFound(new {Status=false, Message= "No developers found" });
            }
            return Ok(new {Status = true, Message = "Developers successfully retrieved", Data=developers});
        }

        [HttpGet]
        [Authorize(Roles = UserRoles.ProjectManager)]
        [Route("get-by-id/{id}")]
        public async Task<IActionResult> GetDeveloperById([FromRoute] string id)
        {
            this._logger.LogInformation("GET: api/v1/Developers/get-by-id/{id} called", id);
            var developer = await this._developerServices.GetByIdAsync(id);
            if (developer == null)
            {
                this._logger.LogWarning("Developer not found. Id: {id}", id);
                return NotFound(new { Status = false, Message = "Developer not found" });
            }
            return Ok(new { Status = true, Message = "Developer successfully retrieved", Data = developer });
        }

        [HttpGet]
        [Authorize(Roles = UserRoles.ProjectManager)]
        [Route("get-by-id/{id}/projectmangers")]
        public async Task<IActionResult> GetDeveloperProjectManagers([FromRoute] string id)
        {
            this._logger.LogInformation("GET: api/v1/Developers/get-by-id/{id}/projectmangers called", id);

            var projectManagers = await this._developerServices.GetProjectManagersAsync(id);

            if (projectManagers == null)
            {
                this._logger.LogWarning("No Project Managers found for Developer Id: {id}", id);
                return NotFound(new { Status = false, Message = "No Project Managers found for this developer" });
            }
            return Ok(new { Status = true, Message = "Project Managers successfully retrieved", Data = projectManagers });
        }
    }
}
