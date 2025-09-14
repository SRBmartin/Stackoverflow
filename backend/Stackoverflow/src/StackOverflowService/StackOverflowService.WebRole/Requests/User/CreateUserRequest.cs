namespace StackOverflowService.WebRole.Requests.User
{
	public class CreateUserRequest
	{
        public string Name { get; set; } = default!;
        public string Lastname { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
        public string Gender { get; set; } = ""; // "M", "F"
        public string State { get; set; } = "";
        public string City { get; set; } = "";
        public string Address { get; set; } = "";
    }
}