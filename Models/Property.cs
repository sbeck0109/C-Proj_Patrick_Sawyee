using System;
// using System.Web.DynamicData;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CsharpProject.Models {
    public class Property {
        [Key]
        public int PropertyId { get; set; }

        [Required]
        [Display (Name = "Listing Type")]
        public String ListingType { get; set; }

        [Required]
        [MinLength (5, ErrorMessage = "must be at least 5 characters")]
        [Display (Name = "Title")]
        public string Title { get; set; }

        [Required]
        [MinLength (5, ErrorMessage = "must be at least 5 characters")]
        [Display (Name = "Address")]
        public string Address { get; set; }

        [Required]
        [MinLength (5, ErrorMessage = "must be at least 5 characters")]
        [Display (Name = "Description")]
        public string Description { get; set; }

        [Required]
        [Display (Name = "Bedrooms")]
        public int Bedrooms { get; set; }

        [Required]
        [Display (Name = "Baths")]
        public int Baths { get; set; }

        [Required]
        [Display (Name = "Square Feet")]
        public int SqFt { get; set; }

        [Required]
        [Display (Name = "Lot Size")]
        public int LotSize { get; set; }

        [Required]
        [Range (1500, 2020, ErrorMessage = "Year Built Must be in the Past.")]

        [Display (Name = "Year Built")]
        public int Year { get; set; }

        [Required]
        [Display (Name = "Price")]
        public int Price { get; set; }

        // [Required]
        // [Display (Name = "Photos")]
        // [DataType (DataType.Upload)]
        // [AllowedExtensions(new string[] { ".jpg", ".png" })]
        // public IFormFile Photo { get; set; }
        // public HttpPostedFileBase ImageUpload { get; set; }

        public string ImgUrl { get; set; }

        public int UserId { get; set; }

        // Navigation Props - MUST USE .include to access:

        public User Creator { get; set; }

        public List<Association> Associations { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}