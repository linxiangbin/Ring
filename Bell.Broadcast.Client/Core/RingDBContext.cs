using Ring.Play.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ring.Play.Core
{
    public class RingDBContext : DbContext
    {
        public RingDBContext() : base("RingDBConnectionString")
        {
            //使用此初始值设定项将禁用给定上下文类型的数据库初始化
            //系统使用EF只为了ORM
            Database.SetInitializer<RingDBContext>(new NullDatabaseInitializer<RingDBContext>());
        }


        //数据上下文
        public DbSet<tb_PlanModel> tb_PlanModel { get; set; }
    }
}
