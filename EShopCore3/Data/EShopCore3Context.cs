using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EShopCore3.Models;

namespace EShopCore3.Data
{
    public class EShopCore3Context : DbContext
    {
        public EShopCore3Context (DbContextOptions<EShopCore3Context> options)
            : base(options)
        {
        }

        public DbSet<EShopCore3.Models.Admin> Admin { get; set; } = default!;

        public DbSet<EShopCore3.Models.Item>? Item { get; set; }

        public DbSet<EShopCore3.Models.User>? User { get; set; }
    }
}
