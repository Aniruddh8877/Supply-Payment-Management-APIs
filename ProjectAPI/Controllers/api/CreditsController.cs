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
//    [RoutePrefix("api/Credits")]
//    public class CreditsController : ApiController
//    {
//        [HttpPost]
//        [Route("SaveCredit")]
//        public ExpandoObject SaveCredit(RequestModel requestModel)
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
//                    PartyPaymentCollectionModel model = JsonConvert.DeserializeObject<PartyPaymentCollectionModel>(decryptData);

//                    PartyPaymentCollection partyPaymentCollection;
//                    if (model.GetPartyPaymentCollection.PartyPaymentCollectionId > 0)
//                    {
//                        partyPaymentCollection = dbContext.PartyPaymentCollections.First(x => x.PartyPaymentCollectionId == model.GetPartyPaymentCollection.PartyPaymentCollectionId);
//                        if (partyPaymentCollection == null)
//                        {
//                            response.Message = "details not found";
//                            return response;

//                        }
//                        partyPaymentCollection.PaymentDate = model.GetPartyPaymentCollection.PaymentDate;
//                        partyPaymentCollection.Particular = model.GetPartyPaymentCollection.Particular;
//                        partyPaymentCollection.Amount = model.GetPartyPaymentCollection.Amount;
//                        partyPaymentCollection.PaymentMode = model.GetPartyPaymentCollection.PaymentMode;
//                        partyPaymentCollection.DDNO = model.GetPartyPaymentCollection.DDNO;
//                        partyPaymentCollection.ChequeNo = model.GetPartyPaymentCollection.ChequeNo;
//                        partyPaymentCollection.Remarks = model.GetPartyPaymentCollection.Remarks;
//                        partyPaymentCollection.TransactionNo= model.GetPartyPaymentCollection.TransactionNo;

//                        partyPaymentCollection.Status = model.GetPartyPaymentCollection.Status;
//                        partyPaymentCollection.UpdatedBy = model.GetPartyPaymentCollection.UpdatedBy;
//                        partyPaymentCollection.UpdatedOn = DateTime.Now;
//                    }
//                    else
//                    {
//                        partyPaymentCollection = new PartyPaymentCollection
//                        {
//                            PaymentDate = model.GetPartyPaymentCollection.PaymentDate,
//                            PartyId = model.GetPartyPaymentCollection.PartyId,
//                            Particular = model.GetPartyPaymentCollection.Particular,
//                            Amount = model.GetPartyPaymentCollection.Amount,
//                            PaymentMode = model.GetPartyPaymentCollection.PaymentMode,
//                            Status = model.GetPartyPaymentCollection.Status,
//                            ChequeNo = model.GetPartyPaymentCollection.ChequeNo,
//                            TransactionNo= model.GetPartyPaymentCollection.TransactionNo,
//                            DDNO = model.GetPartyPaymentCollection.DDNO,
//                            Remarks = model.GetPartyPaymentCollection.Remarks,
//                            CreatedBy = model.GetPartyPaymentCollection.CreatedBy,
//                            CreatedOn= DateTime.Now,
//                        };
//                        dbContext.PartyPaymentCollections.Add(partyPaymentCollection);
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
//                    response.PartyPaymentCollectionId = partyPaymentCollection.PartyPaymentCollectionId; 
//                }
//                catch(DbEntityValidationException ex)
//                {
//                    transaction.Rollback();

//                    var errorMessages = ex.EntityValidationErrors
//                        .SelectMany(x => x.ValidationErrors)
//                        .Select(x => $"Property: {x.PropertyName}, Error: {x.ErrorMessage}");

//                    string fullError = string.Join("; ", errorMessages);
//                    response.Message = fullError;
//                }
//            }

//            return response;
//        }
//    }
//}
