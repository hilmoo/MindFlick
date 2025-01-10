namespace flashcard.utils;

public static class AuthenticationConstants
{
    public const string CookieName = "auth";
}

public static class PostgresConstants
{
    public static readonly string ConnectionString;

    static PostgresConstants()
    {
        var host = GetEnvironmentVariableOrThrow("POSTGRES_HOST");
        var database = GetEnvironmentVariableOrThrow("POSTGRES_DB_NAME");
        var username = GetEnvironmentVariableOrThrow("POSTGRES_USER");
        var password = GetEnvironmentVariableOrThrow("POSTGRES_PASSWORD");

        ConnectionString =
            $"Host={host};Username={username};Password={password};Database={database}";
    }

    private static string GetEnvironmentVariableOrThrow(string variableName)
    {
        return Environment.GetEnvironmentVariable(variableName)
               ?? throw new InvalidOperationException($"{variableName} environment variable is not set.");
    }
}

public static class CustomRandom
{
    public static string GenerateRandomString(int length)
    {
        var random = new Random();
        const string chars = "abcdefghijklmnopqrstuvwxyz";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)])
            .ToArray());
    }
}