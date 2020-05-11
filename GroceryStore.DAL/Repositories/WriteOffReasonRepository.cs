using GroceryStore.Core.Abstractions.Repositories;
using GroceryStore.Core.Models;
using GroceryStore.DAL.Repositories.Base;

namespace GroceryStore.DAL.Repositories
{
    public class WriteOffReasonRepository : BaseRepository<WriteOffReason>, IWriteOffReasonRepository
    {
        public WriteOffReasonRepository(GroceryStoreDbContext context) : base(context)
        {
        }
    }
}