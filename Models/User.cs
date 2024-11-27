using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace TimeManagmentAPI.Models
{
    public class User : IdentityUser
    {   
        public string Role { get; set; }
    }
}
