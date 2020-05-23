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
    public class ClientService : BaseService, IClientService
    {
        public ClientService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public List<Client> GetAll()
        {
            return unitOfWork.ClientRepository.GetAll()
                .Include(city => city.IdCityNavigation)
                .Include(city => city.IdCityNavigation.IdCountryNavigation)
                .ToList();
        }

        public Client GetId(int id)
        {
            var item = unitOfWork.ClientRepository.GetById(id);
            if (item == null)
                throw new Exception("Such order not found");
            return item;
        }

        public Client Create(Client entity)
        {
            var valueToInsert = entity;
            unitOfWork.ClientRepository.Add(valueToInsert);
            unitOfWork.SaveTablesChanges("dbo.Client");
            entity.Id = valueToInsert.Id;
            return entity;
        }

        public Client Update(Client entity)
        {
            var value = entity;
            unitOfWork.ClientRepository.Update(value);
            return entity;
        }

        public void Delete(int id)
        {
            var entity = unitOfWork.ClientRepository.GetById(id);
            unitOfWork.ClientRepository.Delete(entity);
            unitOfWork.SaveChanges();
        }
    }
}