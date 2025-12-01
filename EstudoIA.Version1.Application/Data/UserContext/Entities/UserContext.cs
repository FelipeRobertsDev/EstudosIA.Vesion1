using EstudoIA.Version1.Application.Data.UserContext.Abstractions;
namespace EstudoIA.Version1.Application.Data.UserContext.Entities;

public class UserContext
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;


    public void SetPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentException("Password cannot be null or empty.", nameof(password));
        }

        PasswordHash = PasswordHasher.Hash(password);
    }

    public bool VerifyPassword(string password)
    {
        return PasswordHasher.Verify(PasswordHash, password);
    }

}
