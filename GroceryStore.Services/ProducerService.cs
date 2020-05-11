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
    public class ProducerService : BaseService, IProducerService
    {
        public ProducerService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public List<Producer> GetAll()
        {
            return unitOfWork.ProducerRepository.GetAll().Include(producer => producer.IdCountryNavigation).ToList();
        }

        public Producer GetId(int id)
        {
            var item = unitOfWork.ProducerRepository.GetById(id);
            if (item == null)
                throw new Exception("Such order not found");
            return item;
        }

        public Producer Create(Producer entity)
        {
            var valueToInsert = entity;
            unitOfWork.ProducerRepository.Add(valueToInsert);
            unitOfWork.SaveTablesChanges("dbo.Producer");
            entity.Id = valueToInsert.Id;
            return entity;
        }

        public Producer Update(Producer entity)
        {
            var value = entity;
            unitOfWork.ProducerRepository.Update(value);
            return entity;
        }

        public void Delete(int id)
        {
            var entity = unitOfWork.ProducerRepository.GetById(id);
            unitOfWork.ProducerRepository.Delete(entity);
            unitOfWork.SaveChanges();
        }
    }
}