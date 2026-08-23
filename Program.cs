using Final_Task.Data;
using Microsoft.AspNetCore.Identity;
using SalesBuzz.Shared.Authorization;
using SalesBuzz.Shared.Data;
using SalesBuzz.Shared.Filters;
using SalesBuzz.Shared.Middleware;

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

    public SalesBuzzPermissionService(
        IPermissions permissions)
    {
        _permissions = permissions;
    }

    public bool HasPermission(
        string operation,
        PermissionKind permission)
    {
        if (string.IsNullOrWhiteSpace(operation))
        {
            return false;
        }

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