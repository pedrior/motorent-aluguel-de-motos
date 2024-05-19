using Motorent.Application.Common.Abstractions.Requests;
using Motorent.Application.Common.Abstractions.Storage;

namespace Motorent.Application.Renters.UploadDriverLicenseImage;

public sealed record UploadDriverLicenseImageCommand : ICommand, ITransactional
{
    public required IFile Image { get; init; }
}