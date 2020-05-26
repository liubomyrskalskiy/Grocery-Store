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
    public class GoodsInMarketOwnService : BaseService, IGoodsInMarketOwnService
    {
        public GoodsInMarketOwnService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public List<GoodsInMarketOwn> GetAll()
        {
            return unitOfWork.GoodsInMarketOwnRepository.GetAll()
                .Include(gimo => gimo.IdProductionNavigation)
                .Include(gimo => gimo.IdMarketNavigation)
                .Include(gimo => gimo.IdMarketNavigation.IdCityNavigation)
                .Include(gimo => gimo.IdProductionNavigation.IdGoodsOwnNavigation)
                .Include(gimo => gimo.IdProductionNavigation.IdGoodsOwnNavigation.IdCategoryNavigation)
                .ToList();
        }

        public GoodsInMarketOwn GetId(int id)
        {
            var item = unitOfWork.GoodsInMarketOwnRepository.GetById(id);
            if (item == null)
                throw new Exception("Such order not found");
            return item;
        }

        public GoodsInMarketOwn Create(GoodsInMarketOwn entity)
        {
            var valueToInsert = entity;
            unitOfWork.GoodsInMarketOwnRepository.Add(valueToInsert);
            unitOfWork.SaveTablesChanges("dbo.Goods_in_Market_Own");
            entity.Id = valueToInsert.Id;
            return entity;
        }

        public GoodsInMarketOwn Update(GoodsInMarketOwn entity)
        {
            var value = entity;
            unitOfWork.GoodsInMarketOwnRepository.Update(value);
            return entity;
        }

        public void Delete(int id)
        {
            var entity = unitOfWork.GoodsInMarketOwnRepository.GetById(id);
            unitOfWork.GoodsInMarketOwnRepository.Delete(entity);
            unitOfWork.SaveChanges();
        }

        public void Refresh(GoodsInMarketOwn entity)
        {
            unitOfWork.GoodsInMarketOwnRepository.RefreshEntity(entity);
            unitOfWork.SaveChanges();
        }
    }
}