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
    public class DeliveryContentsService : BaseService, IDeliveryContentsService
    {
        public DeliveryContentsService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public List<DeliveryContents> GetAll()
        {
            return unitOfWork.DeliveryContentsRepository.GetAll()
                .Include(content => content.IdConsignmentNavigation)
                .Include(content => content.IdConsignmentNavigation.IdGoodsNavigation)
                .Include(content => content.IdConsignmentNavigation.IdGoodsNavigation.IdProducerNavigation)
                .Include(content => content.IdDeliveryNavigation)
                .Include(content => content.IdDeliveryNavigation.IdProviderNavigation)
                .ToList();
        }

        public DeliveryContents GetId(int id)
        {
            var item = unitOfWork.DeliveryContentsRepository.GetById(id);
            if (item == null)
                throw new Exception("Such order not found");
            return item;
        }

        public DeliveryContents Create(DeliveryContents entity)
        {
            var valueToInsert = entity;
            unitOfWork.DeliveryContentsRepository.Add(valueToInsert);
            unitOfWork.SaveTablesChanges("dbo.Delivery_Contents");
            entity.Id = valueToInsert.Id;
            return entity;
        }

        public DeliveryContents Update(DeliveryContents entity)
        {
            var value = entity;
            unitOfWork.DeliveryContentsRepository.Update(value);
            return entity;
        }

        public void Delete(int id)
        {
            var entity = unitOfWork.DeliveryContentsRepository.GetById(id);
            unitOfWork.DeliveryContentsRepository.Delete(entity);
            unitOfWork.SaveChanges();
        }
    }
}