using Microsoft.Data.SqlClient;

namespace ShoeStore;

public static class AuthService
{
    public static UserSession? Authenticate(string login, string password)
    {
        if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
        {
            return null;
        }

        const string sql = @"
SELECT u.UserId, u.FullName, r.Name AS RoleName
FROM dbo.Users u
INNER JOIN dbo.Roles r ON r.RoleId = u.RoleId
WHERE u.Login = @login AND u.Password = @password;";

        using var conn = new SqlConnection(Db.ConnectionString);
        using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@login", login.Trim());
        cmd.Parameters.AddWithValue("@password", password.Trim());
        conn.Open();

        using var reader = cmd.ExecuteReader();
        if (!reader.Read()) return null;

        return new UserSession
        {
            UserId = reader.GetInt32(0),
            FullName = reader.GetString(1),
            RoleName = reader.GetString(2)
        };
    }
}
