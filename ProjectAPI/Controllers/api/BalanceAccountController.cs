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
using System.Data.SqlClient;
using System.Data.Entity.Validation;

namespace ProjectAPI.Controllers.api
{
    [RoutePrefix("api/Balance")]
    public class BalanceAccountController : ApiController
    {
        [HttpPost]
        [Route("BalanceList")]
        public ExpandoObject BalanceList(RequestModel requestModel)
        {
            dynamic response = new ExpandoObject();
            try
            {
                PartyManageEntities dbContext = new PartyManageEntities();
                string AppKey = HttpContext.Current.Request.Headers["AppKey"];
                AppData.CheckAppKey(dbContext, AppKey, (byte)KeyFor.Admin);
                var decryptData = CryptoJs.Decrypt(requestModel.request, CryptoJs.key, CryptoJs.iv);
                BalanceAccount model = JsonConvert.DeserializeObject<BalanceAccount>(decryptData);

                var list = (from b1 in dbContext.BalanceAccounts
                            join loc in dbContext.PartyDetails
                            on b1.PartyId equals loc.PartyId into locGroup
                            from loc in locGroup.DefaultIfEmpty()
                            select new
                            {
                                b1.BalanceAccoountId,
                                b1.PartyId,
                               PartyName = loc != null ? loc.PartyName : null,
                                PartyCode = loc !=null? loc.PartyCode:null,
                                b1.Openingdate,
                                b1.Debit,
                                b1.Credit,
                                b1.CreatedBy,
                                b1.CreatedOn,
                                b1.UpdatedBy,
                                b1.UpdatedOn,

                            }).ToList();
                response.BalanceList = list;
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpPost]
        [Route("SaveBalance")]
        public ExpandoObject SaveBalance(RequestModel requestModel)
        {

            dynamic response = new ExpandoObject();
            using (var dbContext = new PartyManageEntities())
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    string AppKey = HttpContext.Current.Request.Headers["AppKey"];
                    AppData.CheckAppKey(dbContext, AppKey, (byte)KeyFor.Admin);
                    var decryptData = CryptoJs.Decrypt(requestModel.request, CryptoJs.key, CryptoJs.iv);
                    BalanceAccountModel model = JsonConvert.DeserializeObject<BalanceAccountModel>(decryptData);

                    BalanceAccount balanceAccount;
                    if (model.GetBalanceAccount.BalanceAccoountId > 0)
                    {
                        balanceAccount = dbContext.BalanceAccounts.First(x => x.BalanceAccoountId == model.GetBalanceAccount.BalanceAccoountId);
                        if (balanceAccount == null)
                        {
                            response.Message = "details not found";
                            return response;
                        }
                        balanceAccount.Openingdate = model.GetBalanceAccount.Openingdate;
                        balanceAccount.PartyId = model.GetBalanceAccount.PartyId;
                        balanceAccount.Debit = model.GetBalanceAccount.Debit;
                        balanceAccount.Credit = model.GetBalanceAccount.Credit;
                        balanceAccount.UpdatedBy = model.GetBalanceAccount.UpdatedBy;
                        balanceAccount.UpdatedOn = DateTime.Now;
                    }
                    else
                    {
                        balanceAccount = new BalanceAccount
                        {
                            PartyId = model.GetBalanceAccount.PartyId,
                            Openingdate = model.GetBalanceAccount.Openingdate,
                            Debit = model.GetBalanceAccount.Debit,
                            Credit = model.GetBalanceAccount.Credit,
                            CreatedBy = model.GetBalanceAccount.CreatedBy,
                            CreatedOn = DateTime.Now,
                        };
                        dbContext.BalanceAccounts.Add(balanceAccount);
                    }
                    dbContext.SaveChanges();  


                    //PartyPaymentDetail partyPaymentDetail;
                    //if (model.GetPartyPaymentDetail.PartyPaymentCollectionId > 0)
                    //{
                    //    partyPaymentDetail = dbContext.PartyPaymentDetails.First(x => x.PaymentDetailId == model.GetPartyPaymentDetail.PaymentDetailId);
                    //    if (partyPaymentDetail == null)
                    //    {
                    //        response.Message = "details not found";
                    //        return response;

                    //    }
                    //    partyPaymentDetail.PaymentDate = model.GetPartyPaymentDetail.PaymentDate;
                    //    partyPaymentDetail.Particular = model.GetPartyPaymentDetail.Particular;
                    //    partyPaymentDetail.DebitAmount = model.GetPartyPaymentDetail.DebitAmount;
                    //    partyPaymentDetail.CreditAmount = model.GetPartyPaymentDetail.CreditAmount;
                    //    //partyPaymentDetail. = model.GetPartyPaymentCollection.Status;
                    //    partyPaymentDetail.UpdatedBy = model.GetPartyPaymentDetail.UpdatedBy;
                    //    partyPaymentDetail.UpdatedOn = DateTime.Now;
                    //}
                    //else
                    //{
                    PartyPaymentDetail partyPaymentDetail = new PartyPaymentDetail
                    {
                        BalanceAccountId = balanceAccount.BalanceAccoountId,
                        PartyId = balanceAccount.PartyId,
                        PaymentDate = balanceAccount.Openingdate,
                        DebitAmount = balanceAccount.Debit,
                        CreditAmount = balanceAccount.Credit,
                        CreatedBy = balanceAccount.CreatedBy,
                        CreatedOn = DateTime.Now,
                        Particular = "Opening\\",

                    };
                    dbContext.PartyPaymentDetails.Add(partyPaymentDetail);
                    //}
                    dbContext.SaveChanges();
                    transaction.Commit();
                    response.Message = ConstantData.SuccessMessage;
                    response.BalanceAccountId = balanceAccount.BalanceAccoountId;
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
                return response;
            }
        }

        //[HttpPost]
        //[Route("DeleteBalance")]
        //public ExpandoObject DeleteBalance(RequestModel requestModel)
        //{
        //    dynamic response = new ExpandoObject();

        //    using (var dbContext = new PartyManageEntities())
        //    using (var transaction = dbContext.Database.BeginTransaction())
        //    {
        //        try
        //        {
        //            string AppKey = HttpContext.Current.Request.Headers["AppKey"];
        //            if (string.IsNullOrEmpty(AppKey))
        //            {
        //                response.Message = "AppKey is missing";
        //                return response;
        //            }

        //            AppData.CheckAppKey(dbContext, AppKey, (byte)KeyFor.Admin);

        //            var decryptData = CryptoJs.Decrypt(requestModel.request, CryptoJs.key, CryptoJs.iv);
        //            BalanceAccountModel model = JsonConvert.DeserializeObject<BalanceAccountModel>(decryptData);

        //            if (model.GetBalanceAccount.BalanceAccoountId == null || model.GetBalanceAccount.BalanceAccoountId <= 0)
        //            {
        //                response.Message = "Invalid SurgeryId";
        //                return response;
        //            }

        //            int BalanceId = model.GetBalanceAccount.BalanceAccoountId;

                    
        //            var surgery = dbContext.BalanceAccounts.FirstOrDefault(x => x.BalanceAccoountId == BalanceId);
        //            if (surgery == null)
        //            {
        //                response.Message = "Surgery not found";
        //                return response;
        //            }

        //            // Delete PaymentDetails
        //            var paymentCollections = dbContext.PartyPaymentDetails
        //                .Where(pc => pc.PaymentDetailId == BalanceId)
        //                .ToList();

        //            foreach (var pc in paymentCollections)
        //            {
        //                var paymentDetails = dbContext.PartyPaymentDetails
        //                    .Where(pd => pd.PaymentDetailId== pc.BalanceAccountId)
        //                    .ToList();

        //                dbContext.PartyPaymentDetails.RemoveRange(paymentDetails);
        //            }

        //            dbContext.SaveChanges();

                    
        //            var allPartyPaymentDetails = PartyPaymentDetail.Select(pc => pc.PaymentDetailId).ToList();
        //            var packageBookingDetails = dbContext.PartyPaymentDetails
        //                .Where(pbd => allPartyPaymentDetails.Contains(pbd.PaymentDetailId))
        //                .ToList();

        //            dbContext.PackageBookingDetails.RemoveRange(packageBookingDetails);
        //            dbContext.SaveChanges();

        //            // Delete PaymentCollections
        //            dbContext.PaymentCollections.RemoveRange(paymentCollections);
        //            dbContext.SaveChanges();

        //            // Finally, delete the Surgery
        //            dbContext.Surgeries.Remove(surgery);
        //            dbContext.SaveChanges();

        //            transaction.Commit();
        //            response.Message = ConstantData.SuccessMessage;
        //            response.message = "Surgery and related data deleted successfully.";
        //        }
        //        catch (DbEntityValidationException ex)
        //        {
        //            transaction.Rollback();

        //            var errorMessages = ex.EntityValidationErrors
        //                .SelectMany(x => x.ValidationErrors)
        //                .Select(x => $"Property: {x.PropertyName}, Error: {x.ErrorMessage}");

        //            string fullError = string.Join("; ", errorMessages);
        //            response.Message = fullError;
        //        }
        //        catch (Exception ex)
        //        {
        //            transaction.Rollback();
        //            response.Message = "An error occurred: " + ex.Message;
        //        }
        //    }

        //    return response;
        //}



    }
}
