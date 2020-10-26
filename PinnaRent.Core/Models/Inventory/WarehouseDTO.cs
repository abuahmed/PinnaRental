using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PinnaRent.Core.Enumerations;

namespace PinnaRent.Core.Models
{
    public class WarehouseDTO : CommonFieldsE
    {
        public WarehouseDTO() 
        {
            Transactions = new List<TransactionHeaderDTO>();
        }

        public WarehouseTypes Type
        {
            get { return GetValue(() => Type); }
            set { SetValue(() => Type, value); }
        }

        [Key]
        [Column(Order = 1)]
        public int WarehouseNumber
        {
            get { return GetValue(() => WarehouseNumber); }
            set { SetValue(() => WarehouseNumber, value); }
        }

        [ForeignKey("Company")]
        public int CompanyId { get; set; }
        public CompanyDTO Company
        {
            get { return GetValue(() => Company); }
            set { SetValue(() => Company, value); }
        }

        public bool IsDefault//also "IS HeadQuarter"
        {
            get { return GetValue(() => IsDefault); }
            set { SetValue(() => IsDefault, value); }
        }

        [NotMapped]
        public string IsDefaultYesNo
        {
            get
            {
                return IsDefault ? "Yes" : "No";
            }
        }

        [NotMapped]
        public string DisplayNameShort
        {
            get { return DisplayName != null && DisplayName.Length > 18 ? DisplayName.Substring(0, 15) + "..." : DisplayName; }
            set { SetValue(() => DisplayNameShort, value); }
        }

        public ICollection<TransactionHeaderDTO> Transactions
        {
            get { return GetValue(() => Transactions); }
            set { SetValue(() => Transactions, value); }
        }
   
    }
}
