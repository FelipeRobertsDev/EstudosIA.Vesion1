using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

namespace EstudoIA.Version1.Application.Data.UserContext.Abstractions;

public static class PasswordHasher
{
    private const int SaltSize = 16;
    private const int KeySize = 32;
    private const int Iterations = 10000;

    public static string Hash(string senha)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = KeyDerivation.Pbkdf2(
            senha,
            salt,
            KeyDerivationPrf.HMACSHA256,
            Iterations,
            KeySize);

        return $"{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
    }

    public static bool Verify(string hashComSalt, string senha)
    {
        var parts = hashComSalt.Split('.');
        if (parts.Length != 2)
            return false;

        var salt = Convert.FromBase64String(parts[0]);
        var hash = Convert.FromBase64String(parts[1]);

        var tentativa = KeyDerivation.Pbkdf2(
            senha,
            salt,
            KeyDerivationPrf.HMACSHA256,
            Iterations,
            KeySize);

        return hash.SequenceEqual(tentativa);
    }
}
