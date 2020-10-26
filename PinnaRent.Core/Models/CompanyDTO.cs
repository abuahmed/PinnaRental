using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PinnaRent.Core.Enumerations;

namespace PinnaRent.Core.Models
{
    public class CompanyDTO : CommonFieldsE
    {
        public CompanyTypes Type
        {
            get { return GetValue(() => Type); }
            set { SetValue(() => Type, value); }
        }

        [ForeignKey("CompanyAddress")]
        public int? CompanyAddressId { get; set; }
        public AddressDTO CompanyAddress
        {
            get { return GetValue(() => CompanyAddress); }
            set { SetValue(() => CompanyAddress, value); }
        }

        [ForeignKey("Title")]
        public int? TitleId { get; set; }
        public CategoryDTO Title
        {
            get { return GetValue(() => Title); }
            set { SetValue(() => Title, value); }
        }

        //if Type is Org otherwise is empty
        public string ManagerName
        {
            get { return GetValue(() => ManagerName); }
            set { SetValue(() => ManagerName, value); }
        }

        //Owner Or Manager Personal Address
        [ForeignKey("PersonalAddress")]
        public int? PersonalAddressId { get; set; }
        public AddressDTO PersonalAddress
        {
            get { return GetValue(() => PersonalAddress); }
            set { SetValue(() => PersonalAddress, value); }
        }

        [ForeignKey("Representee")]
        public int? RepresenteeId { get; set; }
        public RepresenteeDTO Representee
        {
            get { return GetValue(() => Representee); }
            set { SetValue(() => Representee, value); }
        }

        [MaxLength(10, ErrorMessage = "Name Exceeded 10 letters")]
        [DisplayName("TinNumber")]
        public string TinNumber
        {
            get { return GetValue(() => TinNumber); }
            set { SetValue(() => TinNumber, value); }
        }
        
        [MaxLength(10, ErrorMessage = "Name Exceeded 10 letters")]
        [DisplayName("VatNumber")]
        public string VatNumber
        {
            get { return GetValue(() => VatNumber); }
            set { SetValue(() => VatNumber, value); }
        }

        public string TotalSqrFeet
        {
            get { return GetValue(() => TotalSqrFeet); }
            set { SetValue(() => TotalSqrFeet, value); }
        }

        public string PlotNumber
        {
            get { return GetValue(() => PlotNumber); }
            set { SetValue(() => PlotNumber, value); }
        }

        public bool HadRepresentee
        {
            get { return GetValue(() => HadRepresentee); }
            set { SetValue(() => HadRepresentee, value); }
        }

        #region Additional Properties
        
        public string TradeName
        {
            get { return GetValue(() => TradeName); }
            set { SetValue(() => TradeName, value); }
        }
        
        public string BusinessAddressPrevious
        {
            get { return GetValue(() => BusinessAddressPrevious); }
            set { SetValue(() => BusinessAddressPrevious, value); }
        }
        
        public string OtherOwnerName
        {
            get { return GetValue(() => OtherOwnerName); }
            set { SetValue(() => OtherOwnerName, value); }
        }

        public string OtherOwnerCountry
        {
            get { return GetValue(() => OtherOwnerCountry); }
            set { SetValue(() => OtherOwnerCountry, value); }
        }

        #endregion

        public ICollection<BankAccountDTO> BankAccounts
        {
            get { return GetValue(() => BankAccounts); }
            set { SetValue(() => BankAccounts, value); }
        }
    }
}
