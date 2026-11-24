using DummyLoginApi.Services;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Register application services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<INotificationService, NotificationService>();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();
