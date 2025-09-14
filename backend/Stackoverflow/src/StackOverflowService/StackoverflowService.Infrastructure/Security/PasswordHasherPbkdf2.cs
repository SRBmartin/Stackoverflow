using StackoverflowService.Application.Abstractions;
using System.Security.Cryptography;
using System;

namespace StackoverflowService.Infrastructure.Security
{
    public class PasswordHasherPbkdf2 : IPasswordHasher
    {
        private const int DefaultIterations = 200_000;
        private const int SaltSize = 16;
        private const int KeySize = 32;

        private readonly int _iterations;
        public PasswordHasherPbkdf2(int? iterations = null) => _iterations = iterations ?? DefaultIterations;

        public string Hash(string password)
        {
            if (password == null) throw new ArgumentNullException(nameof(password));

            var salt = new byte[SaltSize];
            using (var rng = RandomNumberGenerator.Create())
                rng.GetBytes(salt);

            byte[] key;
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, _iterations))
            {
                key = pbkdf2.GetBytes(KeySize);
            }

            var saltB64 = Convert.ToBase64String(salt);
            var keyB64 = Convert.ToBase64String(key);

            return $"PBKDF2-SHA1|v1|iter={_iterations}|salt={saltB64}|hash={keyB64}";
        }

        public bool Verify(string password, string passwordHash)
        {
            if (password == null || string.IsNullOrWhiteSpace(passwordHash)) return false;

            if (!TryParse(passwordHash, out var iter, out var salt, out var expected))
                return false;

            byte[] actual;
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iter))
                actual = pbkdf2.GetBytes(expected.Length);

            return FixedTimeEquals(actual, expected);
        }

        public bool NeedsRehash(string passwordHash)
        {
            if (!TryParse(passwordHash, out var iter, out _, out var expected)) return true;
            
            return iter < _iterations || expected.Length != KeySize || !passwordHash.StartsWith("PBKDF2-SHA1|v1|", StringComparison.Ordinal);
        }

        private static bool TryParse(string formatted, out int iterations, out byte[] salt, out byte[] hash)
        {
            iterations = 0; salt = Array.Empty<byte>(); hash = Array.Empty<byte>();
            //Expected format: PBKDF2-SHA1|v1|iter=N|salt=B64|hash=B64
            var parts = formatted.Split('|');
            if (parts.Length != 5) return false;
            if (!parts[0].Equals("PBKDF2-SHA1", StringComparison.Ordinal)) return false;
            if (!parts[1].Equals("v1", StringComparison.Ordinal)) return false;

            var iterPart = parts[2];
            var saltPart = parts[3];
            var hashPart = parts[4];

            if (!iterPart.StartsWith("iter=", StringComparison.Ordinal)) return false;
            if (!saltPart.StartsWith("salt=", StringComparison.Ordinal)) return false;
            if (!hashPart.StartsWith("hash=", StringComparison.Ordinal)) return false;

            if (!int.TryParse(iterPart.Substring(5), out iterations) || iterations <= 0) return false;

            try
            {
                salt = Convert.FromBase64String(saltPart.Substring(5));
                hash = Convert.FromBase64String(hashPart.Substring(5));
                return salt.Length > 0 && hash.Length > 0;
            }
            catch { return false; }
        }

        private static bool FixedTimeEquals(byte[] a, byte[] b)
        {
            uint diff = (uint)(a?.Length ?? 0) ^ (uint)(b?.Length ?? 0);
            int len = Math.Min(a?.Length ?? 0, b?.Length ?? 0);
            for (int i = 0; i < len; i++) diff |= (uint)(a[i] ^ b[i]);
            return diff == 0;
        }

    }
}
