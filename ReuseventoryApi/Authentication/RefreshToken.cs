using System;

namespace ReuseventoryApi.Authentication
{
    public class RefreshToken
    {
        public string userName { get; set; }    // can be used for usage tracking
        // can optionally include other metadata, such as user agent, ip address, device name, and so on

        public string tokenString { get; set; }

        public DateTime expireAt { get; set; }
    }
}