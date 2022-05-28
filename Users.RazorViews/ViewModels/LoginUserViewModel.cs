using System;

namespace Users.RazorViews.ViewModels
{
    public record LoginUserViewModel(string Name, string Ip, string? Continent, string? Region, string? City, 
        long? Latitude, long? Longitude, DateTimeOffset LoginDateTime);
}
