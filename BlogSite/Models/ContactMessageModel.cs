namespace BlogSite.Models
{
    public class ContactMessageModel
    {
        public int ContactMessageId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
