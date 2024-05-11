namespace Motorent.Application.Common.Abstractions.Storage;

public interface IFile
{
    string Name { get; }

    string Extension { get; }

    string ContentType { get; }

    Stream Stream { get; }
}