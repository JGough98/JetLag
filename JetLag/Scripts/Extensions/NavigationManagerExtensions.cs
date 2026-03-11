using Microsoft.AspNetCore.Components;


public static class NavigationManagerExtensions
{
    public static void NavigateTo<T>(this NavigationManager navigationManager)
        => navigationManager.NavigateTo($"/{typeof(T).Name.ToLower()}");
}