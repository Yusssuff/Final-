using SalesBuzz.Shared.Authorization;
using SalesBuzz.Shared.Filters;

namespace Final_Task.Services
{
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
                permission);
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
                explicitly: true);
        }

        public void RefreshRolePermissions(
            string roleId)
        {
            if (string.IsNullOrWhiteSpace(roleId))
            {
                return;
            }

            _permissions.UpdateUserPermissions(
                roleId.Trim());
        }
    }
}