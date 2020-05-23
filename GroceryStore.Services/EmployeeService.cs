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
    public class EmployeeService : BaseService, IEmployeeService
    {
        public EmployeeService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public List<Employee> GetAll()
        {
            return unitOfWork.EmployeeRepository.GetAll()
                .Include(employee => employee.IdCityNavigation)
                .Include(employee => employee.IdCityNavigation.IdCountryNavigation)
                .Include(employee => employee.IdMarketNavigation)
                .Include(employee => employee.IdRoleNavigation).ToList();
        }

        public Employee GetId(int id)
        {
            var item = unitOfWork.EmployeeRepository.GetById(id);
            if (item == null)
                throw new Exception("Such order not found");
            return item;
        }

        public Employee Create(Employee entity)
        {
            var valueToInsert = entity;
            unitOfWork.EmployeeRepository.Add(valueToInsert);
            unitOfWork.SaveTablesChanges("dbo.Employee");
            entity.Id = valueToInsert.Id;
            return entity;
        }

        public Employee Update(Employee entity)
        {
            var value = entity;
            unitOfWork.EmployeeRepository.Update(value);
            return entity;
        }

        public void Delete(int id)
        {
            var entity = unitOfWork.EmployeeRepository.GetById(id);
            unitOfWork.EmployeeRepository.Delete(entity);
            unitOfWork.SaveChanges();
        }
    }
}