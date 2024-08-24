using EggProductionProject_MVC.Models;
using Microsoft.AspNetCore.Identity;

namespace EggProductionProject_MVC.Data
{
    public class EggUser:IdentityUser<int>
    {
        public string? Name { get; set; }
        
    }
}
