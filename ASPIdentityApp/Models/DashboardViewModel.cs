namespace ASPIdentityApp.Models
{
    public class DashboardViewModel
    {
        public int TotalBooks { get; set; }
        public int TotalAuthors { get; set; }
        public int TotalMembers { get; set; }
        public int IssuedBooks { get; set; }
        public List<Book> RecentBooks { get; set; }
    }
}
