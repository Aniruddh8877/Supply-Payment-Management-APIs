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
                            where model.PartyId == p1.PartyId
                            select new
                            {
                                p1.PaymentDetailId,
                                p1.PartyId,
                                PartyName = p1.PartyDetail != null ? p1.PartyDetail.PartyName : "",
                                p1.PaymentDate,
                                PaymentMode = p1.PartyPayment != null ? p1.PartyPayment.PaymentMode : (byte)0,
                                p1.Particular,
                                p1.DebitAmount,
                                p1.CreditAmount,
                                InvoiceNo = p1.PartySupplyItem != null ? p1.PartySupplyItem.InvoiceNo : "",
                                p1.CreatedBy,
                                p1.CreatedOn,
                                p1.UpdatedBy,
                                p1.UpdatedOn,

                            }).ToList();

                res.PartyPaymentDetailList = list;
                res.DebitAmount = list.Sum(p => p.DebitAmount);
                res.CreditAmount = list.Sum(p => p.CreditAmount);
                res.Message = ConstantData.SuccessMessage;
            }
            catch (DbEntityValidationException ex)
            {
                res.Message = string.Join("; ", ex.EntityValidationErrors
                    .SelectMany(e => e.ValidationErrors)
                    .Select(e => e.PropertyName + ": " + e.ErrorMessage));
            }
            catch (Exception ex)
            {
                res.Message = $"Error: {ex.Message}";
                if (ex.InnerException != null)
                {
                    res.Message += $" | Inner: {ex.InnerException.Message}";
                }
            }

            return res;
        }

        [HttpPost]
        [Route("PartyDueHistoryList")]
        public ExpandoObject PartyDueHistoryList(RequestModel requestModel)
        {
            dynamic res = new ExpandoObject();
            try
            {
                PartyManageEntities dbContext = new PartyManageEntities();

                string AppKey = HttpContext.Current.Request.Headers["AppKey"];
                AppData.CheckAppKey(dbContext, AppKey, (byte)KeyFor.Admin);

                var decryptData = CryptoJs.Decrypt(requestModel.request, CryptoJs.key, CryptoJs.iv);
                PartyPaymentDetail model = JsonConvert.DeserializeObject<PartyPaymentDetail>(decryptData);

                // If LocationId is provided, filter by it
                var query = dbContext.PartyPaymentDetails
                    .Where(p => p.PartyDetail != null &&
                                (model.LocationId == null || p.PartyDetail.LocationId == model.LocationId));

                var summaryList = query
                    .GroupBy(p => new
                    {
                        p.PartyId,
                        p.PartyDetail.PartyName,
                        p.PartyDetail.LocationId
                    })
                    .Select(g => new
                    {
                        PartyId = g.Key.PartyId,
                        PartyName = g.Key.PartyName,
                        LocationName = dbContext.Locations
                            .Where(l => l.LocationId == g.Key.LocationId)
                            .Select(l => l.LocationName)
                            .FirstOrDefault(),
                        TotalCreditAmount = g.Sum(x => x.CreditAmount),
                        TotalDebitAmount = g.Sum(x => x.DebitAmount),
                        
                    })
                    .OrderBy(p => p.PartyName)
                    .ToList();
                decimal grandTotalCredit = summaryList.Sum(p => p.TotalCreditAmount);
                decimal grandTotalDebit = summaryList.Sum(p => p.TotalDebitAmount);

                res.PartyDueHistoryList = summaryList;
                res.AllCreditAmount = grandTotalCredit;
                res.AllDebitAmount = grandTotalDebit;
                res.AllBalanceDue = grandTotalDebit - grandTotalCredit;
                res.Message = ConstantData.SuccessMessage;
            }
            catch (DbEntityValidationException ex)
            {
                res.Message = string.Join("; ", ex.EntityValidationErrors
                    .SelectMany(e => e.ValidationErrors)
                    .Select(e => e.PropertyName + ": " + e.ErrorMessage));
            }
            catch (Exception ex)
            {
                res.Message = $"Error: {ex.Message}";
                if (ex.InnerException != null)
                {
                    res.Message += $" | Inner: {ex.InnerException.Message}";
                }
            }

            return res;
        }


    }
}