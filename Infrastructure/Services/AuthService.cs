using Application.DTOs.Auth;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System; 
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;
        public AuthService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IConfiguration configuration, ApplicationDbContext applicationDbContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _context = applicationDbContext;
        }

        public async Task<AuthResponseDTO> RegisterAsync(RegisterDTO registerDto)
        {

            //check if luser exist
            var userExist = await _userManager.FindByEmailAsync(registerDto.Email);

            if (userExist != null)
            {
                return new AuthResponseDTO
                {
                    Success = false,
                    Message = "User with this email already exist"
                };
            }

            // Create User
            var user = new ApplicationUser
            {
                UserName = registerDto.Email,
                Email = registerDto.Email,
                FirstName = registerDto.FirstName!,
                LastName = registerDto.LastName!,
                EmailConfirmed = true
            };

            var res = await _userManager.CreateAsync(user, registerDto.Password);

            if (!res.Succeeded)
            {
                return new AuthResponseDTO
                {
                    Success = false,
                    Message = string.Join(", ", res.Errors.Select(e => e.Description))
                };
            }

            //
            if (registerDto.UserType.Equals("ProjectManager", StringComparison.OrdinalIgnoreCase))
            {
                await _userManager.AddToRoleAsync(user, UserRoles.ProjectManager);

                var projectManager = new ProjectManager
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = user.Id,
                    SkillLevel = registerDto.SkillLevel ?? "Junior",
                    YearsOfExpirience = registerDto.YearNumberOfExpirience ?? 0,
                    Department = registerDto.Department
                };

                _context.Add(projectManager);
            }
            else if (registerDto.UserType.Equals("Developer", StringComparison.OrdinalIgnoreCase))
            {
                await _userManager.AddToRoleAsync(user, UserRoles.Developer);

                var developer = new Developers
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = user.Id,
                    SkillLevel = registerDto.SkillLevel ?? "Junior",
                    Specialization = registerDto.Specialization ?? "Backend",
                    YearsOfExpirience = registerDto.YearNumberOfExpirience ?? 0,
                    Department = registerDto.Department
                };

                _context.Add(developer);
            }
            else
            {
                return new AuthResponseDTO
                {
                    Success = false,
                    Message = "Invalid user type, Must be 'ProjectManger' or 'Developer'."
                };
            }

            await _context.SaveChangesAsync();

            var token = await GenerateJwtToken(user);
            var roles = await _userManager.GetRolesAsync(user);

            return new AuthResponseDTO
            {
                Success = true,
                Message = "Success",
                Token = token,
                UserId = user.Id,
                Email = user.Email,
                UserType = registerDto.UserType,
                Roles = roles
            };
        }

        public async Task<AuthResponseDTO> LoginAsync(LoginDTO loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);

            if (user == null)
            {
                return new AuthResponseDTO
                {
                    Success = false,
                    Message = "Invalid credentials"
                };
            }

            var res = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, lockoutOnFailure: true);

            if (!res.Succeeded) 
            {
                if (res.IsLockedOut)
                {
                    return new AuthResponseDTO
                    {
                        Success = false,
                        Message = "Account is lockedOut"
                    };

                }


                return new AuthResponseDTO
                {
                    Success = false,
                    Message = "Invalid credentials"
                };
            }


            var token = await GenerateJwtToken(user);
            var roles = await _userManager.GetRolesAsync(user);

            var userType = roles.Contains(UserRoles.ProjectManager) ? "ProjectManger" : "Developer";


            return new AuthResponseDTO
            {
                Success = true,
                Message = "Success",
                Token = token,
                UserId = user.Id,
                Email = user.Email,
                UserType = userType,
                Roles = roles
            };
        }

        private async Task<string> GenerateJwtToken(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.UserName ?? string.Empty)
        };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration["JwtSettings:Secret"] ?? "YourSuperSecretKeyThatIsAtLeast32CharactersLong!"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(
                    double.Parse(_configuration["JwtSettings:ExpirationInHours"] ?? "24")),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
