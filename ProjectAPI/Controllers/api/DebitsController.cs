//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net;
//using System.Net.Http;
//using System.Web.Http;
//using System.Web;
//using System.Dynamic;
//using Project;
//using Newtonsoft.Json;
//using System.Data.SqlClient;
//using System.Data.Entity.Validation;

//namespace ProjectAPI.Controllers.api
//{
//    [RoutePrefix("api/Debits")]
//    public class DebitsController : ApiController
//    {
//        [HttpPost]
//        [Route("SaveDebit")]
//        public ExpandoObject SaveDebit(RequestModel requestModel)
//        {
//            dynamic response = new ExpandoObject();
//            using (var dbContext = new PartyManageEntities())
//            using (var transaction = dbContext.Database.BeginTransaction())
//            {
//                try
//                {
//                    string AppKey = HttpContext.Current.Request.Headers["AppKey"];
//                    if (string.IsNullOrEmpty(AppKey))
//                    {
//                        response.Message = "AppKey is missing";
//                        return response;
//                    }
//                    AppData.CheckAppKey(dbContext, AppKey, (byte)KeyFor.Admin);

//                    var decryptData = CryptoJs.Decrypt(requestModel.request, CryptoJs.key, CryptoJs.iv);
//                    PartySupplyPaymentModel model = JsonConvert.DeserializeObject<PartySupplyPaymentModel>(decryptData);

//                    PartySupplyItem partySupplyPayment;
//                    if (model.GetPartySupplyPayment.PartySupplyItemId > 0)
//                    {
//                        partySupplyPayment = dbContext.PartySupplyPayments.First(x => x.PartySupplyPaymentId == model.GetPartySupplyPayment.PartySupplyPaymentId);
//                        if (partySupplyPayment == null)
//                        {
//                            response.Message = "details not found";
//                            return response;
//                        }
//                        partySupplyPayment.SupplyDate = model.GetPartySupplyPayment.SupplyDate;
//                        partySupplyPayment.InvoiceNo = model.GetPartySupplyPayment.InvoiceNo;
//                        partySupplyPayment.Particular = model.GetPartySupplyPayment.Particular;
//                        partySupplyPayment.Amount = model.GetPartySupplyPayment.Amount;
//                        partySupplyPayment.Remarks = model.GetPartySupplyPayment.Remarks;
//                        partySupplyPayment.Status = model.GetPartySupplyPayment.Status;
//                        partySupplyPayment.Attachement = model.GetPartySupplyPayment.Attachement;
//                        partySupplyPayment.FileFormate = model.GetPartySupplyPayment.FileFormate;
//                        partySupplyPayment.UpdatedBy = model.GetPartySupplyPayment.UpdatedBy;
//                        partySupplyPayment.UpdatedOn = DateTime.Now;
//                    }
//                    else
//                    {
//                        partySupplyPayment = new PartySupplyPayment
//                        {
//                            SupplyDate = model.GetPartySupplyPayment.SupplyDate,
//                            PartyId = model.GetPartySupplyPayment.PartyId,
//                            InvoiceNo = model.GetPartySupplyPayment.InvoiceNo,
//                            Particular = model.GetPartySupplyPayment.Particular,
//                            Attachement = model.GetPartySupplyPayment.Attachement,
//                            FileFormate = model.GetPartySupplyPayment.FileFormate,
//                            Amount = model.GetPartySupplyPayment.Amount,
//                            Status = model.GetPartySupplyPayment.Status,
//                            CreatedBy = model.GetPartySupplyPayment.CreatedBy,
//                            CreatedOn = DateTime.Now,

//                        };
//                        dbContext.PartySupplyPayments.Add(partySupplyPayment);
//                    }
//                    dbContext.SaveChanges();




//                    PartyPaymentDetail partyPaymentDetail;
//                    if (model.GetPartyPaymentDetail.PartyPaymentCollectionId > 0)
//                    {
//                        partyPaymentDetail = dbContext.PartyPaymentDetails.First(x => x.PaymentDetailId == model.GetPartyPaymentDetail.PaymentDetailId);
//                        if (partyPaymentDetail == null)
//                        {
//                            response.Message = "details not found";
//                            return response;

//                        }
//                        partyPaymentDetail.PaymentDate = model.GetPartyPaymentDetail.PaymentDate;
//                        partyPaymentDetail.Particular = model.GetPartyPaymentDetail.Particular;
//                        partyPaymentDetail.DebitAmount = model.GetPartyPaymentDetail.DebitAmount;
//                        partyPaymentDetail.CreditAmount = model.GetPartyPaymentDetail.CreditAmount;
//                        //partyPaymentDetail. = model.GetPartyPaymentCollection.Status;
//                        partyPaymentDetail.UpdatedBy = model.GetPartyPaymentDetail.UpdatedBy;
//                        partyPaymentDetail.UpdatedOn = DateTime.Now;
//                    }
//                    else
//                    {
//                        partyPaymentDetail = new PartyPaymentDetail
//                        {
//                            PartyId = model.GetPartyPaymentDetail.PartyId,
//                            PaymentDate = model.GetPartyPaymentDetail.PaymentDate,
//                            Particular = model.GetPartyPaymentDetail.Particular,
//                            DebitAmount = model.GetPartyPaymentDetail.DebitAmount,
//                            CreditAmount = model.GetPartyPaymentDetail.CreditAmount,
//                            PartySupplyPaymentId = model.GetPartyPaymentDetail.PartySupplyPaymentId,
//                            PartyPaymentCollectionId = model.GetPartyPaymentDetail.PartyPaymentCollectionId,
//                            CreatedBy = model.GetPartyPaymentDetail.CreatedBy,
//                            CreatedOn = DateTime.Now,
//                        };
//                        dbContext.PartyPaymentDetails.Add(partyPaymentDetail);
//                    }
//                    dbContext.SaveChanges();

//                    dbContext.SaveChanges();
//                    transaction.Commit();

//                    response.Message = ConstantData.SuccessMessage;
//                    response.PartySupplyPaymentId = partySupplyPayment.PartySupplyPaymentId;
//                }
//                catch (DbEntityValidationException ex)
//                {
//                    transaction.Rollback();

//                    var errorMessages = ex.EntityValidationErrors
//                        .SelectMany(x => x.ValidationErrors)
//                        .Select(x => $"Property: {x.PropertyName}, Error: {x.ErrorMessage}");

//                    string fullError = string.Join("; ", errorMessages);
//                    response.Message = fullError;
//                }
//                return response;
//            }
//        }
//    }
//}
