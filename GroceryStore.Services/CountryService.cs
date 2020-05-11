using System;
using System.Collections.Generic;
using System.Linq;
using GroceryStore.Core.Abstractions;
using GroceryStore.Core.Abstractions.IServices;
using GroceryStore.Core.Models;
using GroceryStore.Services.Base;

namespace GroceryStore.Services
{
    public class CountryService : BaseService, ICountryService
    {
        public CountryService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public List<Country> GetAll()
        {
            return unitOfWork.CountryRepository.GetAll().ToList();
        }

        public Country GetId(int id)
        {
            var item = unitOfWork.CountryRepository.GetById(id);
            if (item == null)
                throw new Exception("Such order not found");
            return item;
        }

        public Country Create(Country entity)
        {
            var valueToInsert = entity;
            unitOfWork.CountryRepository.Add(valueToInsert);
            unitOfWork.SaveTablesChanges("dbo.Country");
            entity.Id = valueToInsert.Id;
            return entity;
        }

        public Country Update(Country entity)
        {
            var value = entity;
            unitOfWork.CountryRepository.Update(value);
            return entity;
        }

        public void Delete(int id)
        {
            var entity = unitOfWork.CountryRepository.GetById(id);
            unitOfWork.CountryRepository.Delete(entity);
            unitOfWork.SaveChanges();
        }
    }
}