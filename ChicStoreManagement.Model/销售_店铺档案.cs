//------------------------------------------------------------------------------
// <auto-generated>
//    此代码是根据模板生成的。
//
//    手动更改此文件可能会导致应用程序中发生异常行为。
//    如果重新生成代码，则将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;

public partial class 销售_店铺档案
{
    public 销售_店铺档案()
    {
        this.销售_店铺员工档案 = new HashSet<销售_店铺员工档案>();
        this.销售_订单 = new HashSet<销售_订单>();
        this.销售_意向追踪日志 = new HashSet<销售_意向追踪日志>();
        this.销售_跟进目标数申请表 = new HashSet<销售_跟进目标数申请表>();
        this.销售_设计案跟进日志 = new HashSet<销售_设计案跟进日志>();
        this.设计_设计案完结单 = new HashSet<设计_设计案完结单>();
        this.销售_设计案提交表 = new HashSet<销售_设计案提交表>();
    }

    public int ID { get; set; }
    public Nullable<int> 经销商ID { get; set; }
    public Nullable<int> 品牌ID { get; set; }
    public string 编号 { get; set; }
    public string 名称 { get; set; }
    public string 地址 { get; set; }
    public string 商场 { get; set; }
    public Nullable<int> 地区ID { get; set; }
    public string 负责人 { get; set; }
    public string 负责人电话 { get; set; }
    public string 联系人 { get; set; }
    public string 联系人电话 { get; set; }
    public string 收货人 { get; set; }
    public string 收货地址 { get; set; }
    public string 收货人电话 { get; set; }
    public Nullable<int> 使用面积 { get; set; }
    public string 等级 { get; set; }
    public Nullable<bool> 停用标志 { get; set; }
    public Nullable<int> 制单人 { get; set; }
    public Nullable<System.DateTime> 制单日期 { get; set; }
    public string 密码 { get; set; }

    public virtual 销售_经销商档案 销售_经销商档案 { get; set; }
    public virtual ICollection<销售_店铺员工档案> 销售_店铺员工档案 { get; set; }
    public virtual ICollection<销售_订单> 销售_订单 { get; set; }
    public virtual ICollection<销售_意向追踪日志> 销售_意向追踪日志 { get; set; }
    public virtual ICollection<销售_跟进目标数申请表> 销售_跟进目标数申请表 { get; set; }
    public virtual ICollection<销售_设计案跟进日志> 销售_设计案跟进日志 { get; set; }
    public virtual 商品档案_品牌 商品档案_品牌 { get; set; }
    public virtual ICollection<设计_设计案完结单> 设计_设计案完结单 { get; set; }
    public virtual ICollection<销售_设计案提交表> 销售_设计案提交表 { get; set; }
}
