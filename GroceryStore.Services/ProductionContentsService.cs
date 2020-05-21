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
    public class ProductionContentsService : BaseService, IProductionContentsService
    {
        public ProductionContentsService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public List<ProductionContents> GetAll()
        {
            return unitOfWork.ProductionContentsRepository.GetAll()
                .Include(item => item.IdProductionNavigation)
                .Include(item => item.IdGoodsInMarketNavigation)
                .Include(item => item.IdGoodsInMarketNavigation.IdGoodsNavigation)
                .Include(item => item.IdGoodsInMarketNavigation.IdGoodsNavigation.IdCategoryNavigation)
                .Include(item => item.IdGoodsInMarketNavigation.IdGoodsNavigation.IdProducerNavigation)
                .ToList();
        }

        public ProductionContents GetId(int id)
        {
            var item = unitOfWork.ProductionContentsRepository.GetById(id);
            if (item == null)
                throw new Exception("Such order not found");
            return item;
        }

        public ProductionContents Create(ProductionContents entity)
        {
            var valueToInsert = entity;
            unitOfWork.ProductionContentsRepository.Add(valueToInsert);
            unitOfWork.SaveTablesChanges("dbo.Production_Contents");
            entity.Id = valueToInsert.Id;
            return entity;
        }

        public ProductionContents Update(ProductionContents entity)
        {
            var value = entity;
            unitOfWork.ProductionContentsRepository.Update(value);
            return entity;
        }

        public void Delete(int id)
        {
            var entity = unitOfWork.ProductionContentsRepository.GetById(id);
            unitOfWork.ProductionContentsRepository.Delete(entity);
            unitOfWork.SaveChanges();
        }
    }
}