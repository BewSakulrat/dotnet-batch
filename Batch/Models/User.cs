using System.ComponentModel.DataAnnotations;

namespace Batch.Data;

public class User
{
    [Key]
    public long Id { get; set; }

    public string? username { get; set; } = string.Empty;
}