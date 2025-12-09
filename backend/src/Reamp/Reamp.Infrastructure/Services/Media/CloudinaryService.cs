using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Reamp.Domain.Media.Enums;
using Reamp.Infrastructure.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace Reamp.Infrastructure.Services.Media
{
    public sealed class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;
        private readonly CloudinarySettings _settings;
        private readonly MediaUploadSettings _uploadSettings;
        private readonly ILogger<CloudinaryService> _logger;

        public CloudinaryService(
            IOptions<CloudinarySettings> settings,
            IOptions<MediaUploadSettings> uploadSettings,
            ILogger<CloudinaryService> logger)
        {
            _settings = settings.Value;
            _uploadSettings = uploadSettings.Value;
            _logger = logger;
            
            var account = new Account(
                _settings.CloudName,
                _settings.ApiKey,
                _settings.ApiSecret
            );
            
            _cloudinary = new Cloudinary(account)
            {
                Api = { Secure = _settings.UseHttps }
            };
        }

        public async Task<bool> TestConnectionAsync()
        {
            try
            {
                var pingResult = await _cloudinary.PingAsync();
                return pingResult.StatusCode == System.Net.HttpStatusCode.OK;
            }
            catch
            {
                return false;
            }
        }

        public async Task<string> GetCloudInfoAsync()
        {
            try
            {
                var pingResult = await _cloudinary.PingAsync();
                
                if (pingResult.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    return $"❌ Failed to connect: {pingResult.Error?.Message ?? "Unknown error"}";
                }

                return $@"✅ Cloudinary Connected Successfully!

Cloud Name: {_settings.CloudName}
HTTPS: {_settings.UseHttps}
Folder: {_settings.Folder}

Status: API connection verified! 🚀";
            }
            catch (Exception ex)
            {
                return $"❌ Connection Error: {ex.Message}";
            }
        }

        public async Task<CloudinaryUploadResult> UploadImageAsync(
            Stream stream,
            string fileName,
            string? folder = null,
            CancellationToken ct = default)
        {
            try
            {
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(fileName, stream),
                    Folder = folder ?? _settings.Folder,
                    UseFilename = false,
                    UniqueFilename = true,
                    Overwrite = false
                };

                if (_uploadSettings.EnableAutoOptimization)
                {
                    uploadParams.Transformation = new Transformation()
                        .Quality("auto")
                        .FetchFormat("auto");
                }

                var uploadResult = await _cloudinary.UploadAsync(uploadParams, ct);

                if (uploadResult.Error != null)
                {
                    _logger.LogError("Image upload failed: {Error}", uploadResult.Error.Message);
                    return new CloudinaryUploadResult
                    {
                        Error = uploadResult.Error.Message
                    };
                }

                return new CloudinaryUploadResult
                {
                    PublicId = uploadResult.PublicId,
                    SecureUrl = uploadResult.SecureUrl.ToString(),
                    Format = uploadResult.Format,
                    Bytes = uploadResult.Bytes,
                    Width = uploadResult.Width,
                    Height = uploadResult.Height,
                    ResourceType = MediaResourceType.Image
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception during image upload");
                return new CloudinaryUploadResult
                {
                    Error = $"Upload failed: {ex.Message}"
                };
            }
        }

        public async Task<CloudinaryUploadResult> UploadVideoAsync(
            Stream stream,
            string fileName,
            string? folder = null,
            CancellationToken ct = default)
        {
            try
            {
                var uploadParams = new VideoUploadParams
                {
                    File = new FileDescription(fileName, stream),
                    Folder = folder ?? _settings.Folder,
                    UseFilename = false,
                    UniqueFilename = true,
                    Overwrite = false,
                    EagerAsync = _uploadSettings.GenerateThumbnails
                };

                if (_uploadSettings.EnableAutoOptimization)
                {
                    uploadParams.Transformation = new Transformation()
                        .Quality(_uploadSettings.VideoQuality);
                }

                if (_uploadSettings.GenerateThumbnails)
                {
                    uploadParams.EagerTransforms = new List<Transformation>
                    {
                        new Transformation()
                            .Width(_uploadSettings.ThumbnailWidth)
                            .Height(_uploadSettings.ThumbnailHeight)
                            .Crop("fill")
                            .Gravity("center")
                    };
                }

                var uploadResult = await _cloudinary.UploadAsync(uploadParams, ct);

                if (uploadResult.Error != null)
                {
                    _logger.LogError("Video upload failed: {Error}", uploadResult.Error.Message);
                    return new CloudinaryUploadResult
                    {
                        Error = uploadResult.Error.Message
                    };
                }

                return new CloudinaryUploadResult
                {
                    PublicId = uploadResult.PublicId,
                    SecureUrl = uploadResult.SecureUrl.ToString(),
                    Format = uploadResult.Format,
                    Bytes = uploadResult.Bytes,
                    Width = uploadResult.Width,
                    Height = uploadResult.Height,
                    Duration = uploadResult.Duration,
                    ResourceType = MediaResourceType.Video
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception during video upload");
                return new CloudinaryUploadResult
                {
                    Error = $"Upload failed: {ex.Message}"
                };
            }
        }

        public async Task<bool> DeleteAssetAsync(
            string publicId,
            MediaResourceType resourceType,
            CancellationToken ct = default)
        {
            try
            {
                DeletionParams deleteParams = resourceType == MediaResourceType.Image
                    ? new DeletionParams(publicId)
                    : new DeletionParams(publicId) { ResourceType = ResourceType.Video };

                var result = await _cloudinary.DestroyAsync(deleteParams);
                
                if (result.Result == "ok" || result.Result == "not found")
                {
                    _logger.LogInformation("Asset deleted: {PublicId}", publicId);
                    return true;
                }

                _logger.LogWarning("Failed to delete asset {PublicId}: {Result}", publicId, result.Result);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception deleting asset {PublicId}", publicId);
                return false;
            }
        }

        public string GenerateTransformationUrl(
            string publicId,
            int? width,
            int? height,
            MediaResourceType resourceType)
        {
            var transformation = new Transformation();

            if (width.HasValue)
                transformation = transformation.Width(width.Value);

            if (height.HasValue)
                transformation = transformation.Height(height.Value);

            if (width.HasValue || height.HasValue)
                transformation = transformation.Crop("fill").Gravity("center");

            var url = _cloudinary.Api.UrlImgUp.Transform(transformation)
                .Secure(_settings.UseHttps)
                .ResourceType(resourceType == MediaResourceType.Image ? "image" : "video")
                .BuildUrl(publicId);

            return url;
        }

        public string GenerateVideoThumbnailUrl(
            string publicId,
            int width = 640,
            int height = 360)
        {
            var transformation = new Transformation()
                .Width(width)
                .Height(height)
                .Crop("fill")
                .Gravity("center");

            // Build URL for video with .jpg extension for thumbnail
            var url = _cloudinary.Api.UrlImgUp.Transform(transformation)
                .Secure(_settings.UseHttps)
                .ResourceType("video")
                .BuildUrl(publicId + ".jpg");

            return url;
        }

        // Compute SHA256 checksum for file content
        public static async Task<string> ComputeSha256Async(Stream stream)
        {
            using var sha256 = SHA256.Create();
            var position = stream.Position;
            stream.Position = 0;
            
            var hash = await sha256.ComputeHashAsync(stream);
            stream.Position = position;
            
            return Convert.ToHexString(hash).ToLowerInvariant();
        }
    }
}

