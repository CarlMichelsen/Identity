using System.Security.Cryptography;
using System.Text;

namespace Application.Service.OAuth.Login;

public static class RefreshTokenHasher
{
    private const int SaltSize = 32;        // 256 bits
    private const int KeySize = 64;         // 512 bits
    private const int Iterations = 500_000; // Current safe baseline for 2025+

    public static string Hash(string refreshToken)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);

        var hash = Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(refreshToken),
            salt,
            Iterations,
            HashAlgorithmName.SHA256,
            KeySize);

        return $"{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
    }

    public static bool Verify(string refreshToken, string storedHash)
    {
        var parts = storedHash.Split('.');
        var iterations = int.Parse(parts[0]);
        var salt = Convert.FromBase64String(parts[1]);
        var expectedHash = Convert.FromBase64String(parts[2]);

        var actualHash = Rfc2898DeriveBytes.Pbkdf2(
            refreshToken,
            salt,
            iterations,
            HashAlgorithmName.SHA256,
            expectedHash.Length);

        return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
    }
}