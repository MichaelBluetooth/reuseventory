using System;
using System.ComponentModel.DataAnnotations;

namespace ReuseventoryApi.Models
{
    public abstract class BaseModel
    {
        [Key]
        public Guid? id { get; set; }
    }
}