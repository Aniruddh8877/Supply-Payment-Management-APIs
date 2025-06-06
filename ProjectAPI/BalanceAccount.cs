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
    
    public partial class BalanceAccount
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public BalanceAccount()
        {
            this.PartyPaymentDetails = new HashSet<PartyPaymentDetail>();
        }
    
        public int BalanceAccoountId { get; set; }
        public int PartyId { get; set; }
        public System.DateTime Openingdate { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
    
        public virtual StaffLogin StaffLogin { get; set; }
        public virtual StaffLogin StaffLogin1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PartyPaymentDetail> PartyPaymentDetails { get; set; }
    }
}
