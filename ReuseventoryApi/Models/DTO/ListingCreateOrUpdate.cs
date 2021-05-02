using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReuseventoryApi.Helpers;

namespace ReuseventoryApi.Models.DTO
{
    [ModelBinder(typeof(JsonWithFilesFormDataModelBinder), Name = "json")]
    public class ListingCreateOrUpdate
    {
        [Required]
        [MaxLength(256)]
        public string name { get; set; }

        [MaxLength(256)]
        public string description { get; set; }

        public double? price { get; set; }

        public ICollection<string> tags { get; set; }

        public IFormFile image { get; set; }
    }
}