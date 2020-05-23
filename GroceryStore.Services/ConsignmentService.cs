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
    public class ConsignmentService : BaseService, IConsignmentService
    {
        public ConsignmentService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public List<Consignment> GetAll()
        {
            return unitOfWork.ConsignmentRepository.GetAll()
                .Include(goods => goods.IdGoodsNavigation)
                .Include(goods => goods.IdGoodsNavigation.IdCategoryNavigation)
                .Include(goods => goods.IdGoodsNavigation.IdProducerNavigation)
                .ToList();
        }

        public Consignment GetId(int id)
        {
            var item = unitOfWork.ConsignmentRepository.GetById(id);
            if (item == null)
                throw new Exception("Such order not found");
            return item;
        }

        public Consignment Create(Consignment entity)
        {
            var valueToInsert = entity;
            unitOfWork.ConsignmentRepository.Add(valueToInsert);
            unitOfWork.SaveTablesChanges("dbo.Consignment");
            entity.Id = valueToInsert.Id;
            return entity;
        }

        public Consignment Update(Consignment entity)
        {
            var value = entity;
            unitOfWork.ConsignmentRepository.Update(value);
            return entity;
        }

        public void Delete(int id)
        {
            var entity = unitOfWork.ConsignmentRepository.GetById(id);
            unitOfWork.ConsignmentRepository.Delete(entity);
            unitOfWork.SaveChanges();
        }
    }
}