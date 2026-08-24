using SalesBuzz.Shared.Authorization;
using SalesBuzz.Shared.Filters;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace Final_Task.Data
{
    public class RolePermission
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int RoleId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Operation { get; set; } = string.Empty;

        public PermissionKind Permission { get; set; }

        public Role? Role { get; set; }
    }
}