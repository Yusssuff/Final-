using System.ComponentModel.DataAnnotations;
using System.Data;

namespace Final_Task.Data
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;


        [Required]
        public int RoleId { get; set; }

        public Role? Role { get; set; }
    }
}