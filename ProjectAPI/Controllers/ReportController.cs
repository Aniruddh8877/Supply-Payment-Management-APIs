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

                DataTable company = plh.CompanyDetail;
                var c = (from co in dataContext.Companies
                         select new
                         {
                             co.CompanyName,
                             co.CompanyAddress,
                             co.CityName,
                             co.DistrictName,
                             co.StateName,
                             co.PinCode,
                             co.Email,
                             co.MobileNo,
                             co.AlternateNo,
                             co.Website,
                             co.Logo

                         }).First();

                DataRow cRow = company.NewRow();
                cRow["CompanyName"] = c.CompanyName;
                cRow["FullAddress"] = c.CompanyAddress;
                cRow["City"] = c.CityName;
                cRow["State"] = c.StateName;
                cRow["PinCode"] = c.PinCode;
                cRow["Email"] = c.Email != null ? c.Email : "";
                cRow["MobileNo"] = "+91 " + c.MobileNo;
                string imagePath = Server.MapPath(c.Logo);
                cRow["Logo"] = GetImageAsByteArray(imagePath); // Ensure this is a byte array
                company.Rows.Add(cRow);


                var PartyId = Convert.ToInt32(id);
                DataTable Party = plh.PartyDetail;
                var x = (from p1 in dataContext.PartyPaymentDetails
                         join a1 in dataContext.PartyDetails on p1.PartyId equals a1.PartyId
                         where p1.PartyId == PartyId
                         select new
                         {
                             a1.PartyName,
                             a1.PartyCode,
                             a1.MobileNo,
                             a1.AlternateMobileNo,
                             a1.Email,
                             a1.Location,
                             a1.Address,
                             a1.GSTNo,
                         }).AsEnumerable()
                         .Select(h => new
                         {
                             h.PartyName,
                             h.PartyCode,
                             h.MobileNo,
                             h.AlternateMobileNo,
                             h.Location,
                             h.Address,
                             h.GSTNo,
                             h.Email,
                         }).First();

                DataRow bRow = Party.NewRow();
                bRow["PartyName"] = x.PartyName;
                bRow["PatryCode"] = x.PartyCode;
                bRow["MobileNo"] = x.MobileNo;
                bRow["AlterMobileNo"] = x.AlternateMobileNo ?? "";
                bRow["Email"] = x.Email ?? "";
                bRow["Location"] = x.Location;
                bRow["Address"] = x.Address;
                bRow["GSTNo"] = x.GSTNo ?? "";
                Party.Rows.Add(bRow);


                DataTable Payment = plh.PartyLadgeHistory;
                var charge = (from d1 in dataContext.PartyPaymentDetails
                              join p1 in dataContext.PartyDetails on d1.PartyId equals p1.PartyId
                              join o in dataContext.PartyPayments on d1.PartyId equals o.PartyId
                              where o.PartyId == PartyId
                              select new
                              {
                                  d1.CreditAmount,
                                  d1.DebitAmount,
                                  d1.PaymentDate,
                                  d1.Particular,
                                  o.PaymentMode,

                              }).ToList();


                   
                foreach (var item in charge)
                {

               

                    DataRow pRow = Payment.NewRow();
                    pRow["PaymentDate"] = item.PaymentDate;
                    pRow["Particular"] = item.Particular;
                    pRow["PaymentMode"] = item.PaymentMode ?? "";
                    pRow["CreditAmount"] = item.CreditAmount ;
                    pRow["DebitAmount"] = item.DebitAmount;
                    pRow["Credits"] =+ item.CreditAmount;
                    pRow["Debits"] =+ item.DebitAmount;
                    pRow["BalanceAmount"] = + item.DebitAmount - item.CreditAmount;

                    Payment.Rows.Add(pRow);
                }

                ReportDocument rp = new ReportDocument();
                rp.Load(Server.MapPath("~/Reports/InvoiceBill.rpt"));
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
                ViewBag.Message = ex.Message;
            }
            return View();
        }
    }
}