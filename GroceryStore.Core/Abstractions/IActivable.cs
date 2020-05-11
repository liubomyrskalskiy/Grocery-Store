using System.Threading.Tasks;

namespace GroceryStore.Core.Abstractions
{
    public interface IActivable
    {
        Task ActivateAsync(object parameter);
    }
}