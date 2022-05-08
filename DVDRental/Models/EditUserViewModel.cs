﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace DVDRental.Models
{
    public class EditUserViewModel
    {
        public string Id { get; set; }


        [Required]
        [Column(TypeName = "nvarchar(100)")]
        public string FirstName { get; set; }


        [Required]
        [Column(TypeName = "nvarchar(100)")]
        public string LastName { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
