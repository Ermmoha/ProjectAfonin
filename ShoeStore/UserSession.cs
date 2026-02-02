namespace ShoeStore;

public sealed class UserSession
{
    public int? UserId { get; init; }
    public string FullName { get; init; } = string.Empty;
    public string RoleName { get; init; } = string.Empty;

    public bool IsGuest => RoleName.Equals("Guest", StringComparison.OrdinalIgnoreCase);
    public bool IsAdmin => RoleName.Equals("Администратор", StringComparison.OrdinalIgnoreCase);
    public bool IsManager => RoleName.Equals("Менеджер", StringComparison.OrdinalIgnoreCase);
    public bool IsClient => RoleName.Equals("Авторизированный клиент", StringComparison.OrdinalIgnoreCase);

    public static UserSession Guest() => new() { RoleName = "Guest", FullName = "Гость" };
}
