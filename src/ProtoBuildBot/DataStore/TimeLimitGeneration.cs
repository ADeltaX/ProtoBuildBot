using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace ProtoBuildBot.DataStore
{
    public static class TimeLimitGeneration
    {
        private const string MasterKey = SecretKeys.TimeLimitMasterKey;
        private const string Separator = "$";

        public static string GenerateNewHashTime(TimeSpan expiresAt)
        {
            var epoch = ((DateTime.Now.ToUniversalTime() + expiresAt) - DateTime.UnixEpoch).TotalMilliseconds.ToString("#", CultureInfo.InvariantCulture);

            var hash = new StringBuilder();

            using (var sha1Hash = SHA256.Create())
            {
                var hashBytes = sha1Hash.ComputeHash(Encoding.UTF8.GetBytes(epoch + MasterKey));

                foreach (var t in hashBytes)
                    hash.Append(t.ToString("x2", CultureInfo.InvariantCulture));
            }

            return hash.ToString().ToLowerInvariant() + Separator + epoch;
        }

        public static TimeValidationResult ValidateHashTime(string hash)
        {
            if (hash == null)
                return TimeValidationResult.Invalid;

            if (!hash.Contains(Separator, StringComparison.Ordinal))
                return TimeValidationResult.Invalid;

            var hashSplitted = hash.Split(Separator[0]);
            var hashPart = hashSplitted[0];
            var timePart = hashSplitted[1];

            if (!long.TryParse(timePart, out var epoch))
                return TimeValidationResult.Invalid;

            var resHash = new StringBuilder();

            using (var sha1Hash = SHA256.Create())
            {
                var hashBytes = sha1Hash.ComputeHash(Encoding.UTF8.GetBytes(timePart + MasterKey));

                foreach (var t in hashBytes)
                    resHash.Append(t.ToString("x2", CultureInfo.InvariantCulture));
            }

            if (resHash.ToString().Equals(hashPart, StringComparison.OrdinalIgnoreCase))
                return TimeValidationResult.Invalid;

            var currentEpoch = long.Parse((DateTime.UtcNow - DateTime.UnixEpoch).TotalMilliseconds.ToString("#", CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);

            if (currentEpoch > epoch)
                return TimeValidationResult.Expired;

            return TimeValidationResult.Valid;
        }

        public enum TimeValidationResult
        {
            Invalid,
            Valid,
            Expired
        }
    }
}
