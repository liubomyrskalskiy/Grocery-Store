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
    public class GoodsWriteOffService : BaseService, IGoodsWriteOffService
    {
        public GoodsWriteOffService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public List<GoodsWriteOff> GetAll()
        {
            return unitOfWork.GoodsWriteOffRepository.GetAll().Include(writeOff =>
                    writeOff.IdDeliveryShipmentNavigation.IdConsignmentNavigation)
                .Include(writeOff => writeOff.IdEmployeeNavigation)
                .Include(writeOff => writeOff.IdWriteOffReasonNavigation)
                .Include(writeOff => writeOff.IdGoodsInMarketNavigation.IdGoodsNavigation).ToList();
        }

        public GoodsWriteOff GetId(int id)
        {
            var item = unitOfWork.GoodsWriteOffRepository.GetById(id);
            if (item == null)
                throw new Exception("Such order not found");
            return item;
        }

        public GoodsWriteOff Create(GoodsWriteOff entity)
        {
            var valueToInsert = entity;
            unitOfWork.GoodsWriteOffRepository.Add(valueToInsert);
            unitOfWork.SaveTablesChanges("dbo.Goods_Write_off");
            entity.Id = valueToInsert.Id;
            return entity;
        }

        public GoodsWriteOff Update(GoodsWriteOff entity)
        {
            var value = entity;
            unitOfWork.GoodsWriteOffRepository.Update(value);
            return entity;
        }

        public void Delete(int id)
        {
            var entity = unitOfWork.GoodsWriteOffRepository.GetById(id);
            unitOfWork.GoodsWriteOffRepository.Delete(entity);
            unitOfWork.SaveChanges();
        }
    }
}