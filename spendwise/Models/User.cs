namespace spendwise.Models;

public class User
{
    public int Id { get; set; }
    public required string Email { get; set; }
    public required string Name { get; set; }
    public required string PasswordHash { get; set; }
    public required string UserType { get; set; }
    public DateTime CreatedAt { get; set; }
}
