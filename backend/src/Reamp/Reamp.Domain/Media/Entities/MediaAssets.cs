using Reamp.Domain.Media.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Reamp.Domain.Media.Entities
{
    public sealed class MediaAsset
    {
        public Guid Id { get; private set; }

        public Guid OwnerStudioId { get; private set; }
        public Guid UploaderUserId { get; private set; }

        // Hosting provider & resource type
        public MediaProvider Provider { get; private set; }
        public string ProviderAssetId { get; private set; } = default!; // Cloudinary public_id / Stream videoId
        public MediaResourceType ResourceType { get; private set; }

        // Metadata
        public MediaProcessStatus ProcessStatus { get; private set; } = MediaProcessStatus.Uploaded;
        public string ContentType { get; private set; } = default!;
        public long SizeBytes { get; private set; }
        public int? WidthPx { get; private set; }
        public int? HeightPx { get; private set; }
        public decimal? DurationSeconds { get; private set; }
        public string? OriginalFileName { get; private set; }
        public string? PublicUrl { get; private set; }

        // Exact deduplication
        public string? ChecksumSha256 { get; private set; } // 64-char lowercase hex

        public DateTime CreatedAtUtc { get; private set; } = DateTime.UtcNow;
        public DateTime UpdatedAtUtc { get; private set; } = DateTime.UtcNow;

        // Variants
        private readonly List<MediaVariant> _variants = new();
        public IReadOnlyCollection<MediaVariant> Variants => _variants.AsReadOnly();

        private MediaAsset() { } // EF

        public MediaAsset(
            Guid ownerStudioId,
            Guid uploaderUserId,
            MediaProvider provider,
            string providerAssetId,
            MediaResourceType resourceType,
            string contentType,
            long sizeBytes,
            string? originalFileName = null,
            string? publicUrl = null,
            string? checksumSha256 = null)
        {
            if (ownerStudioId == Guid.Empty) throw new ArgumentException("OwnerStudioId is required.", nameof(ownerStudioId));
            if (uploaderUserId == Guid.Empty) throw new ArgumentException("UploaderUserId is required.", nameof(uploaderUserId));
            if (string.IsNullOrWhiteSpace(providerAssetId)) throw new ArgumentException("ProviderAssetId is required.", nameof(providerAssetId));
            if (string.IsNullOrWhiteSpace(contentType)) throw new ArgumentException("ContentType is required.", nameof(contentType));
            if (sizeBytes <= 0) throw new ArgumentOutOfRangeException(nameof(sizeBytes), "SizeBytes must be greater than 0.");

            EnsureCompatibility(provider, resourceType);

            Id = Guid.NewGuid();
            OwnerStudioId = ownerStudioId;
            UploaderUserId = uploaderUserId;
            Provider = provider;
            ProviderAssetId = providerAssetId.Trim();
            ResourceType = resourceType;
            ContentType = contentType.Trim();
            SizeBytes = sizeBytes;
            OriginalFileName = string.IsNullOrWhiteSpace(originalFileName) ? null : originalFileName.Trim();
            PublicUrl = string.IsNullOrWhiteSpace(publicUrl) ? null : publicUrl.Trim();

            if (!string.IsNullOrWhiteSpace(checksumSha256))
                SetChecksumSha256(checksumSha256);
        }

        // ---- State transitions ----
        public void StartProcessing() { ProcessStatus = MediaProcessStatus.Processing; Touch(); }
        public void MarkReady() { ProcessStatus = MediaProcessStatus.Ready; Touch(); }
        public void MarkFailed() { ProcessStatus = MediaProcessStatus.Failed; Touch(); }

        // ---- Metadata updates ----
        public void UpdateBasicMeta(int? width, int? height, decimal? durationSec)
        {
            WidthPx = width;
            HeightPx = height;
            DurationSeconds = durationSec;
            Touch();
        }

        public void UpdatePublicUrl(string? url)
        {
            PublicUrl = string.IsNullOrWhiteSpace(url) ? null : url.Trim();
            Touch();
        }

        // ---- Deduplication checksum (64-hex lowercase) ----
        public void SetChecksumSha256(string hex)
        {
            if (string.IsNullOrWhiteSpace(hex) || !Regex.IsMatch(hex, "^[0-9a-fA-F]{64}$"))
                throw new ArgumentException("Checksum must be a valid 64-character hex string.", nameof(hex));

            ChecksumSha256 = hex.ToLowerInvariant();
            Touch();
        }

        public bool Matches(long sizeBytes, string? checksumHex)
            => sizeBytes == SizeBytes
               && !string.IsNullOrWhiteSpace(ChecksumSha256)
               && !string.IsNullOrWhiteSpace(checksumHex)
               && string.Equals(ChecksumSha256, checksumHex?.Trim().ToLowerInvariant(), StringComparison.Ordinal);

        // ---- Variants (idempotent by Name) ----
        public void AddOrReplaceVariant(
            string name, string url, string? format = null,
            int? width = null, int? height = null, int sortOrder = 0,
            int? bitrateKbps = null, long? sizeBytes = null)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Variant name is required.", nameof(name));
            if (string.IsNullOrWhiteSpace(url)) throw new ArgumentException("Variant url is required.", nameof(url));
            if (sizeBytes is <= 0) throw new ArgumentOutOfRangeException(nameof(sizeBytes), "Variant size must be greater than 0.");

            var existing = _variants.FirstOrDefault(v => v.Name == name);
            if (existing is null)
            {
                _variants.Add(new MediaVariant(
                    assetId: Id, name: name.Trim(), url: url.Trim(),
                    format: string.IsNullOrWhiteSpace(format) ? null : format.Trim(),
                    width: width, height: height, sortOrder: sortOrder,
                    bitrateKbps: bitrateKbps, sizeBytes: sizeBytes));
            }
            else
            {
                existing.Update(url, format, width, height, sortOrder, bitrateKbps, sizeBytes);
            }
            Touch();
        }

        public string? TryGetVariantUrl(params string[] preferredNames)
            => preferredNames.Select(n => _variants.FirstOrDefault(v => v.Name == n)?.Url)
                             .FirstOrDefault(u => !string.IsNullOrWhiteSpace(u));

        private static void EnsureCompatibility(MediaProvider provider, MediaResourceType type)
        {
            if (provider == MediaProvider.CloudflareStream && type != MediaResourceType.Video)
                throw new InvalidOperationException("Cloudflare Stream can only be used for Video assets.");

            // Cloudinary can handle both images and videos
        }

        private void Touch() => UpdatedAtUtc = DateTime.UtcNow;
    }
}