﻿using ChicStoreManagement.IBLL;
using ChicStoreManagement.IDAL;
using ChicStoreManagement.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChicStoreManagement.BLL
{
    /// <summary>
    /// 商品档案_SPU
    /// </summary>
   public partial  class Product_SPUBLL:BaseService<商品档案_SPU>,IProduct_SPUBLL
    {
        private IProduct_SPUDAL product_SPUDAL;

        public Product_SPUBLL(IProduct_SPUDAL product_SPUDAL)
        {
            this.product_SPUDAL = product_SPUDAL;
            Dal = product_SPUDAL;
        }
    }
}
