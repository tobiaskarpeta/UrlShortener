using System.ComponentModel.DataAnnotations;

public class UpdateShortenedUrlRequest
{
    [Required]
    [MaxLength(2000)]
    public required string OriginalUrl { get; set; }

    [Required]
    [MaxLength(10)]
    [MinLength(7)]
    public required string ShortCode { get; set; }
    
    public DateTime? ExpiresAt { get; set; }
    
}