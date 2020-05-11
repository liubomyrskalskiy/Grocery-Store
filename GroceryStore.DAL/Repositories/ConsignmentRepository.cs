using GroceryStore.Core.Abstractions.Repositories;
using GroceryStore.Core.Models;
using GroceryStore.DAL.Repositories.Base;

namespace GroceryStore.DAL.Repositories
{
    public class ConsignmentRepository : BaseRepository<Consignment>, IConsignmentRepository
    {
        public ConsignmentRepository(GroceryStoreDbContext context) : base(context)
        {
        }
    }
}