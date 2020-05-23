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
    public class ProviderService : BaseService, IProviderService
    {
        public ProviderService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public List<Provider> GetAll()
        {
            return unitOfWork.ProviderRepository.GetAll()
                .Include(provider => provider.IdCityNavigation)
                .Include(provider => provider.IdCityNavigation.IdCountryNavigation)
                .ToList();
        }

        public Provider GetId(int id)
        {
            var item = unitOfWork.ProviderRepository.GetById(id);
            if (item == null)
                throw new Exception("Such order not found");
            return item;
        }

        public Provider Create(Provider entity)
        {
            var valueToInsert = entity;
            unitOfWork.ProviderRepository.Add(valueToInsert);
            unitOfWork.SaveTablesChanges("dbo.Provider");
            entity.Id = valueToInsert.Id;
            return entity;
        }

        public Provider Update(Provider entity)
        {
            var value = entity;
            unitOfWork.ProviderRepository.Update(value);
            return entity;
        }

        public void Delete(int id)
        {
            var entity = unitOfWork.ProviderRepository.GetById(id);
            unitOfWork.ProviderRepository.Delete(entity);
            unitOfWork.SaveChanges();
        }
    }
}