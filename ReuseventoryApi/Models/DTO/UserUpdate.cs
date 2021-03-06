using System.ComponentModel.DataAnnotations;

namespace ReuseventoryApi.Models.DTO
{
    public class UserUpdate
    {
        [Required]
        public string username { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string phone { get; set; }
    }
}