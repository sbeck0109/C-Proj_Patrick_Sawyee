using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CsharpProject.Models
{
    public class Association
    {
        [Key]
        public int AssociationId { get; set; }

        public int UserId { get; set; }
        public int PropertyId { get; set; }
        
        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public DateTime UpdatedAt { get; set; }
		
		// Navigation Props - MUST USE .include to access:
		public User User { get; set; }
        public Property Property { get; set; }
    }
}

