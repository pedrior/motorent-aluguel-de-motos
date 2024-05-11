using Motorent.Application.Common.Abstractions.Requests;
using Motorent.Application.Common.Abstractions.Storage;

namespace Motorent.Application.Renters.UploadCNHValidationImages;

public sealed record UploadCNHValidationImagesCommand : ICommand, ITransactional
{
    public required IFile FrontImage { get; init; }
    
    public required IFile BackImage { get; init; }
}