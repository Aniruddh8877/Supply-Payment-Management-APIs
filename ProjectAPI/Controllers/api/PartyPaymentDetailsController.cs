using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web;
using System.Dynamic;
using Project;
using Newtonsoft.Json;
using System.Data.Entity.Validation;

namespace ProjectAPI.Controllers.api
{
    [RoutePrefix("api/PartyPaymentDetail")]
    public class PartyPaymentDetailsController : ApiController
    {
        [HttpPost]
        [Route("PartyPaymentDetailList")]
        public ExpandoObject PartyPaymentDetailList(RequestModel requestModel)
        {
            dynamic res = new ExpandoObject();
            try
            {
                PartyManageEntities dbContext = new PartyManageEntities();
                string AppKey = HttpContext.Current.Request.Headers["AppKey"];
                AppData.CheckAppKey(dbContext, AppKey, (byte)KeyFor.Admin);
                var decryptData = CryptoJs.Decrypt(requestModel.request, CryptoJs.key, CryptoJs.iv);
                PartyPaymentDetail model = JsonConvert.DeserializeObject<PartyPaymentDetail>(decryptData);

                var list = (from p1 in dbContext.PartyPaymentDetails
                            where( model.PartyId == p1.PartyId )
                            select new
                            {
                                PaymentDetailId = p1.PaymentDetailId,
                                PartyId = p1.PartyId,
                                PartyName = p1.PartyDetail.PartyName,
                                PaymentDate = p1.PaymentDate,
                                PaymentMode = p1.PartyPayment.PaymentMode,
                                Particular = p1.Particular,
                                DebitAmount = p1.DebitAmount,
                                CreditAmount = p1.CreditAmount,
                                InvoiceNo = p1.PartySupplyItem.InvoiceNo,
                                //SupplyDate = p1.PartySupplyItem.SupplyDate,
                                CreatedBy = p1.CreatedBy,
                                CreatedOn = p1.CreatedOn,
                                UpdatedBy = p1.UpdatedBy,
                                UpdatedOn = p1.UpdatedOn,
                                //PaymentDate = p1.PartyPayment.PaymentDate
                            }).ToList();
                res.PartyPaymentDetailList = list;
                res.DebitAmount = list.Sum(p1 => p1.DebitAmount);
                res.CreditAmount = list.Sum(p1 => p1.CreditAmount);


                res.Message = ConstantData.SuccessMessage;
            }
            catch(Exception ex)
            {
                res.Message = ex.Message;
            }
            return res;
          
        }
    }
}
