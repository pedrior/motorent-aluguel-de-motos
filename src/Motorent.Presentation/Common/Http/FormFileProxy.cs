using Motorent.Application.Common.Abstractions.Storage;

namespace Motorent.Presentation.Common.Http;

internal sealed class FormFileProxy(IFormFile file) : IFile
{
    public string Name => file.Name;
    
    public string Extension => Path.GetExtension(file.FileName);
    
    public string ContentType => file.ContentType;
    
    public Stream Stream => file.OpenReadStream();
}