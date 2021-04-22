using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReuseventoryApi.Models
{
    public class Listing : BaseModel
    {
        [Required]
        [MaxLength(256)]
        public string name { get; set; }

        [EmailAddress]
        [MaxLength(256)]
        public string description { get; set; }

        public byte[] image { get; set; }

        [ForeignKey("user")]
        public Guid? userId { get; set; }
        public User user { get; set; }

        public ICollection<ListingTag> tags { get; set; }
    }
}