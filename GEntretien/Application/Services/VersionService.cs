using System.Reflection;

namespace GEntretien.Application.Services;

/// <summary>
/// Service for retrieving application version information.
/// </summary>
public interface IVersionService
{
    /// <summary>
    /// Gets the application version number.
    /// </summary>
    string GetVersion();
}

/// <summary>
/// Implementation of version service that reads from assembly attributes.
/// </summary>
public class VersionService : IVersionService
{
    public string GetVersion()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var informationalVersion = assembly
            .GetCustomAttribute<System.Reflection.AssemblyInformationalVersionAttribute>()
            ?.InformationalVersion ?? "Unknown";
        
        return informationalVersion;
    }
}
