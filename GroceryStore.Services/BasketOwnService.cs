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
    public class BasketOwnService : BaseService, IBasketOwnService
    {
        public BasketOwnService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public List<BasketOwn> GetAll()
        {
            return unitOfWork.BasketOwnRepository.GetAll()
                .Include(basketOwn => basketOwn.IdSaleNavigation)
                .Include(basketOwn => basketOwn.IdGoodsInMarketOwnNavigation)
                .Include(basketOwn => basketOwn.IdGoodsInMarketOwnNavigation.IdProductionNavigation)
                .Include(
                    basketOwn => basketOwn.IdGoodsInMarketOwnNavigation.IdProductionNavigation.IdGoodsOwnNavigation)
                .ToList();
        }

        public BasketOwn GetId(int id)
        {
            var item = unitOfWork.BasketOwnRepository.GetById(id);
            if (item == null)
                throw new Exception("Such order not found");
            return item;
        }

        public BasketOwn Create(BasketOwn entity)
        {
            var valueToInsert = entity;
            unitOfWork.BasketOwnRepository.Add(valueToInsert);
            unitOfWork.SaveTablesChanges("dbo.Basket_Own");
            entity.Id = valueToInsert.Id;
            return entity;
        }

        public BasketOwn Update(BasketOwn entity)
        {
            var value = entity;
            unitOfWork.BasketOwnRepository.Update(value);
            return entity;
        }

        public void Delete(int id)
        {
            var entity = unitOfWork.BasketOwnRepository.GetById(id);
            unitOfWork.BasketOwnRepository.Delete(entity);
            unitOfWork.SaveChanges();
        }
    }
}