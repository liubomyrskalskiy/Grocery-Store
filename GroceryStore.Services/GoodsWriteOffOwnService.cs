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
    public class GoodsWriteOffOwnService : BaseService, IGoodsWriteOffOwnService
    {
        public GoodsWriteOffOwnService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public List<GoodsWriteOffOwn> GetAll()
        {
            return unitOfWork.GoodsWriteOffOwnRepository.GetAll().Include(goodsown => goodsown.IdEmployeeNavigation)
                .Include(goodsown => goodsown.IdGoodsInMarketOwnNavigation.IdMarketNavigation)
                .Include(goodsown => goodsown.IdProductionNavigation.IdGoodsOwnNavigation)
                .Include(goodsown => goodsown.IdWriteOffReasonNavigation).ToList();
        }

        public GoodsWriteOffOwn GetId(int id)
        {
            var item = unitOfWork.GoodsWriteOffOwnRepository.GetById(id);
            if (item == null)
                throw new Exception("Such order not found");
            return item;
        }

        public GoodsWriteOffOwn Create(GoodsWriteOffOwn entity)
        {
            var valueToInsert = entity;
            unitOfWork.GoodsWriteOffOwnRepository.Add(valueToInsert);
            unitOfWork.SaveTablesChanges("dbo.Goods_Write_off_Own");
            entity.Id = valueToInsert.Id;
            return entity;
        }

        public GoodsWriteOffOwn Update(GoodsWriteOffOwn entity)
        {
            var value = entity;
            unitOfWork.GoodsWriteOffOwnRepository.Update(value);
            return entity;
        }

        public void Delete(int id)
        {
            var entity = unitOfWork.GoodsWriteOffOwnRepository.GetById(id);
            unitOfWork.GoodsWriteOffOwnRepository.Delete(entity);
            unitOfWork.SaveChanges();
        }
    }
}