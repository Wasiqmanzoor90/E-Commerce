﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_Commerce.Models.DomainModel
{
    public class Adress
    {
        [Key]
        public Guid AdressId { get; set; }
        public required string Street1 { get; set; }
        public required string Street2 { get; set; }
        public required string District { get; set; }
        public required string State { get; set; }
        public required string Pincode { get; set; }
        public required string Phone { get; set; }
        public required string Landmark { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        public required Guid UserId { get; set; }
        [ForeignKey("UserId")]
        public User? user { get; set; }

    }
}
