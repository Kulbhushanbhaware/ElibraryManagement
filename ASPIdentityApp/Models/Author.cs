using System.ComponentModel.DataAnnotations;

namespace ASPIdentityApp.Models
{
    public class Author
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(100)]
        public string LastName { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        public string? Biography { get; set; }
        [Required]
        public DateTime DateOfBirth { get; set; }

        public string FullName => $"{FirstName} {LastName}";

        // Navigation property - make it virtual and initialize it
        public virtual ICollection<Book> Books { get; set; } = new List<Book>();
    }
}
