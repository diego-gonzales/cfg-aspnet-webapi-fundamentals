using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace WebAPIAutoresSeguridad;

public class HashService
{
    public HashResultDTO Hash(string password)
    {
        var salt = new byte[16];
        using (var random = RandomNumberGenerator.Create())
        {
            random.GetBytes(salt);
        }

        return Hash(password, salt);
    }

    private HashResultDTO Hash(string password, byte[] salt)
    {
        var derivedKey = KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA1,
            iterationCount: 1000,
            numBytesRequested: 32
        );

        var hash = Convert.ToBase64String(derivedKey);

        return new HashResultDTO() { Hash = hash, Salt = salt };
    }
}
