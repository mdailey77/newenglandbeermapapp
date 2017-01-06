using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;


namespace NewEnglandBeerMapApplication.Models
{
    public class CraftBreweries
    {
        public virtual int Id {get; set;}
        [Required]
        [Display(Name = "Brewery Name")]
        public virtual string BreweryName { get; set; }
        [Required]
        [Display(Name = "Brewery Address")]
        public virtual string BreweryAddress { get; set; }
        [Required]
        [Display(Name = "Brewery City")]
        public virtual string BreweryCity { get; set; }
        [Required]
        [Display(Name = "Brewery State")]
        public virtual string BreweryState { get; set; }
        [Required]
        [Display(Name = "Brewery ZipCode")]
        public virtual string BreweryZipCode { get; set; }
        [DisplayFormat(DataFormatString = "{0:0.00000000}", ApplyFormatInEditMode = true)]
        public virtual decimal latitude { get; set; }
        [DisplayFormat(DataFormatString = "{0:0.00000000}", ApplyFormatInEditMode = true)]
        public virtual decimal longitude { get; set; }
        [Required]
        [AllowHtml]
        [Display(Name = "Brewery Website")]
        public virtual string BreweryWebsite { get; set; }

    }
}