using Motorent.Application.Common.Abstractions.Requests;
using Motorent.Application.Common.Abstractions.Storage;

namespace Motorent.Application.Renters.UploadCNHImage;

public sealed record UploadCNHImageCommand : ICommand, ITransactional
{
    public required IFile Image { get; init; }
}