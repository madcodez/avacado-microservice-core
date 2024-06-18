using Avacado.Services.AuthAPI.Data;
using Avacado.Services.AuthAPI.Models;
using Avacado.Services.AuthAPI.Models.Dto;
using Avacado.Services.AuthAPI.Service.IService;
using Microsoft.AspNetCore.Identity;
using System.Text.RegularExpressions;

namespace Avacado.Services.AuthAPI.Service
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IJwtGeneratorService _jwtGeneratorService;
        public AuthService(AppDbContext db,UserManager<ApplicationUser> userManager,RoleManager<IdentityRole> roleManager,IJwtGeneratorService jwtGeneratorService) 
        {
            _db = db;
            _jwtGeneratorService = jwtGeneratorService;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<bool> AssignRole(string email, string roleName)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(u => u.Email.ToLower() == email.ToLower());
            if (user != null)
            {
                if (!_roleManager.RoleExistsAsync(roleName).GetAwaiter().GetResult())
                {
                    //create role if it does not exist
                    _roleManager.CreateAsync(new IdentityRole(roleName)).GetAwaiter().GetResult();
                }
                await _userManager.AddToRoleAsync(user, roleName);
                return true;
            }
            return false;

        }
        public async Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto)
        {
            var user =  _db.ApplicationUsers.FirstOrDefault(u => u.UserName.ToLower() == loginRequestDto.UserName.ToLower());

            bool isValid = await _userManager.CheckPasswordAsync(user, loginRequestDto.Password);

            if(user == null || !isValid) 
            {

                return new LoginResponseDto() { Token = "", User = null };
            }

            var token = _jwtGeneratorService.GenerateToken(user);

            UserDto userDto = new() 
            {
             Name= user.Name,
             Email=user.Email,
             ID = user.Id,
             PhoneNumber=user.PhoneNumber
            };

            return new LoginResponseDto() { User = userDto , Token= token };
        }

        public async Task<string> Register(RegisterRequestDto registerRequestDto)
        {
            ApplicationUser user = new()
            {
                UserName = registerRequestDto.Email,
                Email = registerRequestDto.Email,
                NormalizedEmail = registerRequestDto.Email.ToUpper(),
                PhoneNumber = registerRequestDto.PhoneNumber,
                Name = registerRequestDto.Name,
            };

            try
            {
                var result = await _userManager.CreateAsync(user, registerRequestDto.Password);
                if (result.Succeeded)
                {
                    var userToReturn = _db.ApplicationUsers.First(u => u.UserName == registerRequestDto.Email);

                    UserDto userDto = new()
                    { 
                     Email = userToReturn.Email,
                     ID = userToReturn.Id,
                     Name = userToReturn.Name,
                     PhoneNumber=userToReturn.PhoneNumber
                    };

                    return "";

                }
                else 
                {
                    return result.Errors.FirstOrDefault().Description;
                }
            }
            catch (Exception ex) 
            { 
              
            }
            return "error encountered";
        }
    }
}
