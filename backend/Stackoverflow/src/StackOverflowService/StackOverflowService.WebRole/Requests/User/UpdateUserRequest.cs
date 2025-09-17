namespace StackOverflowService.WebRole.Requests.User
{
    public class UpdateUserRequest
    {
        public string Name { get; set; } = default!;
        public string Lastname { get; set; } = default!;
        public string State { get; set; } = "";
        public string City { get; set; } = "";
        public string Address { get; set; } = "";
    }
}