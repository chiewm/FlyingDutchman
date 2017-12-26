﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace FlyingDutchman.Models
{
    public class MyShopDbContext:DbContext
    {
        public MyShopDbContext() : base("MyShop") { }
        public DbSet<Statistic> Statistics { get; set; }
        public DbSet<User> Users { get; set; }
    }
}