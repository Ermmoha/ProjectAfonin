using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Data.SqlClient;

namespace ShoeStore;

public static class Db
{
    // Update in App.config (ObuvStoreDb) if you use a different SQL Server instance or database name.
    public static string ConnectionString { get; set; } = ResolveConnectionString();

    private static string ResolveConnectionString()
    {
        var baseDir = AppContext.BaseDirectory;
        var appConfigPath = Path.Combine(baseDir, "App.config");
        var fromConfig = TryReadConnectionString(appConfigPath, "ObuvStoreDb");
        if (!string.IsNullOrWhiteSpace(fromConfig))
        {
            return fromConfig!;
        }

        foreach (var path in Directory.EnumerateFiles(baseDir, "*.config"))
        {
            if (string.Equals(path, appConfigPath, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            fromConfig = TryReadConnectionString(path, "ObuvStoreDb");
            if (!string.IsNullOrWhiteSpace(fromConfig))
            {
                return fromConfig!;
            }
        }

        return "Data Source=_I_AM_PC_;Initial Catalog=ObuveStore;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";
    }

    private static string? TryReadConnectionString(string path, string name)
    {
        try
        {
            if (!File.Exists(path))
            {
                return null;
            }

            var doc = XDocument.Load(path);
            var add = doc.Root?
                .Element("connectionStrings")?
                .Elements("add")
                .FirstOrDefault(e => string.Equals((string?)e.Attribute("name"), name, StringComparison.OrdinalIgnoreCase));
            return (string?)add?.Attribute("connectionString");
        }
        catch
        {
            return null;
        }
    }

    public static DataTable GetDataTable(string sql, params SqlParameter[] parameters)
    {
        using var conn = new SqlConnection(ConnectionString);
        using var cmd = new SqlCommand(sql, conn);
        if (parameters is { Length: > 0 })
        {
            cmd.Parameters.AddRange(parameters);
        }

        using var adapter = new SqlDataAdapter(cmd);
        var table = new DataTable();
        adapter.Fill(table);
        return table;
    }

    public static int Execute(string sql, params SqlParameter[] parameters)
    {
        using var conn = new SqlConnection(ConnectionString);
        conn.Open();
        using var cmd = new SqlCommand(sql, conn);
        if (parameters is { Length: > 0 })
        {
            cmd.Parameters.AddRange(parameters);
        }
        return cmd.ExecuteNonQuery();
    }

    public static T? Scalar<T>(string sql, params SqlParameter[] parameters)
    {
        using var conn = new SqlConnection(ConnectionString);
        conn.Open();
        using var cmd = new SqlCommand(sql, conn);
        if (parameters is { Length: > 0 })
        {
            cmd.Parameters.AddRange(parameters);
        }
        var result = cmd.ExecuteScalar();
        if (result == null || result == DBNull.Value) return default;
        return (T)result;
    }
}
