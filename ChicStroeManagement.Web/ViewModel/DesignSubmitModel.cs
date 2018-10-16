﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ChicStoreManagement.WEB.ViewModel
{
    /// <summary>
    /// CHIC软装服务设计案提交表
    /// </summary>
    public class DesignSubmitModel
    {
       
        public int Id { get; set; }

        [Display(Name ="接待记录ID")]
        [DataType(DataType.Text)]
        [Required]
        public int 接待记录ID { get; set; }
        [Display(Name = "客户姓名")]
        [DataType(DataType.Text)]
        [Required]
        public string 客户姓名 { get; set; }
        [Display(Name = "联系方式")]
        [DataType(DataType.PhoneNumber)]
        [Required]
        public string 联系方式 { get; set; }
        [Display(Name = "职业")]
        [DataType(DataType.Text)]
        [Required]
        public string 职业 { get; set; }
        [Display(Name = "家庭成员")]
        [DataType(DataType.Text)]
        [Required]
        public string 家庭成员 { get; set; }
        [Display(Name = "楼盘具体位置")]
        [DataType(DataType.Text)]
        [Required]
        public string 楼盘具体位置 { get; set; }
        [Display(Name = "面积大小")]
        [DataType(DataType.Text)]
        [Required]
        public string 面积大小 { get; set; }
        [Display(Name = "装修风格")]
        [DataType(DataType.Text)]
        [Required]
        public string 装修风格 { get; set; }
        [Display(Name = "装修进度")]
        [DataType(DataType.Text)]
        [Required]
        public string 装修进度 { get; set; }
        [Display(Name = "预算")]
        [DataType(DataType.Currency)]
        
        public Nullable<decimal> 预算 { get; set; }
        [Display(Name = "客户喜好或忌讳")]
        [DataType(DataType.Text)]
    
        public string 客户喜好 { get; set; }
        [Display(Name = "客户在意品牌或已购买品牌")]
        [DataType(DataType.Text)]
        
        public string 客户在意品牌或已购买品牌 { get; set; }
       
        [Display(Name = "客户提问与要求")]
        [DataType(DataType.Text)]

        public string 客户提问与要求 { get; set; }
        [Display(Name = "项目方案要求")]
        [DataType(DataType.Text)]
 
        public string 项目方案要求 { get; set; }
        [Display(Name = "整体软装配饰")]
        [DataType(DataType.Text)]
       
        public string 整体软装配饰 { get; set; }
        [Display(Name = "家具空间")]
        [DataType(DataType.Text)]
     
        public string 家具空间 { get; set; }
 
        [Display(Name = "销售人员")]
        [DataType(DataType.Text)]
        [Required]
        public string 销售人员 { get; set; }
        [Display(Name = "设计人员")]
        [DataType(DataType.Text)]
        
        public string 设计人员 { get; set; }
        [Display(Name = "店长")]
        [DataType(DataType.Text)]
        
        public string 店长 { get; set; }

        [Display(Name = "店长确认")]
        [DataType(DataType.Text)]
        [Required]
        public Nullable<bool> 店长确认 { get; set; }
        [Display(Name = "设计人员确认")]
        [DataType(DataType.Text)]
        [Required]
        public Nullable<bool> 设计人员确认 { get; set; }

        [Display(Name = "项目提交时间")]
        [DataType(DataType.DateTime)]
        [Required]
        public System.DateTime 项目提交时间 { get; set; }
        [Display(Name = "项目量房时间")]
        [DataType(DataType.DateTime)]
        [Required]
        public System.DateTime 项目量房时间 { get; set; }
        [Display(Name = "项目预计完成时间")]
        [DataType(DataType.DateTime)]
        [Required]
        public System.DateTime 项目预计完成时间 { get; set; }
        [Display(Name = "订单有效性")]
        [DataType(DataType.Text)]
        public bool 有效订单 { get; set; }

        [Display(Name ="备注")]
        [DataType(DataType.Text)]
        public string 备注 { get; set; }
        public virtual 销售_接待记录 销售_接待记录 { get; set; }
        public virtual ICollection<销售_设计案提交表_项目相关图纸> 销售_设计案提交表_项目相关图纸 { get; set; }
        public virtual ICollection<销售_设计案提交表_客户意向产品明细> 销售_设计案提交表_客户意向产品明细 { get; set; }
        public virtual ICollection<销售_设计案跟进日志> 销售_设计案跟进日志 { get; set; }
    }
}