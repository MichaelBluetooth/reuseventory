using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReuseventoryApi.Models.DTO
{
    public class ListingDTO : BaseModel
    {
        [Required]
        [MaxLength(256)]
        public string name { get; set; }

        [EmailAddress]
        [MaxLength(256)]
        public string description { get; set; }

        public double? price { get; set; }

        public UserDTO user { get; set; }

        public ICollection<string> tags { get; set; }

        public bool hasImage { get; set; }
    }
}