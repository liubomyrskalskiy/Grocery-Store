using GroceryStore.Core.Abstractions.Repositories;
using GroceryStore.Core.Models;
using GroceryStore.DAL.Repositories.Base;

namespace GroceryStore.DAL.Repositories
{
    public class RoleRepository : BaseRepository<Role>, IRoleRepository
    {
        public RoleRepository(GroceryStoreDbContext context) : base(context)
        {
        }
    }
}