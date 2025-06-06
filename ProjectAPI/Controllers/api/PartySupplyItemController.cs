﻿using System;
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
    [RoutePrefix("api/PartySupplyItem")]
    public class PartySupplyItemSupplyItemController : ApiController
    {
        [HttpPost]
        [Route("PartySupplyItemList")]
        public ExpandoObject PartySupplyItemList(RequestModel requestModel)
        {
            dynamic response = new ExpandoObject();
            try
            {
                PartyManageEntities dbContext = new PartyManageEntities();
                string AppKey = HttpContext.Current.Request.Headers["AppKey"];
                AppData.CheckAppKey(dbContext, AppKey, (byte)KeyFor.Admin);
                var decryptData = CryptoJs.Decrypt(requestModel.request, CryptoJs.key, CryptoJs.iv);
                PartySupplyItem model = JsonConvert.DeserializeObject<PartySupplyItem>(decryptData);

                var list = (from d1 in dbContext.PartySupplyItems
                            select new
                            {
                                d1.PartySupplyItemId,
                                d1.SupplyDate,
                                d1.PartyId,
                                d1.PartyDetail.PartyName,
                                d1.InvoiceNo,
                                d1.Particular,
                                d1.Attachement,
                                d1.FileFormat,
                                d1.Amount,
                                d1.Remarks,
                                d1.Status,

                            }).ToList();


                response.PartySupplyItemList = list;
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpPost]
        [Route("SavePartySupplyItem")]
        public ExpandoObject SavePartySupplyItem(RequestModel requestModel)
        {

            dynamic response = new ExpandoObject();
            PartyManageEntities dbContext = new PartyManageEntities();
            //using (var dbContext = new PartyManageEntities())
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    string AppKey = HttpContext.Current.Request.Headers["AppKey"];
                    AppData.CheckAppKey(dbContext, AppKey, (byte)KeyFor.Admin);
                    var decryptData = CryptoJs.Decrypt(requestModel.request, CryptoJs.key, CryptoJs.iv);
                    PartySupplyItemModel model = JsonConvert.DeserializeObject<PartySupplyItemModel>(decryptData);

                    PartySupplyItem partySupplyItem;
                    if (model.GetPartySupplyItem.PartySupplyItemId > 0)
                    {
                        partySupplyItem = dbContext.PartySupplyItems.First(x => x.PartySupplyItemId == model.GetPartySupplyItem.PartySupplyItemId);
                        if (partySupplyItem == null)
                        {
                            response.Message = "details not found";
                            return response;
                        }
                        partySupplyItem.SupplyDate = model.GetPartySupplyItem.SupplyDate;
                        partySupplyItem.PartyId = model.GetPartySupplyItem.PartyId;
                        partySupplyItem.InvoiceNo = model.GetPartySupplyItem.InvoiceNo;
                        partySupplyItem.Particular = model.GetPartySupplyItem.Particular;
                        partySupplyItem.Amount = model.GetPartySupplyItem.Amount;
                        partySupplyItem.Remarks = model.GetPartySupplyItem.Remarks;
                        partySupplyItem.Attachement = model.GetPartySupplyItem.Attachement;
                        partySupplyItem.FileFormat = model.GetPartySupplyItem.FileFormat;
                        partySupplyItem.Status = model.GetPartySupplyItem.Status;
                        partySupplyItem.UpdatedBy = model.GetPartySupplyItem.UpdatedBy;
                        partySupplyItem.UpdatedOn = DateTime.Now;
                    }
                    else
                    {
                        partySupplyItem = new PartySupplyItem
                        {
                            SupplyDate = model.GetPartySupplyItem.SupplyDate,
                            PartyId = model.GetPartySupplyItem.PartyId,
                            InvoiceNo = model.GetPartySupplyItem.InvoiceNo,
                            Particular = model.GetPartySupplyItem.Particular,
                            Attachement = model.GetPartySupplyItem.Attachement,
                            FileFormat = model.GetPartySupplyItem.FileFormat,
                            Amount = model.GetPartySupplyItem.Amount,
                            Status = model.GetPartySupplyItem.Status,
                            CreatedBy = model.GetPartySupplyItem.CreatedBy,
                            Remarks = model.GetPartySupplyItem.Remarks,
                            CreatedOn = DateTime.Now,

                        };
                        dbContext.PartySupplyItems.Add(partySupplyItem);
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
                        partyPaymentDetail.PaymentDate = partySupplyItem.SupplyDate;
                        partyPaymentDetail.Particular = partySupplyItem.Particular;
                        partyPaymentDetail.DebitAmount = partySupplyItem.Amount;
                        partyPaymentDetail.Remarks = partySupplyItem.Remarks;
                        //partyPaymentDetail. = model.GetPartyPaymentCollection.Status;
                        partyPaymentDetail.UpdatedBy = partySupplyItem.UpdatedBy;
                        partyPaymentDetail.UpdatedOn = DateTime.Now;
                    }
                    else
                    {
                        partyPaymentDetail = new PartyPaymentDetail
                        {
                            PartyId = model.GetPartyPaymentDetail.PartyId,
                            PaymentDate = partySupplyItem.SupplyDate,
                            //Particular = model.GetPartyPaymentDetail.Particular,
                            DebitAmount = partySupplyItem.Amount,
                            PartySupplyItemId = partySupplyItem.PartySupplyItemId,
                            //PartyPaymentId = partySupplyItem.PartyPaymentCollectionId,
                            //PartySupplyItemId = model.GetPartySupplyItem.PartySupplyItemId,
                            Remarks = partySupplyItem.Remarks,
                            CreatedBy = partySupplyItem.CreatedBy,
                            CreatedOn = DateTime.Now,
                            Particular = "Supply Item ",
                        };
                        dbContext.PartyPaymentDetails.Add(partyPaymentDetail);
                    }
                    dbContext.SaveChanges();
                    transaction.Commit();

                    response.Message = ConstantData.SuccessMessage;
                    response.PartySupplyItemId = partySupplyItem.PartySupplyItemId;
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
        [Route("DeletePartySupplyItem")]
        public ExpandoObject DeletePartySupplyItem(RequestModel requestModel)
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
                PartySupplyItem model = JsonConvert.DeserializeObject<PartySupplyItem>(decryptData);
                var PartySupplyItemDetail = dataContext.PartySupplyItems.FirstOrDefault(x => x.PartySupplyItemId == model.PartySupplyItemId);
                if (PartySupplyItemDetail == null)
                {
                    res.Message = "Patient not found";
                    return res;
                }

                dataContext.PartySupplyItems.Remove(PartySupplyItemDetail);
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
