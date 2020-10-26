using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PinnaRent.Core.Models;

namespace PinnaRent.Core
{
    public class CommonFieldsF : CommonFieldsA
    {
        [MaxLength(255, ErrorMessage = "Name Exceeded 255 letters")]
        [DisplayName("Name")]
        [Required]
        public string DisplayName
        {
            get { return GetValue(() => DisplayName); }
            set { SetValue(() => DisplayName, value); }
        }

        //[Index(IsUnique = true)]
        [MaxLength(50, ErrorMessage = "Number exceeded 50 letters")]
        [DisplayName("Registration Number")]
        public string Number
        {
            get { return GetValue(() => Number); }
            set { SetValue(() => Number, value); }
        }

        [MaxLength(10, ErrorMessage = "TinNumber Exceeded 10 letters")]
        [DisplayName("TinNumber")]
        public string TinNumber
        {
            get { return GetValue(() => TinNumber); }
            set { SetValue(() => TinNumber, value); }
        }

        [MaxLength(10, ErrorMessage = "VatNumber Exceeded 10 letters")]
        [DisplayName("VatNumber")]
        public string VatNumber
        {
            get { return GetValue(() => VatNumber); }
            set { SetValue(() => VatNumber, value); }
        }
        
        [DisplayName("Description")]
        public string Description
        {
            get { return GetValue(() => Description); }
            set { SetValue(() => Description, value); }
        }

        [MaxLength(255, ErrorMessage = "Contact Name Exceeded 255 letters")]
        [DisplayName("Contact Name")]
        public string ContactName
        {
            get { return GetValue(() => ContactName); }
            set { SetValue(() => ContactName, value); }
        }

        [MaxLength(255, ErrorMessage = "Type of Work Exceeded 255 letters")]
        [DisplayName("Type of Work")]
        public string TypeOfWork
        {
            get { return GetValue(() => TypeOfWork); }
            set { SetValue(() => TypeOfWork, value); }
        }
        [ForeignKey("Address")]
        public int? AddressId { get; set; }
        public AddressDTO Address
        {
            get { return GetValue(() => Address); }
            set { SetValue(() => Address, value); }
        }

        [NotMapped]
        public string RenteeDetail
        {
            get
            {
                var clDet = DisplayName;
                if (!string.IsNullOrEmpty(TinNumber))
                    clDet = clDet + " " + TinNumber;
                if (Address != null)
                    clDet = clDet + " " + Address.Mobile;
                return string.IsNullOrWhiteSpace(clDet) ? "" : clDet;
            }
            set { SetValue(() => RenteeDetail, value); }
        }
    }
}