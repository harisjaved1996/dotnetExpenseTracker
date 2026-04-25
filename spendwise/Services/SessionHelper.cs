using System.Text.Json;
using spendwise.Models;

namespace spendwise.Services;

public static class SessionHelper
{
    private const string UserSessionKey = "AuthUser";
    private const string RoleSessionKey = "AuthRole";

    public static void SetUser(ISession session, User user, string role)
    {
        var json = JsonSerializer.Serialize(user);
        session.SetString(UserSessionKey, json);
        session.SetString(RoleSessionKey, role);
    }

    public static User? GetUser(ISession session)
    {
        var json = session.GetString(UserSessionKey);
        if (string.IsNullOrEmpty(json))
            return null;

        return JsonSerializer.Deserialize<User>(json);
    }

    public static string? GetRole(ISession session)
    {
        return session.GetString(RoleSessionKey);
    }

    public static bool IsAuthenticated(ISession session)
    {
        return session.GetString(UserSessionKey) != null;
    }

    public static void ClearSession(ISession session)
    {
        session.Remove(UserSessionKey);
        session.Remove(RoleSessionKey);
    }
}
