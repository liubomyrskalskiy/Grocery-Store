using System;
using System.Collections.Generic;
using System.Linq;
using GroceryStore.Core.Abstractions;
using GroceryStore.Core.Abstractions.IServices;
using GroceryStore.Core.Models;
using GroceryStore.Services.Base;

namespace GroceryStore.Services
{
    public class WriteOffReasonService : BaseService, IWriteOffReasonService
    {
        public WriteOffReasonService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public List<WriteOffReason> GetAll()
        {
            return unitOfWork.WriteOffReasonRepository.GetAll().ToList();
        }

        public WriteOffReason GetId(int id)
        {
            var item = unitOfWork.WriteOffReasonRepository.GetById(id);
            if (item == null)
                throw new Exception("Such order not found");
            return item;
        }

        public WriteOffReason Create(WriteOffReason entity)
        {
            var valueToInsert = entity;
            unitOfWork.WriteOffReasonRepository.Add(valueToInsert);
            unitOfWork.SaveTablesChanges("dbo.Write_off_Reason");
            entity.Id = valueToInsert.Id;
            return entity;
        }

        public WriteOffReason Update(WriteOffReason entity)
        {
            var value = entity;
            unitOfWork.WriteOffReasonRepository.Update(value);
            return entity;
        }

        public void Delete(int id)
        {
            var entity = unitOfWork.WriteOffReasonRepository.GetById(id);
            unitOfWork.WriteOffReasonRepository.Delete(entity);
            unitOfWork.SaveChanges();
        }
    }
}