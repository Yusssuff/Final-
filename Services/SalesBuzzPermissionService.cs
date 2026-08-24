using Final_Task.Data;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

using SalesBuzz.Shared.Authorization;
using SalesBuzz.Shared.Filters;
using System.Security.Claims;

namespace Final_Task.Services;

public sealed class SalesBuzzPermissionService
{
    private readonly AppDbContext _db;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public SalesBuzzPermissionService(
        AppDbContext db,
        IHttpContextAccessor httpContextAccessor)
    {
        _db = db;
        _httpContextAccessor = httpContextAccessor;
    }

    private int? CurrentUserId()
    {
        var user =
            _httpContextAccessor
                .HttpContext?
                .User;

        if (user?.Identity?.IsAuthenticated != true)
        {
            return null;
        }

        var userId =
            user.FindFirstValue(
                ClaimTypes.NameIdentifier
            );

        if (!int.TryParse(
            userId,
            out var id))
        {
            return null;
        }

        return id;
    }

    public async Task<bool> HasPermission(
        string operation,
        PermissionKind permission)
    {
        if (string.IsNullOrWhiteSpace(operation))
        {
            return false;
        }

        var userId =
            CurrentUserId();

        if (!userId.HasValue)
        {
            return false;
        }

        return await _db.RolePermissions
            .AnyAsync(
                rp =>
                    rp.Role != null &&
                    rp.Role.Users.Any(
                        u => u.Id == userId.Value
                    ) &&
                    rp.Operation == operation &&
                    rp.Permission == permission
            );
    }

    public async Task<bool> HasExplicitPermission(
        string operation,
        PermissionKind permission)
    {
        return await HasPermission(
            operation,
            permission
        );
    }
}