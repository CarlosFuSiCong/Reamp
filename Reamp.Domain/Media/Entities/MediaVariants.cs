using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reamp.Domain.Media.Entities
{
    public sealed class MediaVariant
    {
        public Guid Id { get; private set; }
        public Guid MediaAssetId { get; private set; }

        // Logical key under one asset (e.g. thumb_480 / web_1920 / poster / hls)
        public string Name { get; private set; } = default!;
        public string Url { get; private set; } = default!;
        public string? Format { get; private set; }          // jpg/webp/mp4/m3u8/…
        public int? WidthPx { get; private set; }
        public int? HeightPx { get; private set; }
        public int? BitrateKbps { get; private set; }        // for video variants
        public long? SizeBytes { get; private set; }       // nullable; if provided, must be > 0
        public int SortOrder { get; private set; }
        public DateTime CreatedAtUtc { get; private set; } = DateTime.UtcNow;

        private MediaVariant() { } // EF

        public MediaVariant(
            Guid assetId,
            string name,
            string url,
            string? format,
            int? width,
            int? height,
            int sortOrder,
            int? bitrateKbps,
            long? sizeBytes)
        {
            if (assetId == Guid.Empty) throw new ArgumentException("MediaAssetId is required.", nameof(assetId));
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Variant name is required.", nameof(name));
            if (string.IsNullOrWhiteSpace(url)) throw new ArgumentException("Variant url is required.", nameof(url));
            if (sizeBytes is <= 0) throw new ArgumentOutOfRangeException(nameof(sizeBytes), "Variant size must be greater than 0 when specified.");

            Id = Guid.NewGuid();
            MediaAssetId = assetId;
            Name = name.Trim();
            Url = url.Trim();
            Format = string.IsNullOrWhiteSpace(format) ? null : format.Trim();
            WidthPx = width;
            HeightPx = height;
            SortOrder = sortOrder;
            BitrateKbps = bitrateKbps;
            SizeBytes = sizeBytes;
        }

        // Idempotent update used by MediaAsset.AddOrReplaceVariant(...)
        public void Update(
            string url,
            string? format,
            int? width,
            int? height,
            int sortOrder,
            int? bitrateKbps,
            long? sizeBytes)
        {
            if (string.IsNullOrWhiteSpace(url)) throw new ArgumentException("Variant url is required.", nameof(url));
            if (sizeBytes is <= 0) throw new ArgumentOutOfRangeException(nameof(sizeBytes), "Variant size must be greater than 0 when specified.");

            Url = url.Trim();
            Format = string.IsNullOrWhiteSpace(format) ? null : format.Trim();
            WidthPx = width;
            HeightPx = height;
            SortOrder = sortOrder;
            BitrateKbps = bitrateKbps;
            SizeBytes = sizeBytes;
        }
    }
}
