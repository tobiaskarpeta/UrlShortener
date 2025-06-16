using System.ComponentModel.DataAnnotations;

public class CreateShortenUrlRequest
{
    [Required]
    public required string Url { get; set; }

    [MaxLength(10)]
    [MinLength(7)]  
    public string? CustomCode { get; set; }
}