using Microsoft.AspNetCore.Identity;

namespace Avacado.Services.AuthAPI.Models
{
    public class ApplicationUser:IdentityUser
    {
        public string Name { get; set; }
    }
}
