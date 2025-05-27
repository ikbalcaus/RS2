namespace eBooks.Models.Responses
{
    public class NotificationsRes
    {
        public int NotificationId { get; set; }
        public int UserId { get; set; }
        public int? BookId { get; set; }
        public int? PublisherId { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; }
    }
}
