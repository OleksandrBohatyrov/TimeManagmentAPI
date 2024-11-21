using Microsoft.AspNetCore.Identity;
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
        public DbSet<ManagedTask> Tasks { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var passwordHasher = new PasswordHasher<User>();
            var adminUser = new User
            {
                Id = 1,
                Username = "admin",
                Email = "admin@example.com",
                Role = "Admin"
            };
            adminUser.PasswordHash = passwordHasher.HashPassword(adminUser, "admin");

            modelBuilder.Entity<User>().HasData(adminUser);
        }
    }
}
