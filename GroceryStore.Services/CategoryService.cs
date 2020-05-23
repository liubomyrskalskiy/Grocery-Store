using System;
using System.Collections.Generic;
using System.Linq;
using GroceryStore.Core.Abstractions;
using GroceryStore.Core.Abstractions.IServices;
using GroceryStore.Core.Models;
using GroceryStore.Services.Base;

namespace GroceryStore.Services
{
    public class CategoryService : BaseService, ICategoryService
    {
        public CategoryService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public List<Category> GetAll()
        {
            var categories = unitOfWork.CategoryRepository.GetAll().ToList();
            unitOfWork.SaveChanges();
            return categories;
        }

        public Category GetId(int id)
        {
            var item = unitOfWork.CategoryRepository.GetById(id);
            if (item == null)
                throw new Exception("Such order not found");
            return item;
        }

        public Category Create(Category entity)
        {
            var valueToInsert = entity;
            unitOfWork.CategoryRepository.Add(valueToInsert);
            unitOfWork.SaveTablesChanges("dbo.Category");
            entity.Id = valueToInsert.Id;
            return entity;
        }

        public Category Update(Category entity)
        {
            var value = entity;
            unitOfWork.CategoryRepository.Update(value);
            return entity;
        }

        public void Delete(int id)
        {
            var entity = unitOfWork.CategoryRepository.GetById(id);
            unitOfWork.CategoryRepository.Delete(entity);
            unitOfWork.SaveChanges();
        }
    }
}