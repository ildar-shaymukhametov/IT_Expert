using System.ComponentModel.DataAnnotations;

public class Item
{
    public int Id { get; set; }
    public int Code { get; set; }

    [Required]
    [MaxLength(100)]
    public string? Value { get; set; }
}