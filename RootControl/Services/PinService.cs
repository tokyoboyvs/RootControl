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
}
