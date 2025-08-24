using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reamp.Domain.Common.ValueObjects
{
    public sealed class Slug : IEquatable<Slug>
    {
        public string Value { get; }

        private Slug(string value) => Value = value;

        public static Slug From(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                throw new ArgumentException("Slug source cannot be empty.");

            var s = input.Trim().ToLowerInvariant();

            var chars = s.ToCharArray();
            for (int i = 0; i < chars.Length; i++)
                chars[i] = char.IsLetterOrDigit(chars[i]) ? chars[i] : ' ';
            s = new string(chars);

            var parts = s.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var slug = string.Join('-', parts);

            if (slug.Length == 0)
                throw new ArgumentException("Slug cannot be empty after normalization.");

            return new Slug(slug);
        }

        public override string ToString() => Value;
        public override int GetHashCode() => Value.GetHashCode(StringComparison.Ordinal);
        public override bool Equals(object? obj) => obj is Slug other && Equals(other);
        public bool Equals(Slug? other) => other is not null && string.Equals(Value, other.Value, StringComparison.Ordinal);

        public static implicit operator string(Slug s) => s.Value;
    }
}