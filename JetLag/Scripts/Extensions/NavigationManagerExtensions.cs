using JetLag.Scripts.Utility;
using Microsoft.AspNetCore.Components;


namespace JetLag.Scripts.Extensions;

/// <summary>
/// Extensions for the NavigationManager class.
/// </summary>
public static class NavigationManagerExtensions
{
    /// <summary>
    /// Navigates to the specified page.
    /// </summary>
    /// <typeparam name="T">The type of the page to navigate to.</typeparam>
    /// <param name="navigationManager">The navigation manager.</param>
    public static void NavigateTo<T>(this NavigationManager navigationManager)
        where T : ComponentBase
    {
        ErrorUtility.ThrowIfNotContainedInNamespace<T>("Pages");
        navigationManager.NavigateTo($"/{typeof(T).Name.ToLower()}");
    }
}