namespace LightSpeak.Backend.Common.Entities;

public sealed class Role : Entity
{
    public string Name { get; private set; } = default!;

    public ICollection<UserRole> UserRoles { get; private set; } = new List<UserRole>();

    private Role() { }

    public static Role Create(string name)
    {
        return new Role { Name = name };
    }
}
