using Final_Task.Data;
using Microsoft.AspNetCore.Identity;
using SalesBuzz.Shared.Authorization;
using SalesBuzz.Shared.Data;
using SalesBuzz.Shared.Filters;
using SalesBuzz.Shared.Middleware;
using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();
builder.Services.AddMemoryCache();

builder.Services.AddSalesBuzzCurrentBU();

builder.Services.AddSalesBuzzDb<AppDbContext>(
    builder.Configuration
);

builder.Services.AddSalesBuzzJwt(
    builder.Configuration
);

builder.Services.AddAuthorization();

builder.Services.AddScoped<
    IPasswordHasher<User>,
    PasswordHasher<User>
>();

builder.Services.AddScoped<SalesBuzzPermissionService>();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy
            .WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// IMPORTANT:
// This must use the same policy name that was registered above.
app.UseCors("AllowAngularApp");

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseSalesBuzzTokenValidation();

app.UseAuthorization();

app.MapControllers();

app.Run();


public sealed class SalesBuzzPermissionService
{
    private readonly IPermissions _permissions;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public SalesBuzzPermissionService(
        IPermissions permissions,
        IHttpContextAccessor httpContextAccessor)
    {
        _permissions = permissions;
        _httpContextAccessor = httpContextAccessor;
    }

    private string CurrentUserRole()
    {
        try
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user?.Identity?.IsAuthenticated == true)
            {
                return user.FindFirstValue(ClaimTypes.Role) ?? string.Empty;
            }
        }
        catch
        {
            // ignore and fall back to permissions provider
        }

        return string.Empty;
    }

    public bool HasPermission(
        string operation,
        PermissionKind permission)
    {
        if (string.IsNullOrWhiteSpace(operation))
        {
            return false;
        }

        // First apply simple role-based rules for this app's needs.
        var role = CurrentUserRole();

        if (!string.IsNullOrWhiteSpace(role))
        {
            if (role.Equals("Admin", StringComparison.OrdinalIgnoreCase))
            {
                // Admin has all permissions
                return true;
            }

            if (role.Equals("User", StringComparison.OrdinalIgnoreCase))
            {
                // Users are read-only for Products, but can manage Orders
                if (operation.Equals("Products", StringComparison.OrdinalIgnoreCase))
                {
                    return permission == PermissionKind.Read;
                }

                if (operation.Equals("Orders", StringComparison.OrdinalIgnoreCase))
                {
                    // allow full CRUD on Orders for normal users
                    return permission == PermissionKind.Create ||
                           permission == PermissionKind.Update ||
                           permission == PermissionKind.Delete ||
                           permission == PermissionKind.Read;
                }
            }
        }

        // Fall back to the configured permissions provider
        return _permissions.IsValidOperationPermission(
            operation,
            permission
        );
    }

    public bool HasExplicitPermission(
        string operation,
        PermissionKind permission)
    {
        if (string.IsNullOrWhiteSpace(operation))
        {
            return false;
        }

        var role = CurrentUserRole();

        if (!string.IsNullOrWhiteSpace(role))
        {
            if (role.Equals("Admin", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (role.Equals("User", StringComparison.OrdinalIgnoreCase))
            {
                if (operation.Equals("Products", StringComparison.OrdinalIgnoreCase))
                {
                    return permission == PermissionKind.Read;
                }

                if (operation.Equals("Orders", StringComparison.OrdinalIgnoreCase))
                {
                    return permission == PermissionKind.Create ||
                           permission == PermissionKind.Update ||
                           permission == PermissionKind.Delete ||
                           permission == PermissionKind.Read;
                }
            }
        }

        return _permissions.IsValidOperationPermission(
            operation,
            permission,
            explicitly: true
        );
    }

    public void UpdateUserPermissions(
        string roleId)
    {
        if (string.IsNullOrWhiteSpace(roleId))
        {
            return;
        }

        _permissions.UpdateUserPermissions(roleId);
    }
}