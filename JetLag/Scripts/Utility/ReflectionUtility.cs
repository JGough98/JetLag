using System.Reflection;
using System.Text.Json.Serialization;


namespace JetLag.Scripts.Utility.Reflection;

public static class ReflectionUtility
{
    public static string GetEnumJsonName<T>(T value)
        where T : struct, Enum
    {
        var type = typeof(T)!;

        var name = Enum.GetName(type, value);

        if (name == null)
            return string.Empty;

        var field = type.GetField(name);

        var attribute = field?.GetCustomAttribute<JsonStringEnumMemberNameAttribute>();

        return attribute?.Name ?? name;
    }
}
