using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TimeManagmentAPI.Data;
using TimeManagmentAPI.Models;

var builder = WebApplication.CreateBuilder(args);

// Настройка базы данных и контекста
builder.Services.AddDbContext<TimeManagementContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
    new MySqlServerVersion(new Version(8, 0, 25))));

// Добавляем ASP.NET Identity
builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<TimeManagementContext>()
    .AddDefaultTokenProviders();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/api/Users/login";
        options.Events.OnRedirectToLogin = context =>
        {
            context.Response.StatusCode = 401;
            return Task.CompletedTask;
        };
    });





// Добавляем CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhostOrigins", builder =>
    {
        builder.WithOrigins("http://localhost:3000")
               .AllowAnyMethod()    
               .AllowAnyHeader()
               .AllowCredentials();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Включаем CORS для всех запросов
app.UseCors("AllowLocalhostOrigins");

app.UseAuthorization();
app.UseAuthentication();
app.MapControllers();
app.Run();
