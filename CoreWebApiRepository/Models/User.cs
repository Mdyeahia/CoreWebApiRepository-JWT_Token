namespace CoreWebApiRepository.Models
{
    public class User
    {
        public int UserId { get; set; }=0;
        public string UserName { get; set; }    
        public string Email { get; set; }
        public string Password { get; set; }
        public string Token { get; set; } = String.Empty;
        public string Message { get; set; }= String.Empty;
    }
}
