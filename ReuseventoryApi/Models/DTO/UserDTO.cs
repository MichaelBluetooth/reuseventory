using System.Collections.Generic;

namespace ReuseventoryApi.Models.DTO
{
    public class UserDTO : BaseModel
    {
        public string username { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public bool isAdmin { get; set; }
        public ICollection<Listing> listings { get; set; }
    }
}