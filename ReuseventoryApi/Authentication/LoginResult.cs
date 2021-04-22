namespace ReuseventoryApi.Authentication
{
    public class LoginResult
    {
        public string username { get; set; }
        public string accessToken { get; set; }
        public string refreshToken { get; set; }
    }
}