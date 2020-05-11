﻿using GroceryStore.Core.Abstractions.Repositories;
using GroceryStore.Core.Models;
using GroceryStore.DAL.Repositories.Base;

namespace GroceryStore.DAL.Repositories
{
    public class GoodsOwnRepository : BaseRepository<GoodsOwn>, IGoodsOwnRepository
    {
        public GoodsOwnRepository(GroceryStoreDbContext context) : base(context)
        {
        }
    }
}