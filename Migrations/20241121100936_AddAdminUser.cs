using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TimeManagmentAPI.Migrations
{
    public partial class AddAdminUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var passwordHasher = new PasswordHasher<object>();
            var hashedPassword = passwordHasher.HashPassword(null, "admin");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Username", "Email", "PasswordHash", "Role" },
                values: new object[] { "admin", "admin@example.com", hashedPassword, "Admin" }
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Username",
                keyValue: "admin"
            );
        }
    }
}
