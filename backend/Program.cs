using backend.DataBase;
using backend.Services.ServiceDef;
using backend.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;
using Microsoft.Extensions.Options;
using backend.DTOs;
using Microsoft.OpenApi.Models;
using backend.Models;
using backend.Controllers;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// ------------------------------
// Configure Logging
// ------------------------------
builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration
        .WriteTo.Console()
        .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day);
});

// ------------------------------
// Add DbContext
// ------------------------------
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// ------------------------------
// Configure JWT Authentication
// ------------------------------
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key),

        // map claim types (important so [Authorize(Roles="Admin")] works)
        NameClaimType = JwtRegisteredClaimNames.Sub,
        RoleClaimType = ClaimTypes.Role
    };

    // optional: helpful for debugging
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = ctx =>
        {
            var logger = ctx.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogWarning("JWT auth failed: {0}", ctx.Exception.Message);
            return Task.CompletedTask;
        }
    };
});

// ------------------------------
// Configure Services
// ------------------------------
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddScoped<ITesseractOcrService, TesseractOcrService>();
builder.Services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
builder.Services.AddHostedService<OcrBackgroundService>();

builder.Services.Configure<UploadSettings>(builder.Configuration.GetSection("UploadSettings"));
builder.Services.Configure<TesseractSettings>(builder.Configuration.GetSection("Tesseract"));

// ------------------------------
// Controllers + Swagger
// ------------------------------
builder.Services.AddControllers().AddJsonOptions(opts =>
{
    opts.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ------------------------------
// Build app
// ------------------------------
var app = builder.Build();

// ------------------------------
// Ensure Upload/Text folders exist
// ------------------------------
var uploadSettings = app.Services.GetRequiredService<IOptions<UploadSettings>>().Value;
Directory.CreateDirectory(uploadSettings.ImageFolder);
Directory.CreateDirectory(uploadSettings.TextFolder);

// ------------------------------
// Auto-create admin if none exists
// ------------------------------
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var adminExists = await db.Users.AnyAsync(u => u.Role == "Admin");

    if (!adminExists)
    {
        var salt = AuthController.GenerateSalt();
        var admin = new User
        {
            Id = Guid.NewGuid(),
            UserName = "admin",
            Email = "admin@example.com",
            Role = "Admin",
            PasswordSalt = salt,
            PasswordHash = AuthController.HashPassword("Admin123!", salt)
        };

        db.Users.Add(admin);
        await db.SaveChangesAsync();
        Log.Information("Admin user created: admin@example.com / Admin123!");
    }
}


// ------------------------------
// Middleware
// ------------------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
