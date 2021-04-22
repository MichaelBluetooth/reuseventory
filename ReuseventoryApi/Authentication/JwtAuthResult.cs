namespace ReuseventoryApi.Authentication
{
    public class JwtAuthResult
    {
        public string accessToken { get; set; }
        public RefreshToken refreshToken { get; set; }
    }
}