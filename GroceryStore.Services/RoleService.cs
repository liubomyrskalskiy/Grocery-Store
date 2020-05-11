using System;
using System.Collections.Generic;
using System.Linq;
using GroceryStore.Core.Abstractions;
using GroceryStore.Core.Abstractions.IServices;
using GroceryStore.Core.Models;
using GroceryStore.Services.Base;

namespace GroceryStore.Services
{
    public class RoleService : BaseService, IRoleService
    {
        public RoleService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public List<Role> GetAll()
        {
            return unitOfWork.RoleRepository.GetAll().ToList();
        }

        public Role GetId(int id)
        {
            var item = unitOfWork.RoleRepository.GetById(id);
            if (item == null)
                throw new Exception("Such order not found");
            return item;
        }

        public Role Create(Role entity)
        {
            var valueToInsert = entity;
            unitOfWork.RoleRepository.Add(valueToInsert);
            unitOfWork.SaveTablesChanges("dbo.Role");
            entity.Id = valueToInsert.Id;
            return entity;
        }

        public Role Update(Role entity)
        {
            var value = entity;
            unitOfWork.RoleRepository.Update(value);
            return entity;
        }

        public void Delete(int id)
        {
            var entity = unitOfWork.RoleRepository.GetById(id);
            unitOfWork.RoleRepository.Delete(entity);
            unitOfWork.SaveChanges();
        }
    }
}