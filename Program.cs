using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TimeManagmentAPI.Data;
using TimeManagmentAPI.Models;

var builder = WebApplication.CreateBuilder(args);

// Добавляем контроллеры
builder.Services.AddControllers();

// Добавляем поддержку Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Настраиваем CORS (если требуется)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", builder =>
    {
        builder.WithOrigins("http://localhost:3000") // Адрес React-приложения
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials();
    });
});

// Если вы используете Entity Framework и контекст базы данных
builder.Services.AddDbContext<TimeManagementContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
    new MySqlServerVersion(new Version(8, 0, 25))));

builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<TimeManagementContext>()
    .AddDefaultTokenProviders();

// Добавление службы сессий
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true; // Обязательно для GDPR
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Время жизни сессии
});


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    // Подключаем Swagger
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    });
}

// Включаем CORS
app.UseCors("AllowReactApp");

// Настраиваем маршрутизацию и контроллеры
app.UseSession();
app.UseRouting();
app.UseAuthorization();
app.MapControllers();

app.Run();
