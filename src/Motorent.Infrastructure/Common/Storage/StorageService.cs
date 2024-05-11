using System.Net;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using Microsoft.Extensions.Options;
using Motorent.Application.Common.Abstractions.Storage;

namespace Motorent.Infrastructure.Common.Storage;

internal sealed class StorageService(IAmazonS3 s3, IOptions<StorageOptions> options) : IStorageService
{
    private const int MaxExpirationInMinutes = 12 * 60;

    private readonly StorageOptions options = options.Value;

    public async Task UploadAsync(Uri path, IFile file, CancellationToken cancellationToken = default)
    {
        await EnsureBucketCreatedAsync(cancellationToken);

        var request = new PutObjectRequest
        {
            Key = path.ToString(),
            BucketName = options.BucketName,
            InputStream = file.Stream,
            ContentType = file.ContentType,
            Metadata =
            {
                ["x-amz-meta-name"] = file.Name,
                ["x-amx-meta-extension"] = file.Extension
            }
        };

        var response = await s3.PutObjectAsync(request, cancellationToken);
        if (response.HttpStatusCode is not HttpStatusCode.OK)
        {
            throw new StorageException("Failed to upload file to S3 Bucket", response.HttpStatusCode);
        }
    }

    public async Task DeleteAsync(Uri path, CancellationToken cancellationToken = default)
    {
        if (!await IsBucketCreatedAsync())
        {
            return;
        }

        var request = new DeleteObjectRequest
        {
            Key = path.ToString(),
            BucketName = options.BucketName
        };

        var response = await s3.DeleteObjectAsync(request, cancellationToken);
        if (response.HttpStatusCode is not HttpStatusCode.NoContent)
        {
            throw new StorageException("Failed to delete file from S3 Bucket", response.HttpStatusCode);
        }
    }

    public async Task<Uri> GenerateUrlAsync(Uri path, int expirationInMinutes = 120)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(expirationInMinutes, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(expirationInMinutes, MaxExpirationInMinutes);

        var request = new GetPreSignedUrlRequest
        {
            BucketName = options.BucketName,
            Key = path.ToString(),
            Expires = DateTime.UtcNow.AddMinutes(expirationInMinutes)
        };

        return new Uri(await s3.GetPreSignedURLAsync(request));
    }

    private async Task EnsureBucketCreatedAsync(CancellationToken cancellationToken)
    {
        if (!await IsBucketCreatedAsync())
        {
            await s3.PutBucketAsync(options.BucketName, cancellationToken);
        }
    }

    private Task<bool> IsBucketCreatedAsync() => AmazonS3Util.DoesS3BucketExistV2Async(s3, options.BucketName);
}