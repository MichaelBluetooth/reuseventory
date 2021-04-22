using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReuseventoryApi.Models
{
    public class ListingTag : BaseModel
    {
        [Required]
        [MaxLength(256)]
        public string name { get; set; }

        [ForeignKey("listing")]
        public Guid? listingId { get; set; }
        public Listing listing { get; set; }
    }
}