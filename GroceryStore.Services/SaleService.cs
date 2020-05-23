using System;
using System.Collections.Generic;
using System.Linq;
using GroceryStore.Core.Abstractions;
using GroceryStore.Core.Abstractions.IServices;
using GroceryStore.Core.Models;
using GroceryStore.Services.Base;
using Microsoft.EntityFrameworkCore;

namespace GroceryStore.Services
{
    public class SaleService : BaseService, ISaleService
    {
        public SaleService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public List<Sale> GetAll()
        {
            unitOfWork.SaveChanges();
            return unitOfWork.SaleRepository.GetAll()
                .Include(s => s.IdEmployeeNavigation)
                .Include(s => s.IdEmployeeNavigation.IdMarketNavigation)
                .Include(s => s.IdClientNavigation)
                .ToList();
        }

        public Sale GetId(int id)
        {
            var item = unitOfWork.SaleRepository.GetById(id);
            if (item == null)
                throw new Exception("Such order not found");
            return item;
        }

        public Sale Create(Sale entity)
        {
            var valueToInsert = entity;
            unitOfWork.SaleRepository.Add(valueToInsert);
            unitOfWork.SaveTablesChanges("dbo.Sale");
            entity.Id = valueToInsert.Id;
            return entity;
        }

        public Sale Update(Sale entity)
        {
            var value = entity;
            unitOfWork.SaleRepository.Update(value);
            return entity;
        }

        public void Delete(int id)
        {
            var entity = unitOfWork.SaleRepository.GetById(id);
            unitOfWork.SaleRepository.Delete(entity);
            unitOfWork.SaveChanges();
        }

        public void Refresh(Sale entity)
        {
            unitOfWork.SaleRepository.RefreshEntity(entity);
            unitOfWork.SaveChanges();
        }
    }
}