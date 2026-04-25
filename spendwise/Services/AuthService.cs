using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using spendwise.Data;
using spendwise.Models;

namespace spendwise.Services;

public interface IAuthService
{
    Task<User?> AuthenticateAsync(string email, string password, string userType);
    string HashPassword(string password);
    bool VerifyPassword(string password, string hash);
}

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private static readonly byte[] Salt = [123, 45, 67, 89, 101, 112, 123, 145, 156, 178, 189, 201, 212, 223, 234, 245];
    private const int Iterations = 1000;

    public AuthService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User?> AuthenticateAsync(string email, string password, string userType)
    {
        var user = await _context.Users.FirstOrDefaultAsync<User>(u => u.Email == email && u.UserType == userType);

        if (user == null)
            return null;

        if (!VerifyPassword(password, user.PasswordHash))
            return null;

        return user;
    }

#pragma warning disable SYSLIB0060
    public string HashPassword(string password)
    {
        using var hasher = new Rfc2898DeriveBytes(password, Salt, Iterations, HashAlgorithmName.SHA256);
        var hash = hasher.GetBytes(32);
        return Convert.ToBase64String(hash);
    }
#pragma warning restore SYSLIB0060

    public bool VerifyPassword(string password, string hash)
    {
        var hashOfInput = HashPassword(password);
        return hashOfInput == hash;
    }
}
