namespace NotificationService.DTOs
{
    public class FinalAnswerNotifyResult
    {
        public string AnswerId { get; set; }
        public int SentCount { get; set; }
        public bool AllSucceeded { get; set; }
    }
}
