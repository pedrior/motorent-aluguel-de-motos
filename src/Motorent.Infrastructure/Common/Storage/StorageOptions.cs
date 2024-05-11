using System.ComponentModel.DataAnnotations;

namespace Motorent.Infrastructure.Common.Storage;

public class StorageOptions
{
    public const string SectionName = "Storage";
    
    [Required]
    public string BucketName { get; init; } = string.Empty;
}