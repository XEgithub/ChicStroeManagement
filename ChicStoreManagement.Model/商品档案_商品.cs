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
    
    public partial class 商品档案_商品
    {
        public 商品档案_商品()
        {
            this.商品档案_SPU = new HashSet<商品档案_SPU>();
        }
    
        public int ID { get; set; }
        public int 型号ID { get; set; }
        public string 编号 { get; set; }
        public string 名称 { get; set; }
        public string 规格 { get; set; }
        public string 计量单位 { get; set; }
        public int 交货周期 { get; set; }
        public bool 是否进口 { get; set; }
        public bool 停用标志 { get; set; }
        public Nullable<int> 制单人 { get; set; }
        public Nullable<System.DateTime> 制单日期 { get; set; }
        public Nullable<int> OLD_ID { get; set; }
    
        public virtual 商品档案_型号 商品档案_型号 { get; set; }
        public virtual ICollection<商品档案_SPU> 商品档案_SPU { get; set; }
    }
}
