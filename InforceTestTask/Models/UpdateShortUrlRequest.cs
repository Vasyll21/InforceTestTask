namespace InforceTestTask.Models
{
    public class UpdateShortUrlRequest
    {
        public string Url { get; set; } = string.Empty;
        public string Short { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
    }
}
