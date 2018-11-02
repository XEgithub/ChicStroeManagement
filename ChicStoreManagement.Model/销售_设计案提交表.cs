//------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    
    public partial class 销售_设计案提交表
    {
        public 销售_设计案提交表()
        {
            this.销售_设计案提交表_客户意向产品明细 = new HashSet<销售_设计案提交表_客户意向产品明细>();
            this.销售_设计案提交表_项目相关图纸 = new HashSet<销售_设计案提交表_项目相关图纸>();
            this.销售_设计案跟进日志 = new HashSet<销售_设计案跟进日志>();
        }
    
        public int id { get; set; }
        public int 接待记录ID { get; set; }
        public string 客户姓名 { get; set; }
        public string 联系方式 { get; set; }
        public string 职业 { get; set; }
        public string 家庭成员 { get; set; }
        public string 楼盘具体位置 { get; set; }
        public string 面积大小 { get; set; }
        public string 装修风格 { get; set; }
        public string 装修进度 { get; set; }
        public Nullable<decimal> 预算 { get; set; }
        public string 客户喜好或忌讳 { get; set; }
        public string 客户在意品牌或已购买品牌 { get; set; }
        public string 客户提问与要求 { get; set; }
        public Nullable<bool> 整体软装配饰 { get; set; }
        public string 家具空间 { get; set; }
        public string 销售人员 { get; set; }
        public string 设计人员 { get; set; }
        public string 店长 { get; set; }
        public Nullable<bool> 店长确认 { get; set; }
        public Nullable<bool> 设计人员确认 { get; set; }
        public System.DateTime 项目提交时间 { get; set; }
        public System.DateTime 项目量房时间 { get; set; }
        public System.DateTime 项目预计完成时间 { get; set; }
        public string 备注 { get; set; }
        public string 更新人 { get; set; }
        public Nullable<System.DateTime> 更新日期 { get; set; }
    
        public virtual 销售_接待记录 销售_接待记录 { get; set; }
        public virtual ICollection<销售_设计案提交表_客户意向产品明细> 销售_设计案提交表_客户意向产品明细 { get; set; }
        public virtual ICollection<销售_设计案提交表_项目相关图纸> 销售_设计案提交表_项目相关图纸 { get; set; }
        public virtual ICollection<销售_设计案跟进日志> 销售_设计案跟进日志 { get; set; }
    }
}
