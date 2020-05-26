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
    public class GoodsInMarketService : BaseService, IGoodsInMarketService
    {
        public GoodsInMarketService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public List<GoodsInMarket> GetAll()
        {
            return unitOfWork.GoodsInMarketRepository.GetAll()
                .Include(gim => gim.IdGoodsNavigation)
                .Include(gim => gim.IdGoodsNavigation.IdCategoryNavigation)
                .Include(gim => gim.IdMarketNavigation)
                .Include(gim => gim.IdGoodsNavigation.IdProducerNavigation).ToList();
        }

        public GoodsInMarket GetId(int id)
        {
            var item = unitOfWork.GoodsInMarketRepository.GetById(id);
            if (item == null)
                throw new Exception("Such order not found");
            return item;
        }

        public GoodsInMarket Create(GoodsInMarket entity)
        {
            var valueToInsert = entity;
            unitOfWork.GoodsInMarketRepository.Add(valueToInsert);
            unitOfWork.SaveTablesChanges("dbo.Goods_in_Market");
            entity.Id = valueToInsert.Id;
            return entity;
        }

        public GoodsInMarket Update(GoodsInMarket entity)
        {
            var value = entity;
            unitOfWork.GoodsInMarketRepository.Update(value);
            return entity;
        }

        public void Delete(int id)
        {
            var entity = unitOfWork.GoodsInMarketRepository.GetById(id);
            unitOfWork.GoodsInMarketRepository.Delete(entity);
            unitOfWork.SaveChanges();
        }

        public void Refresh(GoodsInMarket entity)
        {
            unitOfWork.GoodsInMarketRepository.RefreshEntity(entity);
            unitOfWork.SaveChanges();
        }
    }
}