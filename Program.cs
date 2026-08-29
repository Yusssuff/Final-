using Final_Task.Data;
using Final_Task.Services;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using SalesBuzz.Shared.Authorization;
using SalesBuzz.Shared.Data;
using SalesBuzz.Shared.Helpers;
using SalesBuzz.Shared.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpContextAccessor();
builder.Services.AddMemoryCache();

// SalesBuzz Business Unit support
builder.Services.AddSalesBuzzCurrentBU();

// SalesBuzz DB infrastructure
builder.Services.AddSalesBuzzDb<AppDbContext>(
    builder.Configuration
);

builder.Services.AddSalesBuzzJwt(
    builder.Configuration
);

// Ensure AppDbContext is configured with the application's connection string
// so EF / direct DB connections (e.g. _db.Database.GetDbConnection()) have a valid ConnectionString.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

// ASP.NET authorization
builder.Services.AddAuthorization();

// Password hashing
builder.Services.AddScoped<
    IPasswordHasher<User>,
    PasswordHasher<User>
>();

// Application permission service
builder.Services.AddScoped<
    SalesBuzzPermissionService
>();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AllowAngularApp",
        policy =>
        {
            policy
                .WithOrigins(
                    "http://localhost:4200"
                )
                .AllowAnyHeader()
                .AllowAnyMethod();
        }
    );
});

var app = builder.Build();

var frontendRoot = Path.Combine(app.Environment.ContentRootPath, "final-frontend", "dist", "final-frontend", "browser");

if (Directory.Exists(frontendRoot))
{
    app.UseDefaultFiles();
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(frontendRoot),
        RequestPath = ""
    });

    app.MapFallback(async context =>
    {
        var indexPath = Path.Combine(frontendRoot, "index.html");
        if (File.Exists(indexPath))
        {
            context.Response.ContentType = "text/html; charset=utf-8";
            await context.Response.SendFileAsync(indexPath);
            return;
        }

        context.Response.StatusCode = StatusCodes.Status404NotFound;
    });
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAngularApp");

app.UseStaticHttpContext();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseSalesBuzzTokenValidation();

app.UseAuthorization();

app.MapControllers();

app.Run();
