using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using TimeManagmentAPI.Models;

namespace TimeManagmentAPI.Data
{
    public class TimeManagementContext : DbContext
    {
        public TimeManagementContext(DbContextOptions<TimeManagementContext> options)
               : base(options)
        {
        }


        public DbSet<User> Users { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<TimeTask> Tasks { get; set; }
    }
}
