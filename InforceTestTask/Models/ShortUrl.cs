namespace InforceTestTask.Models
{
    public class ShortUrl
    {
        public Guid Id { get; set; }
        public string Url { get; set; } = string.Empty;
        public string Short { get; set; } = string.Empty;
        public DateTime CreationDate { get; set; }
        public User User { get; set; } = new User();
    }
}
