using System.Security.Cryptography;

namespace TatarDelivery.UserService.Security;

public sealed class PasswordHasher
{
    private const int SaltSize = 16;
    private const int KeySize = 32;
    private const int Iterations = 100_000;

    public void HashPassword(string password, out string passwordHash, out string passwordSalt)
    {
        var saltBytes = RandomNumberGenerator.GetBytes(SaltSize);
        var hashBytes = Rfc2898DeriveBytes.Pbkdf2(password, saltBytes, Iterations, HashAlgorithmName.SHA256, KeySize);

        passwordSalt = Convert.ToBase64String(saltBytes);
        passwordHash = Convert.ToBase64String(hashBytes);
    }

    public bool VerifyPassword(string password, string passwordHash, string passwordSalt)
    {
        var saltBytes = Convert.FromBase64String(passwordSalt);
        var hashBytes = Convert.FromBase64String(passwordHash);
        var attemptedHash = Rfc2898DeriveBytes.Pbkdf2(password, saltBytes, Iterations, HashAlgorithmName.SHA256, KeySize);

        return CryptographicOperations.FixedTimeEquals(hashBytes, attemptedHash);
    }
}
