using Castle.Components.DictionaryAdapter;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using KeyAttribute = System.ComponentModel.DataAnnotations.KeyAttribute;

namespace WebApplication1.Entity
{
    [Table("Users")]
    public class User
    {
       
        [Key]
        public Guid Id { get; set; }

        [StringLength(50)]
        public string? FullName { get; set; }


        [Required]
        [StringLength(30)]
        public string Username { get; set; }


        [Required]
        [StringLength(100)]
        public string Password { get; set; }


        public bool Locked { get; set; } = false;


        public DateTime CreationAt { get; set; }= DateTime.Now;

        [Required]
        [StringLength(50)]
        public string Role { get; set; } = "user";

    }
}
