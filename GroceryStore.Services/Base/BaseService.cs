using GroceryStore.Core.Abstractions;

namespace GroceryStore.Services.Base
{
    public class BaseService
    {
        protected IUnitOfWork unitOfWork;

        public BaseService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
    }
}