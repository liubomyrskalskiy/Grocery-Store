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
    public class DeliveryShipmentService : BaseService, IDeliveryShipmentService
    {
        public DeliveryShipmentService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public List<DeliveryShipment> GetAll()
        {
            return unitOfWork.DeliveryShipmentRepository.GetAll().Include(shipment => shipment.IdConsignmentNavigation)
                .Include(shipment => shipment.IdGoodsInMarketNavigation.IdGoodsNavigation).ToList();
        }

        public DeliveryShipment GetId(int id)
        {
            var item = unitOfWork.DeliveryShipmentRepository.GetById(id);
            if (item == null)
                throw new Exception("Such order not found");
            return item;
        }

        public DeliveryShipment Create(DeliveryShipment entity)
        {
            var valueToInsert = entity;
            unitOfWork.DeliveryShipmentRepository.Add(valueToInsert);
            unitOfWork.SaveTablesChanges("dbo.Delivery_Shipment");
            entity.Id = valueToInsert.Id;
            return entity;
        }

        public DeliveryShipment Update(DeliveryShipment entity)
        {
            var value = entity;
            unitOfWork.DeliveryShipmentRepository.Update(value);
            return entity;
        }

        public void Delete(int id)
        {
            var entity = unitOfWork.DeliveryShipmentRepository.GetById(id);
            unitOfWork.DeliveryShipmentRepository.Delete(entity);
            unitOfWork.SaveChanges();
        }

        public void Refresh(DeliveryShipment entity)
        {
            unitOfWork.DeliveryShipmentRepository.RefreshEntity(entity);
            unitOfWork.SaveChanges();
        }
    }
}