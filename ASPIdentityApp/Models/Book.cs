using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASPIdentityApp.Models
{
    public class Book
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(255)]
        public string Title { get; set; }

        [Required(ErrorMessage = "ISBN is required")]
        [StringLength(13)]
        public string ISBN { get; set; }

        [Required(ErrorMessage = "Author is required")]
        [Display(Name = "Author")]
        public int AuthorId { get; set; }

        [ForeignKey("AuthorId")]
        public virtual Author Author { get; set; }

        [Required(ErrorMessage = "Publisher is required")]
        [Display(Name = "Publisher")]
        public int PublisherId { get; set; }

        [ForeignKey("PublisherId")]
        public virtual Publisher Publisher { get; set; }

        [Required(ErrorMessage = "Published date is required")]
        [Display(Name = "Published Date")]
        public DateTime PublishedDate { get; set; }

        [Required(ErrorMessage = "Total copies is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Total copies must be at least 1")]
        [Display(Name = "Total Copies")]
        public int TotalCopies { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int AvailableCopies { get; set; }

        public string? Description { get; set; }
        public string? Genre { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public virtual ICollection<BookIssue> BookIssues { get; set; } = new List<BookIssue>();
    }
}