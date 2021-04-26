using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ReuseventoryApi.Models
{
    public class User : BaseModel
    {
        [Required]
        [MaxLength(256)]
        public string username { get; set; }

        [EmailAddress]
        [MaxLength(254)]
        public string email { get; set; }

        [MaxLength(20)]
        public string phone { get; set; }

        [Required]
        [MaxLength(500)]
        public string password { get; set; }

        public bool isAdmin { get; set; }

        public ICollection<Listing> listings { get; set; }
    }
}