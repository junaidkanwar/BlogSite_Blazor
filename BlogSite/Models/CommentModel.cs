namespace BlogSite.Models
{
    public class CommentModel
    {
        public int CommentId { get; set; }
        public int BlogId { get; set; }
        public int UserId { get; set; }
        public string CommentContent { get; set; }

        public string Author { get; set; }

        // Assuming this property stores the username of the author
    }
}
