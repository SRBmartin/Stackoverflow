namespace HealthMonitoringService.Domain.Entities
{
    public class AlertEmail
    {
        public string Email { get; }

        public AlertEmail(string email)
        {
            Email = email;
        }

    }
}
