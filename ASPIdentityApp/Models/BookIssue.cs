using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASPIdentityApp.Models
{
    public class BookIssue
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int BookId { get; set; }

        [ForeignKey("BookId")]
        public Book Book { get; set; }

        [Required]
        public int MemberId { get; set; }

        [ForeignKey("MemberId")]
        public Member Member { get; set; }

        [Required]
        public DateTime IssueDate { get; set; } = DateTime.Now;

        [Required]
        public DateTime DueDate { get; set; }

        public DateTime? ReturnDate { get; set; }

        [Range(0, double.MaxValue)]
        public decimal FineAmount { get; set; } = 0;

        [Required]
        public string Status { get; set; } = "Issued"; // Issued, Returned, Overdue

        public string Notes { get; set; }
    }
}
