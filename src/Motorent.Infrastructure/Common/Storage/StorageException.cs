using System.Net;

namespace Motorent.Infrastructure.Common.Storage;

internal sealed class StorageException(string message, HttpStatusCode statusCode) : Exception(message)
{
    public HttpStatusCode StatusCode { get; } = statusCode;
}