using System.ComponentModel.DataAnnotations;

public class ItemDto
{
    [Required]
    public int? Code { get; set; }

    [Required]
    public string? Value { get; set; }
}