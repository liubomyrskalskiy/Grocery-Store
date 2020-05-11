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
    public class DeliveryService : BaseService, IDeliveryService
    {
        public DeliveryService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public List<Delivery> GetAll()
        {
            return unitOfWork.DeliveryRepository.GetAll().Include(delivery => delivery.IdProviderNavigation).ToList();
        }

        public Delivery GetId(int id)
        {
            var item = unitOfWork.DeliveryRepository.GetById(id);
            if (item == null)
                throw new Exception("Such order not found");
            return item;
        }

        public Delivery Create(Delivery entity)
        {
            var valueToInsert = entity;
            unitOfWork.DeliveryRepository.Add(valueToInsert);
            unitOfWork.SaveTablesChanges("dbo.Delivery");
            entity.Id = valueToInsert.Id;
            return entity;
        }

        public Delivery Update(Delivery entity)
        {
            var value = entity;
            unitOfWork.DeliveryRepository.Update(value);
            return entity;
        }

        public void Delete(int id)
        {
            var entity = unitOfWork.DeliveryRepository.GetById(id);
            unitOfWork.DeliveryRepository.Delete(entity);
            unitOfWork.SaveChanges();
        }
    }
}