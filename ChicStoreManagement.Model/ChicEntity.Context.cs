﻿//------------------------------------------------------------------------------
// <auto-generated>
//    此代码是根据模板生成的。
//
//    手动更改此文件可能会导致应用程序中发生异常行为。
//    如果重新生成代码，则将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace ChicStoreManagement.Model
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class chicEntities : DbContext
    {
        public chicEntities()
            : base("name=chicEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<商品档案_型号> 商品档案_型号 { get; set; }
        public DbSet<设计_设计案完结单_家具成交单> 设计_设计案完结单_家具成交单 { get; set; }
        public DbSet<系统用户> 系统用户 { get; set; }
        public DbSet<销售_店铺档案> 销售_店铺档案 { get; set; }
        public DbSet<销售_店铺员工档案> 销售_店铺员工档案 { get; set; }
        public DbSet<销售_接待记录_意向明细> 销售_接待记录_意向明细 { get; set; }
        public DbSet<销售_经销商档案> 销售_经销商档案 { get; set; }
        public DbSet<销售_职务> 销售_职务 { get; set; }
        public DbSet<销售_设计案提交表_客户意向产品明细> 销售_设计案提交表_客户意向产品明细 { get; set; }
        public DbSet<设计_文件类型> 设计_文件类型 { get; set; }
        public DbSet<销售_设计案提交表_项目相关图纸> 销售_设计案提交表_项目相关图纸 { get; set; }
        public DbSet<销售_订单> 销售_订单 { get; set; }
        public DbSet<销售_订单明细> 销售_订单明细 { get; set; }
        public DbSet<商品档案_SKU> 商品档案_SKU { get; set; }
        public DbSet<销售_意向追踪日志> 销售_意向追踪日志 { get; set; }
        public DbSet<销售_跟进目标数申请表> 销售_跟进目标数申请表 { get; set; }
        public DbSet<销售_设计案跟进日志> 销售_设计案跟进日志 { get; set; }
        public DbSet<商品档案_商品> 商品档案_商品 { get; set; }
        public DbSet<商品档案_系列> 商品档案_系列 { get; set; }
        public DbSet<商品档案_分类> 商品档案_分类 { get; set; }
        public DbSet<商品档案_品牌> 商品档案_品牌 { get; set; }
        public DbSet<商品档案_SPU> 商品档案_SPU { get; set; }
        public DbSet<设计_设计案完结单> 设计_设计案完结单 { get; set; }
        public DbSet<销售_设计案提交表> 销售_设计案提交表 { get; set; }
        public DbSet<销售_接待记录> 销售_接待记录 { get; set; }
    }
}
