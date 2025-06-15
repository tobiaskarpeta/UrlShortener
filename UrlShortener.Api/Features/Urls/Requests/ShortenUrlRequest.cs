using System.ComponentModel.DataAnnotations;

public class ShortenUrlRequest
{
    [Required]
    public required string Url { get; set; }

    [MinLength(7)]  
    public string? CustomCode { get; set; }
}