using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Project;
using Project.DataSet;

namespace ProjectAPI.Controllers
{
    public class ReportController : Controller
    {
        public byte[] GetImageAsByteArray(string imagePath)
        {
            return System.IO.File.ReadAllBytes(imagePath);
        }


        public ActionResult PartyLadgerHistory(string id)
        {
            try
            {
                PartyManageEntities dataContext = new PartyManageEntities();
                PartyLadgerHistory plh = new PartyLadgerHistory();

                // 1. Company detail
                DataTable company = plh.CompanyDetail;
                var c = dataContext.Companies.Select(co => new
                {
                    co.CompanyName,
                    co.CompanyAddress,
                    co.CityName,
                    co.StateName,
                    co.PinCode,
                    co.Email,
                    co.MobileNo,
                    co.Logo
                }).First();

                DataRow cRow = company.NewRow();
                cRow["CompanyName"] = c.CompanyName;
                cRow["FullAddress"] = c.CompanyAddress;
                cRow["City"] = c.CityName;
                cRow["State"] = c.StateName;
                cRow["PinCode"] = c.PinCode;
                cRow["Email"] = c.Email ?? "";
                cRow["MobileNo"] = "+91 " + c.MobileNo;
                cRow["Logo"] = GetImageAsByteArray(Server.MapPath(c.Logo));
                company.Rows.Add(cRow);

                // 2. Party detail
                int PartyId = Convert.ToInt32(id);
                DataTable Party = plh.PartyDetail;

                var partyInfo = (from p1 in dataContext.PartyPaymentDetails
                                 join a1 in dataContext.PartyDetails on p1.PartyId equals a1.PartyId
                                 where p1.PartyId == PartyId
                                 select a1).FirstOrDefault();

                DataRow bRow = Party.NewRow();
                bRow["PartyName"] = partyInfo.PartyName;
                bRow["PatryCode"] = partyInfo.PartyCode;
                bRow["MobileNo"] = partyInfo.MobileNo;
                bRow["AlterMobileNo"] = partyInfo.AlternateMobileNo ?? "";
                bRow["Email"] = partyInfo.Email ?? "";
                bRow["Location"] = partyInfo.Location.LocationName;
                bRow["Address"] = partyInfo.Address;
                bRow["GSTNo"] = partyInfo.GSTNo ?? "";

                // 3. Payment history
                DataTable Payment = plh.PartyLadgeHistory;

               var chargeDetail = (from d1 in dataContext.PartyPaymentDetails
                                    join p1 in dataContext.PartyDetails on d1.PartyId equals p1.PartyId
                                    join o in dataContext.PartyPayments on d1.PartyId equals o.PartyId
                                    where o.PartyId == PartyId
                                    select new
                                    {
                                        d1.CreditAmount,
                                        d1.DebitAmount,
                                        PaymentDate = (DateTime?)o.PaymentDate,
                                        d1.Particular,
                                        o.PaymentMode
                                    }).ToList();

                var charge = chargeDetail.Select(x => new
                {
                    x.CreditAmount,
                    x.DebitAmount,
                    x.PaymentDate,
                    x.Particular,
                    PaymentModeName = Enum.GetName(typeof(PaymentMode), x.PaymentMode),
                }).ToList();

                var CreditSum = charge.Sum(x => x.CreditAmount);
                var DebitSum = charge.Sum(x => x.DebitAmount);

                foreach (var item in charge)
                {

                    DataRow pRow = Payment.NewRow();
                    pRow["PaymentDate"] = item.PaymentDate?.ToString("dd-MM-yyyy") ?? "N/A";
                    pRow["Particular"] = item.Particular;
                    pRow["PaymentMode"] = item.PaymentModeName != null ? item.PaymentModeName : "";
                    pRow["CreditAmount"] = item.CreditAmount;
                    pRow["DebitAmount"] = item.DebitAmount;
                    //pRow["Credits"] = CreditSum;
                    //pRow["Debits"] = DebitSum;
                    pRow["BalanceAmount"] = item.DebitAmount - item.CreditAmount;
                    Payment.Rows.Add(pRow);
                }
                var Total = DebitSum - CreditSum;
                bRow["Total"] = Total;
                bRow["Credits"] = CreditSum;
                bRow["Debits"] = DebitSum;
                Party.Rows.Add(bRow);

                ReportDocument rp = new ReportDocument();
                string reportPath = Server.MapPath("~/Reports/Balance.rpt");
                rp.Load(reportPath);
                rp.SetDataSource(plh);

                System.IO.Stream ms = rp.ExportToStream(ExportFormatType.PortableDocFormat);
                if (rp != null)
                {
                    rp.Close();
                    rp.Dispose();
                    GC.Collect();
                }
                Response.Clear();
                Response.Buffer = true;
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-length", ms.Length.ToString());
                Response.BinaryWrite(AppData.ReadFully(ms));
                Response.End();
            }
            catch (Exception ex)
            {
                return Content("Error: " + ex.Message + "\n\nStackTrace:\n" + ex.StackTrace);
            }
            return View();
        }




    }
}