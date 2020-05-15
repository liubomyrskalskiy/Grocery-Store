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
    public class ProductionService : BaseService, IProductionService
    {
        public ProductionService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public List<Production> GetAll()
        {
            return unitOfWork.ProductionRepository.GetAll()
                .Include(production => production.IdEmployeeNavigation)
                .Include(production => production.IdGoodsOwnNavigation)
                .Include(production => production.IdGoodsOwnNavigation.IdCategoryNavigation)
                .ToList();
        }

        public Production GetId(int id)
        {
            var item = unitOfWork.ProductionRepository.GetById(id);
            if (item == null)
                throw new Exception("Such order not found");
            return item;
        }

        public Production Create(Production entity)
        {
            var valueToInsert = entity;
            unitOfWork.ProductionRepository.Add(valueToInsert);
            unitOfWork.SaveTablesChanges("dbo.Production");
            entity.Id = valueToInsert.Id;
            return entity;
        }

        public Production Update(Production entity)
        {
            var value = entity;
            unitOfWork.ProductionRepository.Update(value);
            return entity;
        }

        public void Delete(int id)
        {
            var entity = unitOfWork.ProductionRepository.GetById(id);
            unitOfWork.ProductionRepository.Delete(entity);
            unitOfWork.SaveChanges();
        }
    }
}