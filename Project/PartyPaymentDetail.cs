//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Project
{
    using System;
    using System.Collections.Generic;
    
    public partial class PartyPaymentDetail
    {
        public int PaymentDetailId { get; set; }
        public int? LocationId { get; set; }
        public int PartyId { get; set; }
        public System.DateTime PaymentDate { get; set; }
        public Nullable<int> PartySupplyItemId { get; set; }
        public string Particular { get; set; }
        public string Remarks { get; set; }
        public decimal DebitAmount { get; set; }
        public decimal CreditAmount { get; set; }
        public Nullable<int> PartyPaymentId { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public Nullable<int> BalanceAccountId { get; set; }
    
        public virtual BalanceAccount BalanceAccount { get; set; }
        public virtual PartyDetail PartyDetail { get; set; }
        public virtual PartySupplyItem PartySupplyItem { get; set; }
        public virtual StaffLogin StaffLogin { get; set; }
        public virtual StaffLogin StaffLogin1 { get; set; }
        public virtual PartyPayment PartyPayment { get; set; }
    }
}
