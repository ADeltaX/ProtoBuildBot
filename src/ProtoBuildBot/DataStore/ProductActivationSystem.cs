using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace ProtoBuildBot.DataStore
{
    public static class ProductActivationSystem
    {
        private static RNGCryptoServiceProvider rnd = new RNGCryptoServiceProvider();
        private const string MasterKey = SecretKeys.ProductMasterKey;
        private const string MasterBase = SecretKeys.ProductMasterBase;

        public static bool VerifyKey(string key)
        {
            var cv = key.Replace("-", "", StringComparison.Ordinal).ToUpperInvariant();
            if (cv.Length != 25)
                return false;

            if (!cv.StartsWith(MasterBase, StringComparison.Ordinal))
                return false;

            var hashSection = cv.Substring(2, 13);
            var originalText = cv.Substring(0, 2) + cv.Substring(15);

            var hash = new StringBuilder();

            using (var sha512Hash = SHA512.Create())
            {
                var hashBytes = sha512Hash.ComputeHash(Encoding.UTF8.GetBytes(originalText + MasterKey));

                foreach (var t in hashBytes)
                    hash.Append(t.ToString("x2", CultureInfo.InvariantCulture));
            }

            //Now the comparison
            return hash.ToString().Substring(10, 13).ToUpperInvariant() == hashSection;
        }

        public static string GenerateNewKey()
        {
            string random = "";
            for (int i = 0; i < 10; i++)
                random += GetNumberOrLetter();

            var hash = new StringBuilder();

            using (var sha512Hash = SHA512.Create())
            {
                var hashBytes = sha512Hash.ComputeHash(Encoding.UTF8.GetBytes(MasterBase + random + MasterKey));

                foreach (var t in hashBytes)
                    hash.Append(t.ToString("x2", CultureInfo.InvariantCulture));
            }

            return (MasterBase + hash.ToString().Substring(10, 13).ToUpperInvariant() + random).Insert(5, "-").Insert(11, "-").Insert(17, "-").Insert(23, "-");
        }

        private static int RandomInteger(int min, int max)
        {
            uint scale = uint.MaxValue;

            while (scale == uint.MaxValue)
            {
                byte[] four_bytes = new byte[4];
                rnd.GetBytes(four_bytes);
                scale = BitConverter.ToUInt32(four_bytes, 0);
            }

            return (int)(min + (max - min) *
                (scale / (double)uint.MaxValue));
        }

        public static char GetNumberOrLetter()
        {
            int letterOrNumber = RandomInteger(0, 2);
            if (letterOrNumber == 1)
            {
                int num = RandomInteger(0, 26);
                char let = (char)('A' + num);
                return let;
            }
            else
            {
                return RandomInteger(0, 10).ToString(CultureInfo.InvariantCulture)[0];
            }
        }
    }
}
