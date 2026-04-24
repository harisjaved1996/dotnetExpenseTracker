using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using spendwise.Data;
using spendwise.Models;

var connectionString = "Data Source=localhost\\SQLEXPRESS01;Initial Catalog=spendwise;Integrated Security=True;Persist Security Info=False;Pooling=False;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Command Timeout=0";

var options = new DbContextOptionsBuilder<AppDbContext>()
    .UseSqlServer(connectionString)
    .Options;

var islamicNames = new[]
{
    "Aisha", "Ahmad", "Amina", "Ali", "Ayesha", "Bilal", "Fatima", "Hasan",
    "Hafsa", "Ibrahim", "Iqra", "Jamila", "Karim", "Layla", "Mariam", "Muhammad",
    "Noor", "Nadia", "Omar", "Ombra", "Rashid", "Rania", "Samira", "Samir",
    "Tariq", "Tania", "Yasmin", "Yusuf", "Zahra", "Zainab"
};

if (islamicNames.Length < 10)
{
    Console.WriteLine($"Error: Not enough names available. Required: 10, Available: {islamicNames.Length}");
    Environment.Exit(1);
}

var random = new Random();
var selectedNames = islamicNames.OrderBy(_ => random.Next()).Take(10).ToList();

var salt = new byte[] { 123, 45, 67, 89, 101, 112, 123, 145, 156, 178, 189, 201, 212, 223, 234, 245 };
var password = "1234";

List<(string Name, string Email, string Hash)> usersToCreate = new();

foreach (var name in selectedNames)
{
    var email = $"{name.ToLower()}@gmail.com";

    using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 1000, HashAlgorithmName.SHA256))
    {
        var hash = Convert.ToBase64String(pbkdf2.GetBytes(32));
        usersToCreate.Add((name, email, hash));
    }
}

using (var context = new AppDbContext(options))
{
    var createdCount = 0;
    var failedCount = 0;

    foreach (var (name, email, hash) in usersToCreate)
    {
        try
        {
            var user = new User
            {
                Email = email,
                Name = name,
                PasswordHash = hash,
                UserType = "user",
                CreatedAt = DateTime.UtcNow
            };

            context.Users.Add(user);
            context.SaveChanges();
            Console.WriteLine($"✅ Created: {name} ({email})");
            createdCount++;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Failed: {name} ({email}) - {ex.Message}");
            failedCount++;
        }
    }

    Console.WriteLine($"\nSummary: {createdCount} created, {failedCount} failed");

    var recentUsers = context.Users
        .Where(u => u.UserType == "user")
        .OrderByDescending(u => u.Id)
        .Take(10)
        .Select(u => new { u.Id, u.Email, u.Name, u.UserType, u.CreatedAt })
        .ToList();

    Console.WriteLine("\n📋 Recent users:");
    foreach (var u in recentUsers)
    {
        Console.WriteLine($"ID: {u.Id} | Email: {u.Email} | Name: {u.Name} | Type: {u.UserType} | Created: {u.CreatedAt:yyyy-MM-dd HH:mm:ss}");
    }
}
