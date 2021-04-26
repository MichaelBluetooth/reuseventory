using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReuseventoryApi.Models
{
    public class ListingImage : BaseModel
    {
        public byte[] image { get; set; }

        public string fileName { get; set; }
        public string contentType { get; set; }

        [Required]
        [ForeignKey("listing")]
        public Guid? listingId { get; set; }
        public Listing listing { get; set; }
    }
}