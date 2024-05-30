using System.ComponentModel.DataAnnotations;

namespace Motorent.Contracts.Common.Messages;

public sealed record MessageBusOptions
{
    public const string SectionName = "MessageBus";

    [Required]
    public string Host { get; init; } = null!;
    
    [Required]
    public string Username { get; init; } = null!;
    
    [Required]
    public string Password { get; init; } = null!;
}