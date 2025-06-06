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
    
    public partial class PartyDetail
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PartyDetail()
        {
            this.PartyPaymentDetails = new HashSet<PartyPaymentDetail>();
            this.PartySupplyItems = new HashSet<PartySupplyItem>();
            this.PartyPayments = new HashSet<PartyPayment>();
        }
    
        public int PartyId { get; set; }
        public string PartyCode { get; set; }
        public string PartyName { get; set; }
        public string MobileNo { get; set; }
        public string AlternateMobileNo { get; set; }
        public string Email { get; set; }
        public int LocationId { get; set; }
        public string Address { get; set; }
        public string GSTNo { get; set; }
        public byte PartyStatus { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
    
        public virtual Location Location { get; set; }
        public virtual StaffLogin StaffLogin { get; set; }
        public virtual StaffLogin StaffLogin1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PartyPaymentDetail> PartyPaymentDetails { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PartySupplyItem> PartySupplyItems { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PartyPayment> PartyPayments { get; set; }
    }
}
