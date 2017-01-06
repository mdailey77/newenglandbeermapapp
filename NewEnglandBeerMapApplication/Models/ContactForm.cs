using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace NewEnglandBeerMapApplication.Models
{
    public class ContactForm
    {
        
        [Required(ErrorMessage = "You must provide your email.")]
        [StringLength(300, MinimumLength = 6)]
        public string contactEmail { get; set; } 
        [Required(ErrorMessage = "You must provide your first name.")]
        [StringLength(200, MinimumLength = 1)]
        public string contactFirstName { get; set; }        
        [Required(ErrorMessage = "You must provide your last name.")]
        [StringLength(200, MinimumLength = 1)]
        public string contactLastName { get; set; }
        [Required(ErrorMessage = "You must include comments.")]
        [StringLength(700, MinimumLength = 4)]
        public string contactComments { get; set; }
        public string gRecaptchaResponse { get; set; }
    }
}