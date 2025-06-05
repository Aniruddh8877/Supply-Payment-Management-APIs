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
    [RoutePrefix("api/PartyPayment")]
    public class PartyPaymentController : ApiController
    {
        [HttpPost]
        [Route("PartyPaymentList")]
        public ExpandoObject PartypaymentList(RequestModel requestModel)
        {
            dynamic response = new ExpandoObject();
            try
            {
                PartyManageEntities dbContext = new PartyManageEntities();
                string AppKey = HttpContext.Current.Request.Headers["AppKey"];
                AppData.CheckAppKey(dbContext, AppKey, (byte)KeyFor.Admin);
                var decryptData = CryptoJs.Decrypt(requestModel.request, CryptoJs.key, CryptoJs.iv);
                PartyPaymentModel model = JsonConvert.DeserializeObject<PartyPaymentModel>(decryptData);

                var list = (from d1 in dbContext.PartyPayments
                            select new
                            {
                                d1.PartyPaymentId,
                                d1.PaymentDate,
                                d1.PartyId,
                                d1.PartyDetail.PartyName,
                                d1.Particular,
                                d1.Amount,
                                d1.PaymentMode,
                                d1.Status,
                                d1.Remarks,
                                d1.ChequeNo,
                                d1.DDNo,
                                d1.TransactionNo    ,
                                d1.CreatedBy,
                                d1.CreatedOn,
                                d1.UpdatedBy,
                                d1.UpdatedOn,
                            }).ToList();
                response.PartyPaymentList = list;
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpPost]
        [Route("SavePartyPayment")]
        public ExpandoObject SavePartyPayment(RequestModel requestModel)
        {
            dynamic response = new ExpandoObject();
            PartyManageEntities dbContext = new PartyManageEntities();

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    string AppKey = HttpContext.Current.Request.Headers["AppKey"];
                    AppData.CheckAppKey(dbContext, AppKey, (byte)KeyFor.Admin);
                    var decryptData = CryptoJs.Decrypt(requestModel.request, CryptoJs.key, CryptoJs.iv);
                    PartyPaymentModel model = JsonConvert.DeserializeObject<PartyPaymentModel>(decryptData);

                    PartyPayment partyPayment;
                    if (model.GetPartyPayment.PartyPaymentId > 0)
                    {
                        partyPayment = dbContext.PartyPayments.First(x => x.PartyPaymentId == model.GetPartyPayment.PartyPaymentId);
                        if(partyPayment == null)
                        {
                            response.Message = "details not found";
                            return response;
                        }
                        partyPayment.PaymentDate = model.GetPartyPayment.PaymentDate;
                        partyPayment.PartyId = model.GetPartyPayment.PartyId;
                        partyPayment.Particular = model.GetPartyPayment.Particular;
                        partyPayment.Amount = model.GetPartyPayment.Amount;
                        partyPayment.PaymentMode = model.GetPartyPayment.PaymentMode;
                        partyPayment.Status = model.GetPartyPayment.Status;
                        partyPayment.ChequeNo = model.GetPartyPayment.ChequeNo;
                        partyPayment.TransactionNo = model.GetPartyPayment.TransactionNo;
                        partyPayment.DDNo = model.GetPartyPayment.DDNo;
                        partyPayment.UpdatedBy = model.GetPartyPayment.UpdatedBy;
                        partyPayment.UpdatedOn = DateTime.Now;
                    }
                    else
                    {
                        partyPayment = new PartyPayment
                        {
                            PaymentDate = model.GetPartyPayment.PaymentDate,
                            PartyId = model.GetPartyPayment.PartyId,
                            Particular = model.GetPartyPayment.Particular,
                            Amount = model.GetPartyPayment.Amount,
                            PaymentMode = model.GetPartyPayment.PaymentMode,
                            Status = model.GetPartyPayment.Status,
                            ChequeNo = model.GetPartyPayment.ChequeNo,
                            TransactionNo = model.GetPartyPayment.TransactionNo,
                            DDNo = model.GetPartyPayment.DDNo,
                            Remarks = model.GetPartyPayment.Remarks,
                            CreatedBy = model.GetPartyPayment.CreatedBy,
                            CreatedOn = DateTime.Now,
                        };
                        dbContext.PartyPayments.Add(partyPayment);
                    }
                    dbContext.SaveChanges();

                    PartyPaymentDetail partyPaymentDetail;
                    if (model.GetPartyPaymentDetail.PartyPaymentId > 0)
                    {
                        partyPaymentDetail = dbContext.PartyPaymentDetails.First(x => x.PaymentDetailId == model.GetPartyPaymentDetail.PaymentDetailId);
                        if (partyPaymentDetail == null)
                        {
                            response.Message = "details not found";
                            return response;

                        }
                        partyPaymentDetail.PaymentDate = partyPayment.PaymentDate;
                        partyPaymentDetail.Particular = partyPayment.Particular;
                        partyPaymentDetail.CreditAmount = partyPayment.Amount;
                        partyPaymentDetail.Remarks = partyPayment.Remarks;
                        //partyPaymentDetail. = model.GetPartyPaymentCollection.Status;
                        partyPaymentDetail.UpdatedBy = partyPayment.UpdatedBy;
                        partyPaymentDetail.UpdatedOn = DateTime.Now;
                    }
                    else
                    {
                        partyPaymentDetail = new PartyPaymentDetail
                        {
                            PartyId = model.GetPartyPaymentDetail.PartyId,
                            PaymentDate = partyPayment.PaymentDate,
                            //Particular = model.GetPartyPaymentDetail.Particular,
                            CreditAmount = partyPayment.Amount,
                            PartyPaymentId = partyPayment.PartyPaymentId,
                            //PartySupplyItemId = model.GetPartySupplyItem.PartySupplyItemId,
                            Remarks = partyPayment.Remarks,
                            CreatedBy = partyPayment.CreatedBy,
                            CreatedOn = DateTime.Now,
                            Particular = "Supply Item ",
                        };
                        dbContext.PartyPaymentDetails.Add(partyPaymentDetail);
                    }
                    dbContext.SaveChanges();
                    transaction.Commit();

                    response.Message = ConstantData.SuccessMessage;
                    response.PartyPaymentId = partyPayment.PartyPaymentId;
                }
                catch (DbEntityValidationException ex)
                {
                    transaction.Rollback();

                    var errorMessages = ex.EntityValidationErrors
                        .SelectMany(x => x.ValidationErrors)
                        .Select(x => $"Property: {x.PropertyName}, Error: {x.ErrorMessage}");

                    string fullError = string.Join("; ", errorMessages);
                    response.Message = fullError;
                }
            }
            return response;
        }



        [HttpPost]
        [Route("DeletePartyPayment")]
        public ExpandoObject DeletePartyPayment(RequestModel requestModel)
        {
            dynamic res = new ExpandoObject();
            try
            {
                PartyManageEntities dataContext = new PartyManageEntities();
                string AppKey = HttpContext.Current.Request.Headers["AppKey"];
                if (string.IsNullOrEmpty(AppKey))
                {
                    res.Message = "AppKey is missing";
                    return res;
                }
                AppData.CheckAppKey(dataContext, AppKey, (byte)KeyFor.Admin);
                var decryptData = CryptoJs.Decrypt(requestModel.request, CryptoJs.key, CryptoJs.iv);
                PartyPayment model = JsonConvert.DeserializeObject<PartyPayment>(decryptData);
                var partyPayment = dataContext.PartyPayments.FirstOrDefault(x => x.PartyPaymentId == model.PartyPaymentId);
                if (partyPayment == null)
                {
                    res.Message = "Patient not found";
                    return res;
                }

                dataContext.PartyPayments.Remove(partyPayment);
                dataContext.SaveChanges();
                res.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("FK"))
                {
                    res.Message = "This record is in use. Can't delete.";
                }
                else
                {
                    res.Message = ex.Message;
                }
            }
            return res;
        }
    }
}
