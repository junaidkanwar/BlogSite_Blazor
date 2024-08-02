namespace BlogSite.Models
{
    public class BlogModel
    {
        public int BlogId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int UserId { get; set; } // Foreign key to User
        public string SubmittedBy { get; set; }
    }
}
