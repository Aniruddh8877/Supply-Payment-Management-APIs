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
    [RoutePrefix("api/Dashboard")]
    public class DashboardController : ApiController
    {
        [HttpPost]
        [Route("DashboardSummary")]
        public ExpandoObject GetDashboardSummary(RequestModel requestModel)
        {
            dynamic res = new ExpandoObject();
            try
            {
                PartyManageEntities dbContext = new PartyManageEntities();
                string AppKey = HttpContext.Current.Request.Headers["AppKey"];
                AppData.CheckAppKey(dbContext, AppKey, (byte)KeyFor.Admin);

                var today = DateTime.Today;
                var weekStart = today.AddDays(-(int)today.DayOfWeek);
                var monthStart = new DateTime(today.Year, today.Month, 1);

                var todaySupply = dbContext.PartySupplyItems.Where(x => x.SupplyDate == today).Sum(x => (decimal?)x.Amount) ?? 0;
                var todayPayment = dbContext.PartyPayments.Where(x => x.PaymentDate == today).Sum(x => (decimal?)x.Amount) ?? 0;

                var weeklySupply = dbContext.PartySupplyItems.Where(x => x.SupplyDate >= weekStart && x.SupplyDate <= today).Sum(x => (decimal?)x.Amount) ?? 0;
                var weeklyPayment = dbContext.PartyPayments.Where(x => x.PaymentDate >= weekStart && x.PaymentDate <= today).Sum(x => (decimal?)x.Amount) ?? 0;

                var monthlySupply = dbContext.PartySupplyItems.Where(x => x.SupplyDate >= monthStart && x.SupplyDate <= today).Sum(x => (decimal?)x.Amount) ?? 0;
                var monthlyPayment = dbContext.PartyPayments.Where(x => x.PaymentDate >= monthStart && x.PaymentDate <= today).Sum(x => (decimal?)x.Amount) ?? 0;

                res.TodaySupply = todaySupply;
                res.TodayPayment = todayPayment;
                res.WeeklySupply = weeklySupply;
                res.WeeklyPayment = weeklyPayment;
                res.MonthlySupply = monthlySupply;
                res.MonthlyPayment = monthlyPayment;
                res.Message = ConstantData.SuccessMessage;
            }
            catch (Exception ex)
            {
                res.Message = $"Error: {ex.Message}";
            }

            return res;
        }



    }
}
