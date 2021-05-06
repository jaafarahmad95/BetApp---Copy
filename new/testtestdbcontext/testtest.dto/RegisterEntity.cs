using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using testtest;
using testtest.Models;

namespace testtest.dto
{
    public class RegisterEntity
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare(nameof(Password), ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        [Required]
        [Display(Name = "UserName")]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "LastName")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Country")]
        public string Country { get; set; }
        
        [Required]
        [Display(Name = "City")]
        public string City { get; set; }

        [Required]
        [Display(Name = "BirthDate")]
        [MinAge(18)]
        public DateTime birthdate { get; set; }

        [Required]
        [Display(Name="Personal")]
        public string PersonalID { get; set; }

     




    }
}
