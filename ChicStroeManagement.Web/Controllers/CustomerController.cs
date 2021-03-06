﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using ChicStoreManagement.IBLL;
using ChicStoreManagement.Model;
using ChicStoreManagement.WEB.ViewModel;
using Newtonsoft.Json;
using PagedList;

namespace ChicStoreManagement.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ICustomerInfoBLL customerInfoBLL;
        private readonly IExceptedBuyBLL exceptedBuyBLL;
        private readonly IStoreEmployeesBLL storeEmployeesBLL;
        private readonly IStoreBLL storeBLL;
        private readonly IProductCodeBLL productCodeBLL;
        private readonly ICustomerTrackingBLL customerTrackingBLL;
        private readonly ITrackGoalBLL trackGoalBLL;

        private int employeeID;//员工id
        private string employeeName;//员工姓名
        private string store;//当前店铺
        private int storeID;//当前店铺id
        private int positionID;
        private int? goals;
  
        public CustomerController(ICustomerInfoBLL customerInfoBLL, IExceptedBuyBLL exceptedBuyBLL, IStoreEmployeesBLL storeEmployeesBLL, IStoreBLL storeBLL, IProductCodeBLL productCodeBLL, ICustomerTrackingBLL customerTrackingBLL,ITrackGoalBLL trackGoalBLL)
        {
            this.customerInfoBLL = customerInfoBLL;
            this.exceptedBuyBLL = exceptedBuyBLL;
            this.storeEmployeesBLL = storeEmployeesBLL;
            this.storeBLL = storeBLL;
            this.productCodeBLL = productCodeBLL;
            this.customerTrackingBLL = customerTrackingBLL;
            this.trackGoalBLL = trackGoalBLL;
        }

       
        

        // GET: Customer
        /// <summary>
        /// 展示客户信息
        /// </summary>
        /// <returns></returns>
        public ViewResult CustomerIndex(string sortOrder, string searchString, string currentFilter, int? page)
        {
            Session["method"] = "N";
            SetEmployee();
            ViewBag.Store = store;
            ViewBag.Employee = employeeName;
            ViewBag.CustomerCurrentSort = sortOrder;
            ViewBag.IsManager = storeEmployeesBLL.GetModel(p => p.ID == employeeID).是否店长;
            ViewBag.IsDesigner = storeEmployeesBLL.GetModel(p => p.ID == employeeID).是否设计师;
            ViewBag.IsEmployee = storeEmployeesBLL.GetModel(p => p.ID == employeeID).是否销售;
            ViewBag.CustomerDate = sortOrder =="first"? "first_desc" : "first";//该排序作废
            ViewBag.CustomerID = sortOrder == "last" ? "last_desc" : "last";
            List<CustomerInfoModel> customerInfoModels = new List<CustomerInfoModel>();
            if (BuildCustomerInfo()!=null)
            {
                customerInfoModels = BuildCustomerInfo().ToList();//将顾客接待信息数据优化
            }
           
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CustomerCurrentFilter = searchString;//获得前端传回来的搜索关键词

            if (!string.IsNullOrEmpty(searchString))
            {
                customerInfoModels = customerInfoModels.Where(w => w.客户电话==searchString).ToList();//通过电话查找
            }
            //Session["Name"] = customerInfoModels.FirstOrDefault();
            #region 排序，默认按ID升序
            switch (sortOrder)
            {
                case "first_desc":
                    customerInfoModels = customerInfoModels.OrderByDescending(w => w.接待日期).ToList();
                    break;
                case "last_desc":
                    customerInfoModels = customerInfoModels.OrderByDescending(w => w.ID).ToList();
                    break;
                case "last":
                    customerInfoModels = customerInfoModels.OrderBy(w => w.ID).ToList();
                    break;
                default:
                    customerInfoModels = customerInfoModels.OrderBy(w => w.接待日期).ToList();
                    break;
            }

            #endregion
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            ViewBag.employeeName = employeeName;//给前端传入当前操作人
            ViewBag.IsManager = storeEmployeesBLL.GetModel(p => p.姓名 == employeeName).是否店长;//给前端传入当前操作人职位
            ViewBag.Goals = goals;//总共跟进数目
            ViewBag.AvailableGoals = goals - customerInfoBLL.GetModels(p=>p.跟进人ID==employeeID).Count();//剩余跟进数目


            return View(customerInfoModels.ToPagedList(pageNumber, pageSize));

        }

       
        /// <summary>
        /// 添加客户
        /// </summary>
        /// <returns></returns>
        public ViewResult AddCustomerView()
        {
            Session["method"] = "N";
            SetEmployee();
            ViewBag.Store = store;
            ViewBag.Employee = employeeName;
            ViewBag.IsManager = storeEmployeesBLL.GetModel(p => p.ID == employeeID).是否店长;
            ViewBag.IsDesigner = storeEmployeesBLL.GetModel(p => p.ID == employeeID).是否设计师;
            ViewBag.IsEmployee = storeEmployeesBLL.GetModel(p => p.ID == employeeID).是否销售;
            List<销售_店铺员工档案> employeeList = storeEmployeesBLL.GetModels(p => true).ToList();
            SelectList EmployeeListList = new SelectList(employeeList, "ID", "姓名");
            ViewBag.EmployeeOptions = EmployeeListList;

            var selectAgeList = new List<SelectListItem>() { new SelectListItem() { Value = "90后", Text = "90后" }, new SelectListItem() { Value = "85-90年代", Text = "85-90年代" }, new SelectListItem() { Value = "80-85年代", Text = "80-85年代" }, new SelectListItem() { Value = "70后", Text = "70后" }, new SelectListItem() { Value = "70前", Text = "70前" } };
            ViewBag.AgeOptions = selectAgeList;
            
            var productList = productCodeBLL.GetModels(p => true).ToList();
            SelectList productSelectListItems = new SelectList(productList, "型号", "型号");
            ViewBag.ProductOptions = productSelectListItems;
            
            CustomerInfoModel model = new CustomerInfoModel
            {
                接待序号 = Guid.NewGuid().ToString("D"),
                制单日期 = DateTime.Now,
                出店时间=DateTime.Now,
                进店时间=DateTime.Now,
                预计使用时间=DateTime.Now,
                接待日期 = DateTime.Now.ToString("d"),
                店铺 = store,
                接待人 = employeeName
            };

            return View(model);
        }
        [HttpPost]
        public ActionResult CustomerAdd(CustomerInfoModel customerInfoModel) {
            if (Session["method"].ToString() == "Y")
            {
                string str = string.Format("<script>alert('重复操作！');parent.location.href='CustomerIndex';</script>");
                return Content(str);
            }

            SetEmployee();
            var employee = storeEmployeesBLL.GetModel(p => p.姓名 == employeeName);
            销售_接待记录 model = new 销售_接待记录();
            try
            {
                
                model.店铺ID = storeBLL.GetModel(p => p.名称 == store).ID;
                model.接待人ID = storeEmployeesBLL.GetModel(p => p.姓名 == employeeName).ID;
                model.接待序号 = customerInfoModel.接待序号;
                model.接待日期 = DateTime.Now.Date;
                model.主导者 = customerInfoModel.主导者;
                model.主导者喜好风格 = customerInfoModel.主导者喜好风格;
                model.使用空间 = customerInfoModel.使用空间;
                model.出店时间 = customerInfoModel.出店时间;
                model.制单日期 = DateTime.Now;
                model.同行人 = customerInfoModel.同行人;
                model.如何得知品牌 = customerInfoModel.如何得知品牌;
                model.安装地址 = customerInfoModel.安装地址;
                model.客户姓名 = customerInfoModel.客户姓名;
                model.客户建议 = customerInfoModel.客户建议;
                model.客户来源 = customerInfoModel.客户来源;
                model.客户电话 = customerInfoModel.客户电话;
                model.客户着装 = customerInfoModel.客户着装;
                model.客户类别 = customerInfoModel.客户类别;
                model.客户类型 = customerInfoModel.客户类型;
                model.客户职业 = customerInfoModel.客户职业;
                model.家庭成员 = customerInfoModel.家庭成员;
                model.年龄段 = customerInfoModel.年龄段;
                model.性别 = customerInfoModel.性别;
                model.是否有意向 = customerInfoModel.是否有意向;
                if (employeeName != null)
                {
                    model.更新人 = storeEmployeesBLL.GetModel(p => p.姓名 == employeeName).ID;
                }

                model.更新日期 = DateTime.Now;
                model.来店次数 = 1;
                model.比较品牌 = customerInfoModel.比较品牌;
                model.特征 = customerInfoModel.特征;
                model.社交软件 = customerInfoModel.社交软件;
                model.装修情况 = customerInfoModel.装修情况;
                model.装修进度 = customerInfoModel.装修进度;
                model.装修风格 = customerInfoModel.装修风格;
                model.设计师 = customerInfoModel.设计师;
                var recPersonID = storeEmployeesBLL.GetModel(p => p.姓名 == customerInfoModel.接待人).ID;
                if (storeEmployeesBLL.GetModel(p=>p.姓名== customerInfoModel.接待人).跟进目标计划数>customerInfoBLL.GetModels(p=>p.跟进人ID== recPersonID).Count())
                {
                    model.跟进人ID = storeEmployeesBLL.GetModel(p => p.姓名 == customerInfoModel.接待人).ID;//初次添加且当前接待人员跟进指标未满，跟进人为当前接待人员。否则为空。
                }
               
               

                model.返点 = customerInfoModel.返点;
                model.进店时长 = int.Parse((customerInfoModel.出店时间 - customerInfoModel.进店时间).TotalMinutes.ToString());
                model.进店时间 = customerInfoModel.进店时间;
                model.预报价折扣 = customerInfoModel.预报价折扣;
                model.预算金额 = customerInfoModel.预算金额;
                model.预计使用时间 = customerInfoModel.预计使用时间;
                model.不喜欢产品 = customerInfoModel.不喜欢产品;
                model.不喜欢元素 = customerInfoModel.不喜欢元素;
                model.主卧预算备注 = customerInfoModel.主卧预算备注;
                model.主卧预算家具 = customerInfoModel.主卧预算家具;
                model.主卧预算金额 = customerInfoModel.主卧预算金额;
                model.书房预算备注 = customerInfoModel.书房预算备注;
                model.书房预算家具 = customerInfoModel.书房预算家具;
                model.书房预算金额 = customerInfoModel.书房预算金额;
                model.其它空间备注 = customerInfoModel.其它空间备注;
                model.其它空间家具 = customerInfoModel.其它空间家具;
                model.其它空间预算 = customerInfoModel.其它空间预算;
                model.喜欢产品 = customerInfoModel.喜欢产品;
                model.喜欢元素 = customerInfoModel.喜欢元素;
                model.婴儿或儿童房预算备注 = customerInfoModel.婴儿或儿童房预算备注;
                model.婴儿或儿童房预算家具 = customerInfoModel.婴儿或儿童房预算家具;
                model.婴儿或儿童房预算金额 = customerInfoModel.婴儿或儿童房预算金额;
                model.客厅预算备注 = customerInfoModel.客厅预算备注;
                model.客厅预算家具 = customerInfoModel.客厅预算家具;
                model.客厅预算金额 = customerInfoModel.客厅预算金额;
                model.户型大小 = customerInfoModel.户型大小;
                model.次卧预算备注 = customerInfoModel.次卧预算备注;
                model.次卧预算家具 = customerInfoModel.次卧预算家具;
                model.次卧预算金额 = customerInfoModel.次卧预算金额;
                model.比较品牌产品 = customerInfoModel.比较品牌产品;
                model.比较品牌产品备注 = customerInfoModel.比较品牌产品备注;
            }
            catch (Exception e)
            {

                return Content(e.Message); 
            }



            if (!ModelState.IsValid)
            {
                List<string> sb = new List<string>();
                //获取所有错误的Key
                List<string> Keys = ModelState.Keys.ToList();
                //获取每一个key对应的ModelStateDictionary
                foreach (var key in Keys)
                {
                    var errors = ModelState[key].Errors.ToList();
                    //将错误描述添加到sb中
                    foreach (var error in errors)
                    {
                        sb.Add(error.ErrorMessage);
                    }
                }
                string s = null;
                foreach (var item in sb)
                {
                    s += item.ToString() + ";";
                }
                return Content("<script>alert('" + s + "');window.history.go(-1);</script>");
            }
            var isHave = customerInfoBLL.GetModel(p => p.客户姓名 == model.客户姓名 && p.接待日期 == model.接待日期);
            if (isHave!= null)
            {
                return Content("<script>alert('数据已存在！，请勿重复提交');parent.location.href='CustomerIndex';</script>");
            }
            #region 如果客户已在当前店铺登记过
            try
            {
                var lis1 = customerInfoBLL.GetModels(p => p.客户姓名 == model.客户姓名 || p.客户电话 == model.客户电话);
                int num1 = lis1.Count();
                if (num1 >= 0) //统计客户来店登记次数
                {
                    model.来店次数 += num1;
                }
            }
            catch (Exception e)
            {

                return  Content(e.Message);
            }
          
             customerInfoBLL.Add(model); ;
             Session["method"] = "Y";
            #endregion
           
            
             
                var id=customerInfoBLL.GetModel(p=>p.接待序号==model.接待序号).接待人ID;
                return RedirectToAction("CustomerIndex");
           
           
           
        }
        /// <summary>
        /// 修改客户信息
        /// </summary>
        /// <returns></returns>
        public ActionResult EditCustomerView(int id)
        {
            Session["method"] = "N";
            SetEmployee();
            ViewBag.Store = store;
            ViewBag.Employee = employeeName;
            ViewBag.IsManager = storeEmployeesBLL.GetModel(p => p.ID == employeeID).是否店长;
            ViewBag.IsDesigner = storeEmployeesBLL.GetModel(p => p.ID == employeeID).是否设计师;
            ViewBag.IsEmployee = storeEmployeesBLL.GetModel(p => p.ID == employeeID).是否销售;
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
           
            SetEmployee();
            CustomerInfoModel customerInfo = new CustomerInfoModel();
            if (BuildCustomerInfo()!=null) { customerInfo = BuildCustomerInfo().First(p => p.ID == id); }
          

            if (customerInfo.接待人==employeeName||customerInfo.跟进人==employeeName||storeEmployeesBLL.GetModel(p=>p.姓名==employeeName).职务ID==3)
            {
                if (BuildExceptedBuy(id)!=null)
                {
               var exceptedBuyModels=BuildExceptedBuy(id);
                customerInfo.CustomerExceptedBuyModels = exceptedBuyModels;
                customerInfo.更新人 = employeeName;
                customerInfo.更新日期 = DateTime.Now;
                ViewBag.IsManager  = storeEmployeesBLL.GetModel(p => p.姓名 == employeeName).是否店长;
                }
               
                return View(customerInfo);
            }
           
            

            return Content("<script>alert('无操作权限！！');window.history.go(-1);</script>");

        }

     
        /// <summary>
        /// 提交修改数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CustomerEdit(CustomerInfoModel customerInfoModel) {
            if (Session["method"].ToString() == "Y")
            {
                string str = string.Format("<script>alert('重复操作！');parent.location.href='CustomerIndex';</script>");
                return Content(str);
            }
            if (customerInfoModel==null)
            {
                return Content("<script>alert('违规操作！！');window.history.go(-1);</script>");
            }
            DateTimeFormatInfo dtFormat = new DateTimeFormatInfo
            {
                ShortDatePattern = "yyyy/MM/dd"
            };
            销售_接待记录 model = new 销售_接待记录();
            try
            {
                model.ID = customerInfoModel.ID;
                model.店铺ID = storeBLL.GetModel(p => p.名称 == customerInfoModel.店铺).ID;
                model.接待人ID = storeEmployeesBLL.GetModel(p => p.姓名 == customerInfoModel.接待人).ID;
                model.接待序号 = customerInfoModel.接待序号;
                model.接待日期 = Convert.ToDateTime(customerInfoModel.接待日期, dtFormat);
                model.主导者 = customerInfoModel.主导者;
                model.主导者喜好风格 = customerInfoModel.主导者喜好风格;
                model.使用空间 = customerInfoModel.使用空间;
                model.出店时间 = customerInfoModel.出店时间;
                model.制单日期 = customerInfoModel.制单日期;
                model.同行人 = customerInfoModel.同行人;
                model.如何得知品牌 = customerInfoModel.如何得知品牌;
                model.安装地址 = customerInfoModel.安装地址;
                model.客户姓名 = customerInfoModel.客户姓名;
                model.客户建议 = customerInfoModel.客户建议;
                model.客户来源 = customerInfoModel.客户来源;
                model.客户电话 = customerInfoModel.客户电话;
                model.客户着装 = customerInfoModel.客户着装;
                model.客户类别 = customerInfoModel.客户类别;
                model.客户类型 = customerInfoModel.客户类型;
                model.客户职业 = customerInfoModel.客户职业;
                model.家庭成员 = customerInfoModel.家庭成员;
                model.年龄段 = customerInfoModel.年龄段;
                model.性别 = customerInfoModel.性别;
                model.是否有意向 = customerInfoModel.是否有意向;
                if (customerInfoModel.更新人 != null)
                {
                    model.更新人 = storeEmployeesBLL.GetModel(p => p.姓名 == customerInfoModel.更新人).ID;
                }

                model.更新日期 = customerInfoModel.更新日期;
                model.来店次数 = customerInfoModel.来店次数;
                model.比较品牌 = customerInfoModel.比较品牌;
                model.特征 = customerInfoModel.特征;
                model.社交软件 = customerInfoModel.社交软件;
                model.装修情况 = customerInfoModel.装修情况;
                model.装修进度 = customerInfoModel.装修进度;
                model.装修风格 = customerInfoModel.装修风格;
                model.设计师 = customerInfoModel.设计师;
                if (customerInfoModel.跟进人 != null)
                {
                    model.跟进人ID = storeEmployeesBLL.GetModel(p => p.姓名 == customerInfoModel.跟进人).ID;
                }

                model.返点 = customerInfoModel.返点;
                model.进店时长 = customerInfoModel.进店时长;
                model.进店时间 = customerInfoModel.进店时间;
                model.预报价折扣 = customerInfoModel.预报价折扣;
                model.预算金额 = customerInfoModel.预算金额;
                model.预计使用时间 = customerInfoModel.预计使用时间;
                model.不喜欢产品 = customerInfoModel.不喜欢产品;
                model.不喜欢元素 = customerInfoModel.不喜欢元素;
                model.主卧预算备注 = customerInfoModel.主卧预算备注;
                model.主卧预算家具 = customerInfoModel.主卧预算家具;
                model.主卧预算金额 = customerInfoModel.主卧预算金额;
                model.书房预算备注 = customerInfoModel.书房预算备注;
                model.书房预算家具 = customerInfoModel.书房预算家具;
                model.书房预算金额 = customerInfoModel.书房预算金额;
                model.其它空间备注 = customerInfoModel.其它空间备注;
                model.其它空间家具 = customerInfoModel.其它空间家具;
                model.其它空间预算 = customerInfoModel.其它空间预算;
                model.喜欢产品 = customerInfoModel.喜欢产品;
                model.喜欢元素 = customerInfoModel.喜欢元素;
                model.婴儿或儿童房预算备注 = customerInfoModel.婴儿或儿童房预算备注;
                model.婴儿或儿童房预算家具 = customerInfoModel.婴儿或儿童房预算家具;
                model.婴儿或儿童房预算金额 = customerInfoModel.婴儿或儿童房预算金额;
                model.客厅预算备注 = customerInfoModel.客厅预算备注;
                model.客厅预算家具 = customerInfoModel.客厅预算家具;
                model.客厅预算金额 = customerInfoModel.客厅预算金额;
                model.户型大小 = customerInfoModel.户型大小;
                model.次卧预算备注 = customerInfoModel.次卧预算备注;
                model.次卧预算家具 = customerInfoModel.次卧预算家具;
                model.次卧预算金额 = customerInfoModel.次卧预算金额;
                model.比较品牌产品 = customerInfoModel.比较品牌产品;
                model.比较品牌产品备注 = customerInfoModel.比较品牌产品备注;
                if (ModelState.IsValid)
                {
                    customerInfoBLL.Modify(model);
                    Session["method"] = "Y";
                }

                else
                {
                    List<string> sb = new List<string>();
                    //获取所有错误的Key
                    List<string> Keys = ModelState.Keys.ToList();
                    //获取每一个key对应的ModelStateDictionary
                    foreach (var key in Keys)
                    {
                        var errors = ModelState[key].Errors.ToList();
                        //将错误描述添加到sb中
                        foreach (var error in errors)
                        {
                            sb.Add(error.ErrorMessage);
                        }
                    }
                    string s = null;
                    foreach (var item in sb)
                    {
                        s += item.ToString() + "<br/>";
                    }
                    return Content("<script>alert('" + s + "');window.history.go(-1);</script>");
                }
            } 
            catch (Exception)
            {

                throw;
            }
            return RedirectToAction("CustomerIndex");
        }

        /// <summary>
        /// 申请跟进数
        /// </summary>
        /// <returns></returns>
        [HttpPost] 
        public ActionResult ApplyGoals()
        {
            SetEmployee();
            TrackGoalsViewModel tmodel = new TrackGoalsViewModel
            {
                员工姓名 = employeeName,
                店铺 = store,
                申请跟进数 = 1//默认为1
            };
            销售_跟进目标数申请表 model = new 销售_跟进目标数申请表
            {
                申请跟进数 = tmodel.申请跟进数,
                员工ID = employeeID,
                店铺ID = storeID
            };
            try
            {
                trackGoalBLL.Add(model);
            }
            catch (Exception e)
            {

                return Json(e.Message);
            }
           
            return Json("申请已提交！");
        }
        /// <summary>
        /// 显示详细信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult ShowVisitInfo(int id)
        {
            if (id == 0) {
                return View("查询ID不存在！");
            }
            SetEmployee();
            ViewBag.Store = store;
            ViewBag.Employee = employeeName;
            ViewBag.IsManager = storeEmployeesBLL.GetModel(p => p.ID == employeeID).是否店长;
            ViewBag.IsDesigner = storeEmployeesBLL.GetModel(p => p.ID == employeeID).是否设计师;
            ViewBag.IsEmployee = storeEmployeesBLL.GetModel(p => p.ID == employeeID).是否销售;
            CustomerInfoModel customerInfo = new CustomerInfoModel();
            if (BuildCustomerInfo() != null) { customerInfo = BuildCustomerInfo().First(p => p.ID == id); }
      
            if (BuildExceptedBuy(id)!=null)
            {
           var exceptedBuyModels= BuildExceptedBuy(id);
            customerInfo.CustomerExceptedBuyModels = exceptedBuyModels;
            }
          
            return View(customerInfo);

        }

        #region 预计购买操作
        /// <summary>
        /// 预计购买
        /// </summary>
        /// <param name="id">接待id</param>
        /// <returns></returns>
        public ActionResult ExceptedBuyIndex(int id)
        {
            Session["method"] = "N";
            if (id == 0) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SetEmployee();
            List<CustomerExceptedBuyModel> exceptedBuyModels = new List<CustomerExceptedBuyModel>();
            if (BuildExceptedBuy(id)!=null)
            {
                exceptedBuyModels = BuildExceptedBuy(id).ToList();
            }
            ViewBag.Store = store;
            ViewBag.Employee = employeeName;
            ViewBag.IsManager = storeEmployeesBLL.GetModel(p => p.ID == employeeID).是否店长;
            ViewBag.IsDesigner = storeEmployeesBLL.GetModel(p => p.ID == employeeID).是否设计师;
            ViewBag.IsEmployee = storeEmployeesBLL.GetModel(p => p.ID == employeeID).是否销售;
            ViewBag.receptionid = id;
            CustomerInfoModel customerInfo = new CustomerInfoModel();
            if (BuildCustomerInfo() != null) { customerInfo = BuildCustomerInfo().Where(p => p.ID == id).FirstOrDefault(); }
        
            ViewBag.CustomerName = customerInfoBLL.GetModel(p => p.ID == id).客户姓名;
            var productList = productCodeBLL.GetModels(p => true).ToList();
            try
            {
               
               
                SelectList productSelectListItems = new SelectList(productList, "型号", "型号");
                ViewBag.ProductOptions = productSelectListItems;

                if (exceptedBuyModels == null) { return Content("<script>alert('查无数据！！');window.history.go(-1);</script>"); }

             

            }
            catch (Exception)
            {

                throw;
            }
            if (customerInfo.接待人==employeeName||customerInfo.跟进人==employeeName|| storeEmployeesBLL.GetModel(p => p.姓名 == employeeName).职务ID == 3)
            {
                var model = exceptedBuyModels.ToList();

                return View(model);
            }
            return Content("<script>alert('无操作权限！！');window.history.go(-1);</script>");

        }
        /// <summary>
        /// 删除预购
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DelExceptedBuyAction(int ExceptedBuyID, int receptionId) {
            if (exceptedBuyBLL.GetModel(p=>p.ID==ExceptedBuyID)==null)
            {
                string str = string.Format("<script>alert('该数据已被更改，或不存在！');parent.location.href='CustomerIndex';</script>");
                return Content(str);
            }
           

            exceptedBuyBLL.DelBy(p => p.ID == ExceptedBuyID);
            Session["method"] = "Y";
           var exceptedBuyModels=BuildExceptedBuy(receptionId);

            ViewBag.receptionid = receptionId;
            var productList = productCodeBLL.GetModels(p => true).ToList();
            SelectList productSelectListItems = new SelectList(productList, "型号", "型号");
            ViewBag.ProductOptions = productSelectListItems;

            return Json(exceptedBuyModels);
        }
        /// <summary>
        /// 增加预购
        /// </summary>
        /// <param name="productCode"></param>
        /// <param name="remarks"></param>
        /// <returns></returns>
        [HttpPost]
         public ActionResult ExceptedBuyAdd(string exceptModel) {
            if (Session["method"].ToString() == "Y")
            {
                string str = string.Format("<script>alert('重复操作！');parent.location.href='CustomerIndex';</script>");
                return Json(str);
            }
            List<CustomerExceptedBuyModel> list = JsonConvert.DeserializeObject<List<CustomerExceptedBuyModel>>(exceptModel);
            if (list.Count==0)
            {
                return Json("<script>alert('不存在你跟进的客户，你不能执行预购操作！');window.history.go(-1);</script>");
            }
            foreach (var item in list)
            {
                销售_接待记录_意向明细 model = new 销售_接待记录_意向明细
                {
                    商品型号ID = productCodeBLL.GetModel(p => p.型号 == item.型号).ID,
                    备注 = item.备注,
                    接待ID = item.接待,
                    空间=item.空间
                };
                if (ModelState.IsValid)
                {
                    exceptedBuyBLL.Add(model);
                    Session["method"] = "Y";

                }

                else
                {
                    List<string> sb = new List<string>();
                    //获取所有错误的Key
                    List<string> Keys = ModelState.Keys.ToList();
                    //获取每一个key对应的ModelStateDictionary
                    foreach (var key in Keys)
                    {
                        var errors = ModelState[key].Errors.ToList();
                        //将错误描述添加到sb中
                        foreach (var error in errors)
                        {
                            sb.Add(error.ErrorMessage);
                        }
                    }
                    string s = null;
                    foreach (var it in sb)
                    {
                        s += it.ToString() + ";";
                    }
                    return Json("<script>alert('" + s + "');window.history.go(-1);</script>");
                }
            }


            //BuildExceptedBuy(model.接待ID);
            //ViewBag.receptionid = model.接待ID;
            //var productList = productCodeBLL.GetModels(p => true).ToList();
            //SelectList productSelectListItems = new SelectList(productList, "型号", "型号");
            //ViewBag.ProductOptions = productSelectListItems;
            //return RedirectToAction("ExceptedBuyIndex", new { id = list.FirstOrDefault().接待 });
            return Json("success");
        }
        public ViewResult AddExceptedBuyView(int receptionid) {
            Session["method"] = "N";
            ViewBag.AddReceptionid = receptionid;
            SetEmployee();
            ViewBag.Store = store;
            ViewBag.Employee = employeeName;
            ViewBag.IsManager = storeEmployeesBLL.GetModel(p => p.ID == employeeID).是否店长;
            ViewBag.IsDesigner = storeEmployeesBLL.GetModel(p => p.ID == employeeID).是否设计师;
            var productList = productCodeBLL.GetModels(p => true).ToList();
            SelectList productSelectListItems = new SelectList(productList, "型号", "型号");
            ViewBag.AddProductOptions = productSelectListItems;
            return View();
        }
        public JsonResult GetSelect() {
            var productList = productCodeBLL.GetModels(p => true).ToList();
            SelectList productSelectListItems = new SelectList(productList, "型号", "型号");
            
            return Json(productSelectListItems);
        }
        /// <summary>
        /// 更新预购
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult UpdateExceptedBuyAction(string 型号, string 备注, int 接待, int id,string 空间) {
            if (Session["method"].ToString() == "Y")
            {
                string str = string.Format("<script>alert('重复操作！');parent.location.href='CustomerIndex';</script>");
                return Content(str);
            }
            销售_接待记录_意向明细 model = new 销售_接待记录_意向明细
            {
                ID = id,
                接待ID = 接待,
                商品型号ID = productCodeBLL.GetModel(p => p.型号 == 型号).ID,
                备注 = 备注,
                空间=空间
            };
            
            if (ModelState.IsValid)
            {

                exceptedBuyBLL.Modify(model);
                Session["method"] = "Y";

            }
            else
            {
                List<string> sb = new List<string>();
                //获取所有错误的Key
                List<string> Keys = ModelState.Keys.ToList();
                //获取每一个key对应的ModelStateDictionary
                foreach (var key in Keys)
                {
                    var errors = ModelState[key].Errors.ToList();
                    //将错误描述添加到sb中
                    foreach (var error in errors)
                    {
                        sb.Add(error.ErrorMessage);
                    }
                }
                string s = null;
                foreach (var item in sb)
                {
                    s += item.ToString() + ";";
                }
                return Content("<script>alert('" + s + "');window.history.go(-1);</script>");
            }
            BuildExceptedBuy(接待);
                ViewBag.AddReceptionid = 接待;
                var productList = productCodeBLL.GetModels(p => true).ToList();
                SelectList productSelectListItems = new SelectList(productList, "型号", "型号");
                ViewBag.ProductOptions = productSelectListItems;
                return RedirectToAction("ExceptedBuyIndex",new {id=接待});
            

        }
        #endregion
      
        
        /// <summary>
        /// 设置当前操作人员及店铺信息
        /// </summary>
        private void SetEmployee()
        {
            string userName = HttpContext.User.Identity.Name;
            if (userName != null)
            {
                var employees = HttpContext.Session["Employee"] as Employees;
                employeeID = employees.ID;
                employeeName = employees.姓名;
                store = employees.店铺;
                storeID = storeBLL.GetModel(p => p.名称 == store).ID;
                positionID = storeEmployeesBLL.GetModel(p => p.ID == employeeID).职务ID;
            }
        }

        /// <summary>
        /// 初始化客户接待信息
        /// </summary>
        private IQueryable<CustomerInfoModel> BuildCustomerInfo()
        {
            List<CustomerInfoModel> customerInfoModelsList = new List<CustomerInfoModel>();
           
            
                List<销售_接待记录> customer = new List<销售_接待记录>();
                if (positionID == 3)
                {
                    customer= customerInfoBLL.GetModels(p => p.店铺ID == storeID).ToList();//查询当前店铺所有顾客接待信息
                   
                }
                else
                {
                    customer = customerInfoBLL.GetModels(p => p.接待人ID == employeeID || p.跟进人ID == employeeID).ToList();
                }
                
                    foreach (var item in customer)
                    {
                        CustomerInfoModel customerInfo = new CustomerInfoModel();
                        try
                        {


                            customerInfo.ID = item.ID;
                            customerInfo.店铺 = storeBLL.GetModel(p => p.ID == item.店铺ID).名称;
                            customerInfo.接待人 = storeEmployeesBLL.GetModel(p => p.ID == item.接待人ID).姓名;
                            customerInfo.接待序号 = item.接待序号;
                            customerInfo.接待日期 = item.接待日期.ToString("d");
                            customerInfo.主导者 = item.主导者;
                            customerInfo.主导者喜好风格 = item.主导者喜好风格;
                            customerInfo.使用空间 = item.使用空间;
                            customerInfo.出店时间 = item.出店时间;
                            customerInfo.制单日期 = item.制单日期;
                            customerInfo.同行人 = item.同行人;
                            customerInfo.如何得知品牌 = item.如何得知品牌;
                            customerInfo.安装地址 = item.安装地址;
                            customerInfo.客户姓名 = item.客户姓名;
                            customerInfo.客户建议 = item.客户建议;
                            customerInfo.客户来源 = item.客户来源;
                            customerInfo.客户电话 = item.客户电话;
                            customerInfo.客户着装 = item.客户着装;
                            customerInfo.客户类别 = item.客户类别;
                            customerInfo.客户类型 = item.客户类型;
                            customerInfo.客户职业 = item.客户职业;
                            customerInfo.家庭成员 = item.家庭成员;
                            customerInfo.年龄段 = item.年龄段;
                            customerInfo.性别 = item.性别;
                            customerInfo.是否有意向 = item.是否有意向;
                            if (item.更新人 != null)
                            {
                                customerInfo.更新人 = storeEmployeesBLL.GetModel(p => p.ID == item.更新人).姓名;
                            }

                            customerInfo.更新日期 = item.更新日期;
                            customerInfo.来店次数 = item.来店次数;
                            customerInfo.比较品牌 = item.比较品牌;
                            customerInfo.特征 = item.特征;
                            customerInfo.社交软件 = item.社交软件;
                            customerInfo.装修情况 = item.装修情况;
                            customerInfo.装修进度 = item.装修进度;
                            customerInfo.装修风格 = item.装修风格;
                            customerInfo.设计师 = item.设计师;
                            if (item.跟进人ID != null)
                            {
                                customerInfo.跟进人 = storeEmployeesBLL.GetModel(p => p.ID == item.跟进人ID).姓名;
                            }

                            customerInfo.返点 = item.返点;
                            customerInfo.进店时长 = item.进店时长;
                            customerInfo.进店时间 = item.进店时间;
                            customerInfo.预报价折扣 = item.预报价折扣;
                            customerInfo.预算金额 = item.预算金额;
                            customerInfo.预计使用时间 = item.预计使用时间;
                    customerInfo.不喜欢产品 = item.不喜欢产品;
                    customerInfo.不喜欢元素 = item.不喜欢元素;
                    customerInfo.主卧预算备注 = item.主卧预算备注;
                    customerInfo.主卧预算家具 = item.主卧预算家具;
                    customerInfo.主卧预算金额 = item.主卧预算金额;
                    customerInfo.书房预算备注 = item.书房预算备注;
                    customerInfo.书房预算家具 = item.书房预算家具;
                    customerInfo.书房预算金额 = item.书房预算金额;
                    customerInfo.其它空间备注 = item.其它空间备注;
                    customerInfo.其它空间家具 = item.其它空间家具;
                    customerInfo.其它空间预算 = item.其它空间预算;
                    customerInfo.喜欢产品 = item.喜欢产品;
                    customerInfo.喜欢元素 = item.喜欢元素;
                    customerInfo.婴儿或儿童房预算备注 = item.婴儿或儿童房预算备注;
                    customerInfo.婴儿或儿童房预算家具 = item.婴儿或儿童房预算家具;
                    customerInfo.婴儿或儿童房预算金额 = item.婴儿或儿童房预算金额;
                    customerInfo.客厅预算备注 = item.客厅预算备注;
                    customerInfo.客厅预算家具 = item.客厅预算家具;
                    customerInfo.客厅预算金额 = item.客厅预算金额;
                    customerInfo.户型大小 = item.户型大小;
                    customerInfo.次卧预算备注 = item.次卧预算备注;
                    customerInfo.次卧预算家具 = item.次卧预算家具;
                    customerInfo.次卧预算金额 = item.次卧预算金额;
                    customerInfo.比较品牌产品 = item.比较品牌产品;
                    customerInfo.比较品牌产品备注 = item.比较品牌产品备注;

                        }
                        catch (Exception ex)
                        {

                            throw ex;
                        }

                        customerInfoModelsList.Add(customerInfo);
                    }
               return customerInfoModelsList.AsEnumerable().AsQueryable();
        }

        /// <summary>
        /// 根据接待id查询产品信息
        /// </summary>
        private IQueryable<CustomerExceptedBuyModel> BuildExceptedBuy(int id) {
            if (id==0)
            {
                return null;
            }
            List<CustomerExceptedBuyModel> models = new List<CustomerExceptedBuyModel>();
            var exceptedBuy = exceptedBuyBLL.GetListBy(p=>p.接待ID==id);
            if (exceptedBuy!=null)
            {
                foreach (var item in exceptedBuy)
                {
                    CustomerExceptedBuyModel exceptedBuyModel = new CustomerExceptedBuyModel
                    {
                        ID = item.ID,
                        型号 = productCodeBLL.GetModel(p => p.ID == item.商品型号ID).型号,
                        备注 = item.备注,
                        空间=item.空间,
                        接待 = id
                    };
                    models.Add(exceptedBuyModel);
                }
            
            }
              return models.AsEnumerable().AsQueryable();

        }
    }
}