using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TimeManagmentAPI.Models;

namespace TimeManagmentAPI.Data
{
    public class TimeManagementContext : IdentityDbContext<User>
    {
        public TimeManagementContext(DbContextOptions<TimeManagementContext> options) : base(options)
        {
        }

        public DbSet<Project> Projects { get; set; }
        public DbSet<ManagedTask> Tasks { get; set; }
    }
}
