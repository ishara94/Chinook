using Chinook.Models;

namespace Chinook.Services
{
    public class SerachServices
    {
        // Make the Search generic inorder to use this in future
        public List<T> ApplyFilter<T>(List<T> items, Func<T, string> propSelector, string searchTerm)
        {
            return items?.Where(item =>
                string.IsNullOrWhiteSpace(searchTerm) ||
                propSelector(item).IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0
            ).OrderByDescending(item =>
                propSelector(item).IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) == 0
            ).ToList();
        }
    }
}
