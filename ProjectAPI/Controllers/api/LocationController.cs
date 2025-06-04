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

namespace ProjectAPI.Controllers.api
{
    [RoutePrefix("api/Location")]
    public class LocationController : ApiController
    {
        [HttpPost]
        [Route("LocationList")]
        public ExpandoObject LocationList(RequestModel requestModel)
        {
            dynamic response = new ExpandoObject();
            try
            {
                PartyManageEntities dbContext = new PartyManageEntities();
                string AppKey = HttpContext.Current.Request.Headers["AppKey"];
                AppData.CheckAppKey(dbContext, AppKey, (byte)KeyFor.Admin);
                var decryptData = CryptoJs.Decrypt(requestModel.request, CryptoJs.key, CryptoJs.iv);
                Location model = JsonConvert.DeserializeObject<Location>(decryptData);

                var list = (from d1 in dbContext.Locations
                            select new
                            {
                                d1.LocationId,
                                d1.LocationName,
                                d1.Status,
                                //StatusName=Enum.GetName(typeof(Status), d1.Status)

                            }).ToList();

                response.LocationList = list;
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpPost]
        [Route("SaveLocation")]
        public ExpandoObject SaveLocation(RequestModel requestModel)
        {
            dynamic response = new ExpandoObject();
            try
            {
                PartyManageEntities dbContext = new PartyManageEntities();
                string AppKey = HttpContext.Current.Request.Headers["AppKey"];
                AppData.CheckAppKey(dbContext, AppKey, (byte)KeyFor.Admin);
                var decryptData = CryptoJs.Decrypt(requestModel.request, CryptoJs.key, CryptoJs.iv);
                Location model = JsonConvert.DeserializeObject<Location>(decryptData);

                Location location = null;
                if (model.LocationId > 0)
                {
                    location = dbContext.Locations.Where(x => x.LocationId == model.LocationId).First();
                    if (location == null)
                    {
                        response.Message = "Patient not found";
                        return response;
                    }
                    location.LocationName = model.LocationName;
                    location.Status = model.Status;
                }
                else
                {
                    location = new Location();
                    location.LocationName = model.LocationName;
                    location.Status = model.Status;

                    dbContext.Locations.Add(location);
                }

                dbContext.SaveChanges();
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                if (ex.InnerException is SqlException sqlEx)
                {
                    if (sqlEx.Number == 2601 || sqlEx.Number == 2627)
                    {
                        response.Message = "Location already exists.";
                        return response;
                    }
                }

                response.Message = ex.InnerException?.Message ?? ex.Message;
            }



            return response;
        }


        [HttpPost]
        [Route("DeleteLocation")]
        public ExpandoObject DeleteLocation(RequestModel requestModel)
        {
            dynamic response = new ExpandoObject();
            try
            {
                PartyManageEntities dataContext = new PartyManageEntities();
                string AppKey = HttpContext.Current.Request.Headers["AppKey"];
                if (string.IsNullOrEmpty(AppKey))
                {
                    response.Message = "AppKey is missing";
                    return response;
                }
                AppData.CheckAppKey(dataContext, AppKey, (byte)KeyFor.Admin);
                var decryptData = CryptoJs.Decrypt(requestModel.request, CryptoJs.key, CryptoJs.iv);
                PartyDetail model = JsonConvert.DeserializeObject<PartyDetail>(decryptData);
                var Location = dataContext.Locations.FirstOrDefault(x => x.LocationId == model.LocationId);
                if (Location == null)
                {
                    response.Message = "Location not found";
                    return response;
                }

                dataContext.Locations.Remove(Location);
                dataContext.SaveChanges();
                response.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("FK"))
                {
                    response.Message = "This record is in use. Can't delete.";
                }
                else
                {
                    response.Message = ex.Message;
                }
            }
            return response;
        }

    }
}
