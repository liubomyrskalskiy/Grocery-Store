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
    public class CityService : BaseService, ICityService
    {
        public CityService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public List<City> GetAll()
        {
            return unitOfWork.CityRepository.GetAll().Include(city => city.IdCountryNavigation).ToList();
        }

        public City GetId(int id)
        {
            var item = unitOfWork.CityRepository.GetById(id);
            if (item == null)
                throw new Exception("Such order not found");
            return item;
        }

        public City Create(City entity)
        {
            var valueToInsert = entity;
            unitOfWork.CityRepository.Add(valueToInsert);
            unitOfWork.SaveTablesChanges("dbo.City");
            entity.Id = valueToInsert.Id;
            return entity;
        }

        public City Update(City entity)
        {
            var value = entity;
            unitOfWork.CityRepository.Update(value);
            return entity;
        }

        public void Delete(int id)
        {
            var entity = unitOfWork.CityRepository.GetById(id);
            unitOfWork.CityRepository.Delete(entity);
            unitOfWork.SaveChanges();
        }
    }
}