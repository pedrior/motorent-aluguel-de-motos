namespace Motorent.Application.Common.Abstractions.Storage;

public interface IStorageService
{
    Task UploadAsync(Uri path, IFile file, CancellationToken cancellationToken = default);
    
    Task DeleteAsync(Uri path, CancellationToken cancellationToken = default);
    
    Task<Uri> GenerateUrlAsync(Uri path, int expirationInMinutes = 120);
}