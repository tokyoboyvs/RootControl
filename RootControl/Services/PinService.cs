using System;
using System.Security.Cryptography;
using System.Text;

namespace RootControl.Services;

public sealed class PinService
{
    public string HashPin(string pin)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(pin);
        byte[] hash = SHA256.HashData(bytes);
        return Convert.ToHexString(hash);
    }

    public bool VerifyPin(string pin, string expectedHash)
    {
        if (string.IsNullOrWhiteSpace(pin) || string.IsNullOrWhiteSpace(expectedHash))
        {
            return false;
        }

        string actualHash = HashPin(pin);
        return string.Equals(actualHash, expectedHash, StringComparison.OrdinalIgnoreCase);
    }
}
