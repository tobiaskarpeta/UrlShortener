using System;
using System.ComponentModel.DataAnnotations;

namespace UrlShortener.Api.Models
{
    public class ShortenedUrl
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(2000)]
        public required string OriginalUrl { get; set; }
        
        [Required]
        [MaxLength(10)]
        public required string ShortCode { get; set; }
        
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        public DateTime? ExpiresAt { get; set; }
        
        public int AccessCount { get; set; } = 0;
        
        [MaxLength(255)]
        public string? CreatedBy { get; set; }
    }
}
