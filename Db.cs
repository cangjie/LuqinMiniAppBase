using System;
using Microsoft.EntityFrameworkCore;
namespace LuqinMiniAppBase
{
    public class Db : DbContext
    {
        public Db(DbContextOptions<Db> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }
    }
}
