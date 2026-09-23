namespace LightSpeak.Backend.Dominio;

public sealed class User : Entity
{
    public string Email { get; private set; } = default!;
    public string PasswordHash { get; private set; } = default!;
    public string FirstName { get; private set; } = default!;
    public string LastName { get; private set; } = default!;
    public int FailedLoginAttempts { get; private set; }
    public DateTime? LockoutEnd { get; private set; }
    public string? ProfileImageUrl { get; private set; }

    public ICollection<UserRole> UserRoles { get; private set; } = new List<UserRole>();
    public ICollection<ServerMember> ServerMembers { get; private set; } = new List<ServerMember>();

    private User() { }

    public static User Create(
        string email,
        string passwordHash,
        string firstName,
        string lastName)
    {
        return new User
        {
            Email = email,
            PasswordHash = passwordHash,
            FirstName = firstName,
            LastName = lastName
        };
    }

    public void RegisterFailedLogin(int maxAttempts, TimeSpan lockoutDuration)
    {
        FailedLoginAttempts++;
        if (FailedLoginAttempts >= maxAttempts)
        {
            LockoutEnd = DateTime.UtcNow.Add(lockoutDuration);
            FailedLoginAttempts = 0;
        }
    }

    public void ResetLoginFailures()
    {
        FailedLoginAttempts = 0;
        LockoutEnd = null;
    }

    public bool IsLockedOut() => LockoutEnd is { } end && end > DateTime.UtcNow;

    public void SetProfileImage(string? url)
    {
        ProfileImageUrl = url;
        UpdatedAt = DateTime.UtcNow;
    }
}
