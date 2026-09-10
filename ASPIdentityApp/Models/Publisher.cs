using System.ComponentModel.DataAnnotations;

namespace ASPIdentityApp.Models
{
    public class Publisher
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string? Name { get; set; }

        [Required]
        [StringLength(500)]
        public string? Address { get; set; }

        [Phone]
        public string Phone { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        // Navigation property - make it virtual and initialize it
        public virtual ICollection<Book> Books { get; set; } = new List<Book>();
    }
}
