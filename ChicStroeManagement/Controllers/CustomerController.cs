﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ChicStroeManagement.Controllers
{
    public class CustomerController : Controller
    {
        // GET: Customer
        /// <summary>
        /// 展示客户信息
        /// </summary>
        /// <returns></returns>
        public ActionResult CustomerIndex()
        {
            return View();
        }

        /// <summary>
        /// 添加客户
        /// </summary>
        /// <returns></returns>
        public ActionResult AddCustomerView() {
            return View();

        }

        /// <summary>
        /// 修改客户信息
        /// </summary>
        /// <returns></returns>
        public ActionResult EditCustomerView()
        {
            return View();

        }
        //[HttpPost]
        //public ActionResult ChangeVisitInfo(List<string> AccountName, List<string> CustomerName, List<string> StartTime, List<string> VisitWay, List<string> VisitResult, List<string> ManagerTips)
        //{

        //    for (int i = 0; i < AccountName.Count; i++)
        //    {
        //        VisitInfoModel vim = new VisitInfoModel
        //        {
        //            AccountName = AccountName[i],
        //            CustomerName = CustomerName[i],
        //            StartTime = DateTime.Parse(StartTime[i].ToString()),
        //            VisitWay = VisitWay[i],
        //            VisitResult = VisitResult[i],
        //            ManagerTips = ManagerTips[i]
        //        };
        //        VisitInfoAdd(vim);
        //    }
        //    return RedirectToAction("Index", "Home");
        //}

        //public bool VisitInfoAdd(VisitInfoModel vim)
        //{

        //    return true;
        //}
        //public ActionResult ShowVisitInfo()
        //{

        //    return View();

        //}
    }
}