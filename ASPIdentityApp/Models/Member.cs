using ASPIdentityApp.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASPIdentityApp.Models
{
    public class Member
    {
        [Key]
        public int Id { get; set; }

        //[Required]
        //public string UserId { get; set; }

        //[ForeignKey("UserId")]
        //public ApplicationUser User { get; set; }

        //[Required]
        //[StringLength(20)]
        //public string MemberId { get; set; } // Custom member ID like MEM001

        [Required]
        [StringLength(20)]
        public string MemberCode { get; set; } // Changed from MemberId to MemberCode

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(100)]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Phone]
        public string PhoneNumber { get; set; }

        public string Address { get; set; }

        [Required]
        public DateTime RegistrationDate { get; set; } = DateTime.Now;

        [Required]
        public DateTime MembershipExpiry { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal TotalFees { get; set; }

        [Required]
        public string Status { get; set; } = "Active"; // Active, Inactive, Suspended
        // Navigation property - make it virtual and initialize
        public virtual ICollection<BookIssue> BookIssues { get; set; } = new List<BookIssue>();
    }
}
