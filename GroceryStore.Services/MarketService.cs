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
    public class MarketService : BaseService, IMarketService
    {
        public MarketService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public List<Market> GetAll()
        {
            return unitOfWork.MarketRepository.GetAll()
                .Include(market => market.IdCityNavigation)
                .ToList();
        }

        public Market GetId(int id)
        {
            var item = unitOfWork.MarketRepository.GetById(id);
            if (item == null)
                throw new Exception("Such order not found");
            return item;
        }

        public Market Create(Market entity)
        {
            var valueToInsert = entity;
            unitOfWork.MarketRepository.Add(valueToInsert);
            unitOfWork.SaveTablesChanges("dbo.Market");
            entity.Id = valueToInsert.Id;
            return entity;
        }

        public Market Update(Market entity)
        {
            var value = entity;
            unitOfWork.MarketRepository.Update(value);
            return entity;
        }

        public void Delete(int id)
        {
            var entity = unitOfWork.MarketRepository.GetById(id);
            unitOfWork.MarketRepository.Delete(entity);
            unitOfWork.SaveChanges();
        }
    }
}