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
    public class BasketService : BaseService, IBasketService
    {
        public BasketService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public List<Basket> GetAll()
        {
            return unitOfWork.BasketRepository.GetAll()
                .Include(basket => basket.IdSaleNavigation)
                .Include(basket => basket.IdGoodsInMarketNavigation.IdGoodsNavigation)
                .Include(basket => basket.IdGoodsInMarketNavigation.IdGoodsNavigation.IdProducerNavigation)
                .ToList();
        }

        public Basket GetId(int id)
        {
            var item = unitOfWork.BasketRepository.GetById(id);
            if (item == null)
                throw new Exception("Such order not found");
            return item;
        }

        public Basket Create(Basket entity)
        {
            var valueToInsert = entity;
            unitOfWork.BasketRepository.Add(valueToInsert);
            unitOfWork.SaveTablesChanges("dbo.Basket");
            entity.Id = valueToInsert.Id;
            return entity;
        }

        public Basket Update(Basket entity)
        {
            var value = entity;
            unitOfWork.BasketRepository.Update(value);
            return entity;
        }

        public void Delete(int id)
        {
            var entity = unitOfWork.BasketRepository.GetById(id);
            unitOfWork.BasketRepository.Delete(entity);
            unitOfWork.SaveChanges();
        }
    }
}