﻿using PagedList;
using ChicStoreManagement.Model;
using ChicStoreManagement.IBLL;
using System.Web.Mvc;
using System.Linq;
using ChicStoreManagement.WEB.ViewModel;
using System;
using System.Collections.Generic;

namespace ChicStoreManagement.WEB.Controllers
{
    public class DesignTrackLogController : Controller
    {
        private readonly IDesignSubmitBLL designSubmitBLL;
        private readonly IDesignTrackingLogBLL designTrackingLogBLL;
        private readonly IDesign_ProjectDrawingsBLL design_ProjectDrawingsBLL;
        private readonly ICustomerInfoBLL customerInfoBLL;
        private readonly IStoreBLL storeBLL;
        private readonly IPositionBLL positionBLL;
        private readonly IStoreEmployeesBLL storeEmployeesBLL;


        private int employeeID;
        private string employeeName;
        private string store;
        private int storeID;
        public DesignTrackLogController(IDesignSubmitBLL designSubmitBLL, IDesignTrackingLogBLL designTrackingLogBLL, IDesign_ProjectDrawingsBLL design_ProjectDrawingsBLL, ICustomerInfoBLL customerInfoBLL, IStoreBLL storeBLL, IPositionBLL positionBLL, IStoreEmployeesBLL storeEmployeesBLL)
        {
            this.designSubmitBLL = designSubmitBLL;
            this.designTrackingLogBLL = designTrackingLogBLL;
            this.design_ProjectDrawingsBLL = design_ProjectDrawingsBLL;
            this.customerInfoBLL = customerInfoBLL;
            this.storeBLL = storeBLL;
            this.positionBLL = positionBLL;
            this.storeEmployeesBLL = storeEmployeesBLL;
        }

        /// <summary>
        /// 跟进记录首页
        /// </summary>
        /// <param name="id"></param>
        /// <param name="sortOrder"></param>
        /// <param name="searchString"></param>
        /// <param name="currentFilter"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        // GET: DesignTrackLog
        public ActionResult DesignTrakLogIndex(int? id, string sortOrder, string searchString, string currentFilter, int? page)
        {
            if (id == 0 || id == null)
            {
                return Content("非法操作");
            }
            ViewBag.DesignTrackSubmitID = id;
            Session["method"] = "N";
            SetEmployee();
            
            ViewBag.DesignTrackLogCurrentSort = sortOrder;
            ViewBag.DesignSubmitDate = String.IsNullOrEmpty(sortOrder) ? "first_desc" : "";
            ViewBag.CustomerName = sortOrder == "last" ? "last_desc" : "last";
            List<DesignTackLogViewModel> designTackLogViewModels = new List<DesignTackLogViewModel>();
            designTackLogViewModels = BuildDesignTrackLogInfo(id);
            if (designTackLogViewModels == null)
            {
                return Content("<script>alert('当前操作人并无关联的设计 </div>信息或无进入权限！');window.history.go(-1);</script>");
            }
            ViewBag.DesignTrackPeopleName = employeeName;//将当前操作人员传到前端
            ViewBag.DesignTrackstoreName = store;//将当前店铺名字传到前端
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.DesignTrackLogCurrentFilter = searchString;//获得前端传回来的搜索关键词

            if (!string.IsNullOrEmpty(searchString))
            {
                designTackLogViewModels = designTackLogViewModels.Where(w => w.联系方式 == searchString).ToList();
            }
            #region 排序，默认按ID升序
            switch (sortOrder)
            {
                case "first_desc":
                    designTackLogViewModels = designTackLogViewModels.OrderByDescending(w => w.跟进日期).ToList();
                    break;
                case "last_desc":
                    designTackLogViewModels = designTackLogViewModels.OrderByDescending(w => w.设计案需求提交时间).ToList();
                    break;
                case "last":
                    designTackLogViewModels = designTackLogViewModels.OrderBy(w => w.设计案需求提交时间).ToList();
                    break;
                default:
                    designTackLogViewModels = designTackLogViewModels.OrderBy(w => w.跟进日期).ToList();
                    break;
            }

            #endregion

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(designTackLogViewModels.ToPagedList(pageNumber, pageSize));
          
        }

        /// <summary>
        /// 添加跟进日志视图
        /// </summary>
        /// <param name="id">设计案提交表ID</param>
        /// <returns></returns>
        public ActionResult AddDesignTrackView(int? id)
        {
            if (id==0||id==null)
            {
                return Content("设计案申请不存在，非法操作!");
            }
            Session["method"] = "N";
            ViewBag.DesignSubmitID = id;

            return View();
        }

        [HttpPost]
        public ActionResult AddDesignTrackAction(DesignTackLogViewModel designTackLogViewModel)
        {
            SetEmployee();
            if (designSubmitBLL.GetModel(p => p.id == designTackLogViewModel.设计案提交表id).设计人员==null)
            {
                return Content("设计或店长未确认，非法操作!");
            }
            销售_设计案跟进日志 model = new 销售_设计案跟进日志
            {
                参与人员 = designTackLogViewModel.参与人员,
                备注 = designTackLogViewModel.备注,

                设计师 = designSubmitBLL.GetModel(p => p.id == designTackLogViewModel.设计案提交表id).设计人员,
                设计案提交表id = designTackLogViewModel.设计案提交表id,
                设计案需求提交时间 = designSubmitBLL.GetModel(p => p.id == designTackLogViewModel.设计案提交表id).更新日期,
                跟进日期 = designTackLogViewModel.跟进日期,
                进度描述 = designTackLogViewModel.进度描述,
                销售人员 = employeeName,
                需要的支持 = designTackLogViewModel.需要的支持,
                预计签约时间 = designTackLogViewModel.预计签约时间
            };
            if (ModelState.IsValid)
            {
              
                designTrackingLogBLL.Add(model);
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
                string  msg = "添加日志出错：";
                foreach (var item in sb)
                {
                    msg += item.ToString() + "<br/>";
                }
                return Content(msg);
            }
            return RedirectToAction("DesignTrakLogIndex",new { id=model.设计案提交表id});
        }

        /// <summary>
        /// 修改跟进日志
        /// </summary>
        /// <param name="id">跟进日志ID</param>
        /// <returns></returns>
        public ActionResult EditDesignTrackView(int? id) {
            Session["method"] = "N";
            if (id==0||id==null)
            {
                return Content("非法操作！");
            }
            DesignTackLogViewModel designTackLogViewModel = new DesignTackLogViewModel();
            销售_设计案跟进日志 model = new 销售_设计案跟进日志();
            model = designTrackingLogBLL.GetModel(p=>p.id==id);
            designTackLogViewModel.Id = model.id;
            designTackLogViewModel.参与人员 = model.参与人员;
            designTackLogViewModel.备注 = model.备注;
            designTackLogViewModel.客户姓名 = designSubmitBLL.GetModel(p => p.id == model.设计案提交表id).客户姓名;
            designTackLogViewModel.店长审查 = model.店长审查;
            designTackLogViewModel.楼盘具体位置 = designSubmitBLL.GetModel(p => p.id == model.设计案提交表id).楼盘具体位置;
            designTackLogViewModel.联系方式 = designSubmitBLL.GetModel(p => p.id == model.设计案提交表id).联系方式;
            designTackLogViewModel.设计师 = model.设计师;
            designTackLogViewModel.设计案提交表id = model.设计案提交表id;
            designTackLogViewModel.设计案需求提交时间 = model.设计案需求提交时间;
            designTackLogViewModel.跟进日期 = model.跟进日期;
            designTackLogViewModel.进度描述 = model.进度描述;
            designTackLogViewModel.销售人员 = model.销售人员;
            designTackLogViewModel.需要的支持 = model.需要的支持;
            designTackLogViewModel.预计签约时间 = model.预计签约时间;
            return View(designTackLogViewModel);
        }

        [HttpPost]
        public ActionResult EditDesignTrackAction(DesignTackLogViewModel designTackLogViewModel)
        {
            if (designTackLogViewModel==null)
            {
                return Content("数据为空！");
            }
            销售_设计案跟进日志 model = new 销售_设计案跟进日志
            {
                id = designTackLogViewModel.Id.Value,
                参与人员 = designTackLogViewModel.参与人员,
                备注 = designTackLogViewModel.备注,
                设计师 = designSubmitBLL.GetModel(p => p.id == designTackLogViewModel.设计案提交表id).设计人员,
                设计案提交表id = designTackLogViewModel.设计案提交表id,
                设计案需求提交时间 = designSubmitBLL.GetModel(p => p.id == designTackLogViewModel.设计案提交表id).更新日期,
                跟进日期 = designTackLogViewModel.跟进日期,
                进度描述 = designTackLogViewModel.进度描述,
                销售人员 = employeeName,
                需要的支持 = designTackLogViewModel.需要的支持,
                预计签约时间 = designTackLogViewModel.预计签约时间
            };
            if (ModelState.IsValid)
            {
                designTrackingLogBLL.Modify(model);
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
                string msg = "添加日志出错：";
                foreach (var item in sb)
                {
                    msg += item.ToString() + "<br/>";
                }
                return Content(msg);
            }
            return RedirectToAction("DesignTrakLogIndex");
        }
       
        /// <summary>
        /// 初始化设计案跟进日志
        /// </summary>
        /// <returns></returns>
        private List<DesignTackLogViewModel> BuildDesignTrackLogInfo(int? id)
        {
            if (id==0|| id==null)
            {
                return null;
            }
            List<DesignTackLogViewModel> designTackLogViewModels = new List<DesignTackLogViewModel>();
            try
            {

            var designTackLogList = designTrackingLogBLL.GetModels(p => p.设计案提交表id == id);   
            foreach (var item in designTackLogList)
            {
                DesignTackLogViewModel designTackLogViewModel = new DesignTackLogViewModel();
                designTackLogViewModel.Id = item.id;
                designTackLogViewModel.参与人员 = item.参与人员;
                designTackLogViewModel.备注 = item.备注;
                designTackLogViewModel.客户姓名 = designSubmitBLL.GetModel(p => p.id == id).客户姓名;
                designTackLogViewModel.联系方式 = designSubmitBLL.GetModel(p => p.id == id).联系方式;
                designTackLogViewModel.设计师 = item.设计师;
                designTackLogViewModel.设计案提交表id = item.设计案提交表id;
                designTackLogViewModel.设计案需求提交时间 = item.设计案需求提交时间;
                designTackLogViewModel.跟进日期 = item.跟进日期;
                designTackLogViewModel.进度描述 = item.进度描述;
                designTackLogViewModel.销售人员 = item.销售人员;
                designTackLogViewModel.需要的支持 = item.需要的支持;
                designTackLogViewModel.预计签约时间 = item.预计签约时间;
                designTackLogViewModels.Add(designTackLogViewModel);
            }

            }
            catch (Exception e)
            {

                throw e;
            }
            return designTackLogViewModels;

        }


        /// <summary>
        /// 设置当前的用户信息
        /// </summary>
        public void SetEmployee()
        {

            string userName = HttpContext.User.Identity.Name;
            if (userName != null)
            {
                var employees = HttpContext.Session["Employee"] as Employees;
                employeeID = employees.ID;
                employeeName = employees.姓名;
                store = employees.店铺;
                storeID = storeBLL.GetModel(p => p.名称 == store).ID;
            }
        }

        /// <summary>
        /// 构建客户信息
        /// </summary>
        /// <returns>客户信息</returns>
        public IQueryable<CustomerInfoModel> BuildCustomerInfo()
        {

            List<CustomerInfoModel> customerInfoModelsList = new List<CustomerInfoModel>();

            var customer = customerInfoBLL.GetModels(p => p.店铺ID == storeID);//查询当前店铺所有顾客接待信息
            if (customer != null)
            {
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


                    }
                    catch (Exception ex)
                    {

                        throw ex;
                    }

                    customerInfoModelsList.Add(customerInfo);
                }
            }


            return customerInfoModelsList.AsEnumerable().AsQueryable();

        }

    }
}