using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project
{
    //opening balance
    public class BalanceAccountModel
    {
        public BalanceAccount GetBalanceAccount { get; set; }

        public PartyPaymentDetail GetPartyPaymentDetail { get; set; }
    }


    //payment
    public class PartyPaymentCollectionModel
    {
        public PartyPaymentCollection GetPartyPaymentCollection { get; set; }

        //public PartyDetail GetPartyDetail { get; set; }

        public PartyPaymentDetail GetPartyPaymentDetail { get; set; }
    }


    //supply model
    public class PartySupplyItemModel
    {
        public PartySupplyItem GetPartySupplyItem { get; set; }
        public PartyPaymentDetail GetPartyPaymentDetail { get; set; }
    }
    public class PartySupplyPaymentModel
    {
        public PartySupplyItem GetPartySupplyPayment { get; set; }

        //public PartyDetail GetPartyDetail { get; set; }

        public PartyPaymentDetail GetPartyPaymentDetail { get; set; }
    }
    public class ChangePasswordModel
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public int Id { get; set; }
    }
    public class RequestModel
    {
        public string request { get; set; }
    }



    public class MenuModel
    {
        public int MenuId { get; set; }
        public int? ParentMenuId { get; set; }
        public int? PageId { get; set; }
        public string PageUrl { get; set; }
        public string MenuTitle { get; set; }
        public int MenuNo { get; set; }
        public string MenuIcon { get; set; }
        public List<MenuModel> MenuList { get; set; }
    }
}
