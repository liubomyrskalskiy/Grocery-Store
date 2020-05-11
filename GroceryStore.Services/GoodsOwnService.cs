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
    public class GoodsOwnService : BaseService, IGoodsOwnService
    {
        public GoodsOwnService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public List<GoodsOwn> GetAll()
        {
            return unitOfWork.GoodsOwnRepository.GetAll().Include(goodsOwn => goodsOwn.IdCategoryNavigation).ToList();
        }

        public GoodsOwn GetId(int id)
        {
            var item = unitOfWork.GoodsOwnRepository.GetById(id);
            if (item == null)
                throw new Exception("Such order not found");
            return item;
        }

        public GoodsOwn Create(GoodsOwn entity)
        {
            var valueToInsert = entity;
            unitOfWork.GoodsOwnRepository.Add(valueToInsert);
            unitOfWork.SaveTablesChanges("dbo.Goods_Own");
            entity.Id = valueToInsert.Id;
            return entity;
        }

        public GoodsOwn Update(GoodsOwn entity)
        {
            var value = entity;
            unitOfWork.GoodsOwnRepository.Update(value);
            return entity;
        }

        public void Delete(int id)
        {
            var entity = unitOfWork.GoodsOwnRepository.GetById(id);
            unitOfWork.GoodsOwnRepository.Delete(entity);
            unitOfWork.SaveChanges();
        }
    }
}