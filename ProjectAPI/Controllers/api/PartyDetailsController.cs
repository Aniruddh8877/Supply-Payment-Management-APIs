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

namespace ProjectAPI.Controllers.api
{
    [RoutePrefix("api/Party")]
    public class PartyDetailsController : ApiController
    {
        [HttpPost]
        [Route("PartyList")]
        public ExpandoObject PartyList(RequestModel requestModel)
        {
            dynamic response = new ExpandoObject();
            try
            {
                PartyManageEntities dbContext = new PartyManageEntities();
                string AppKey = HttpContext.Current.Request.Headers["AppKey"];
                AppData.CheckAppKey(dbContext, AppKey, (byte)KeyFor.Admin);
                var decryptData = CryptoJs.Decrypt(requestModel.request, CryptoJs.key, CryptoJs.iv);
                PartyDetail model = JsonConvert.DeserializeObject<PartyDetail>(decryptData);

                var list = (from d1 in dbContext.PartyDetails
                            where model.PartyId == 0 || d1.PartyId == model.PartyId
                            join loc in dbContext.Locations
                            on d1.LocationId equals loc.LocationId into locGroup
                            from loc in locGroup.DefaultIfEmpty()
                            select new
                            {
                                d1.PartyId,
                                d1.PartyCode,
                                d1.PartyName,
                                d1.MobileNo,
                                d1.AlternateMobileNo,
                                d1.Email,
                                d1.LocationId,
                                LocationName = loc != null ? loc.LocationName : null, 
                                d1.Address,
                                d1.GSTNo,
                                d1.PartyStatus,
                                d1.CreatedBy,
                                d1.CreatedOn,
                                d1.UpdatedBy,
                                d1.UpdatedOn,
                            }).ToList();


                response.PartyList = list;
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpPost]
        [Route("SaveParty")]
        public ExpandoObject SaveParty(RequestModel requestModel)
        {

            dynamic response = new ExpandoObject();
            try
            {
                PartyManageEntities dbContext = new PartyManageEntities();
                string AppKey = HttpContext.Current.Request.Headers["AppKey"];
                AppData.CheckAppKey(dbContext, AppKey, (byte)KeyFor.Admin);
                var decryptData = CryptoJs.Decrypt(requestModel.request, CryptoJs.key, CryptoJs.iv);
                PartyDetail model = JsonConvert.DeserializeObject<PartyDetail>(decryptData);

                PartyDetail partyDetail = null;
                if (model.PartyId > 0)
                {
                    partyDetail = dbContext.PartyDetails.Where(x => x.PartyId == model.PartyId).First();
                    if (partyDetail == null)
                    {
                        response.Message = "Party Details not found";
                        return response;
                    }
                    partyDetail.PartyName = model.PartyName;
                    partyDetail.PartyCode = model.PartyCode;
                    partyDetail.MobileNo = model.MobileNo;
                    partyDetail.AlternateMobileNo = model.AlternateMobileNo;
                    partyDetail.Email = model.Email;
                    partyDetail.Address = model.Address;
                    partyDetail.LocationId = model.LocationId;
                    partyDetail.GSTNo = model.GSTNo;
                    partyDetail.PartyStatus = model.PartyStatus;
                    partyDetail.UpdatedBy = model.UpdatedBy;
                    partyDetail.UpdatedOn = DateTime.Now;
                    
                }
                else
                {
                    partyDetail = new PartyDetail();
                    
                    partyDetail.PartyName = model.PartyName;
                    partyDetail.PartyCode = AppData.GeneratePartyCode(dbContext);
                    partyDetail.MobileNo = model.MobileNo;
                    partyDetail.AlternateMobileNo = model.AlternateMobileNo;
                    partyDetail.Address = model.Address;
                    partyDetail.Email = model.Email;
                    partyDetail.LocationId = model.LocationId;
                    partyDetail.GSTNo = model.GSTNo;
                    partyDetail.PartyStatus = model.PartyStatus;
                    partyDetail.CreatedBy = model.CreatedBy;
                    partyDetail.CreatedOn = DateTime.Now;
                     
                    dbContext.PartyDetails.Add(partyDetail);
                }
               

                dbContext.SaveChanges();
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    response.Message = ex.InnerException.InnerException != null
                        ? ex.InnerException.InnerException.Message
                        : ex.InnerException.Message;
                }
                else if (ex.Message.Contains("IX"))
                {
                    response.Message = "This record already exists";
                }
                else
                {
                    response.Message = ex.Message;
                }
            }

            return response;
        }


        [HttpPost]
        [Route("DeleteParty")]
        public ExpandoObject DeletePatient(RequestModel requestModel)
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
                PartyDetail model = JsonConvert.DeserializeObject<PartyDetail>(decryptData);
                var partyDetail = dataContext.PartyDetails.First(x => x.PartyId == model.PartyId);
                if (partyDetail == null)
                {
                    res.Message = "Party not found";
                    return res;
                }


                dataContext.PartyDetails.Remove(partyDetail);
                dataContext.SaveChanges();
                res.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
               
                res.Message = "Records Present to this Party so can't detele this Party";
                
            }

            return res;
        }

    }
}
