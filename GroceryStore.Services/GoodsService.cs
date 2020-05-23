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
    public class GoodsService : BaseService, IGoodsService
    {
        public GoodsService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public List<Goods> GetAll()
        {
            return unitOfWork.GoodsRepository.GetAll().Include(category => category.IdCategoryNavigation)
                .Include(producer => producer.IdProducerNavigation).ToList();
        }

        public Goods GetId(int id)
        {
            var item = unitOfWork.GoodsRepository.GetById(id);
            if (item == null)
                throw new Exception("Such order not found");
            return item;
        }

        public Goods Create(Goods entity)
        {
            var valueToInsert = entity;
            unitOfWork.GoodsRepository.Add(valueToInsert);
            unitOfWork.SaveTablesChanges("dbo.Goods");
            entity.Id = valueToInsert.Id;
            return entity;
        }

        public Goods Update(Goods entity)
        {
            var value = entity;
            unitOfWork.GoodsRepository.Update(value);
            return entity;
        }

        public void Delete(int id)
        {
            var entity = unitOfWork.GoodsRepository.GetById(id);
            unitOfWork.GoodsRepository.Delete(entity);
            unitOfWork.SaveChanges();
        }
    }
}