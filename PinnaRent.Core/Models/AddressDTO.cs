#region

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#endregion

namespace PinnaRent.Core.Models
{
    public class AddressDTO : CommonFieldsA
    {
        [Display(Name = "Detail Address")]
        public string AddressDetail
        {
            get { return GetValue(() => AddressDetail); }
            set { SetValue(() => AddressDetail, value); }
        }

        [DataType(DataType.PhoneNumber)]
        [RegularExpression("^[0-9]{10,14}$", ErrorMessage = "Mobile Number is invalid")]
        public string Mobile
        {
            get { return GetValue(() => Mobile); }
            set { SetValue(() => Mobile, value); }
        }

        [DataType(DataType.PhoneNumber)]
        [RegularExpression("^[0-9]{10,14}$", ErrorMessage = "Alternate Mobile Number is invalid")]
        public string AlternateMobile
        {
            get { return GetValue(() => AlternateMobile); }
            set { SetValue(() => AlternateMobile, value); }
        }

        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Fixed Line Telephone")]
        [RegularExpression("^[0-9]{10,14}$", ErrorMessage = "Telephone is invalid")]
        public string Telephone
        {
            get { return GetValue(() => Telephone); }
            set { SetValue(() => Telephone, value); }
        }

        [DataType(DataType.EmailAddress)]
        [MaxLength(30, ErrorMessage = "Exceeded 30 letters")]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "E-mail is invalid")]
        [Display(Name = "Primary Email")]
        public string PrimaryEmail
        {
            get { return GetValue(() => PrimaryEmail); }
            set { SetValue(() => PrimaryEmail, value); }
        }

        [DataType(DataType.EmailAddress)]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "E-mail is invalid")]
        [Display(Name = "Alternate Email")]
        public string AlternateEmail
        {
            get { return GetValue(() => AlternateEmail); }
            set { SetValue(() => AlternateEmail, value); }
        }

        [MaxLength(30, ErrorMessage = "Exceeded 30 letters")]
        public string Country
        {
            get { return GetValue(() => Country); }
            set { SetValue(() => Country, value); SetValue(() => AddressDescription, value); }
        }

        [MaxLength(30, ErrorMessage = "Exceeded 30 letters")]
        public string Region
        {
            get { return GetValue(() => Region); }
            set { SetValue(() => Region, value); SetValue(() => AddressDescription, value); }
        }

        [MaxLength(30, ErrorMessage = "Exceeded 30 letters")]
        public string City
        {
            get { return GetValue(() => City); }
            set { SetValue(() => City, value); SetValue(() => AddressDescription, value); }
        }

        [MaxLength(30, ErrorMessage = "Exceeded 30 letters")]
        [Display(Name = "Sub City")]
        public string SubCity
        {
            get { return GetValue(() => SubCity); }
            set { SetValue(() => SubCity, value); SetValue(() => AddressDescription, value); }
        }

        [MaxLength(30, ErrorMessage = "Exceeded 30 letters")]
        public string Woreda
        {
            get { return GetValue(() => Woreda); }
            set
            {
                SetValue(() => Woreda, value);
                SetValue(() => AddressDescription, value);
            }
        }

        [MaxLength(30, ErrorMessage = "Exceeded 30 letters")]
        public string Kebele
        {
            get { return GetValue(() => Kebele); }
            set
            {
                SetValue(() => Kebele, value);
                SetValue(() => AddressDescription, value);
            }
        }

        [MaxLength(30, ErrorMessage = "Exceeded 30 letters")]
        [Display(Name = "House Number")]
        public string HouseNumber
        {
            get { return GetValue(() => HouseNumber); }
            set
            {
                SetValue(() => HouseNumber, value);
                SetValue(() => AddressDescription, value); 
            }
        }

        [MaxLength(30, ErrorMessage = "Exceeded 30 letters")]
        [Display(Name = "P.O.Box")]
        public string PoBox
        {
            get { return GetValue(() => PoBox); }
            set { SetValue(() => PoBox, value); }
        }

        [RegularExpression("^[0-9]{8,14}$", ErrorMessage = "Fax is invalid")]
        public string Fax
        {
            get { return GetValue(() => Fax); }
            set { SetValue(() => Fax, value); }
        }

        [StringLength(20)]
        [RegularExpression("^[A-Z0-9]{9,10}$", ErrorMessage = "Passport Number is invalid")]
        public string PassportNumber
        {
            get { return GetValue(() => PassportNumber); }
            set { SetValue(() => PassportNumber, value); }
        }

        public string DrivingLicenseNumber
        {
            get { return GetValue(() => DrivingLicenseNumber); }
            set { SetValue(() => DrivingLicenseNumber, value); }
        }

        [NotMapped]
        public string AddressDescription
        {
            get
            {
                string desc = AddressDetail + " " + " " + Mobile + " " + SubCity + " " + Woreda +Environment.NewLine+ " " + Kebele + " " + HouseNumber + "  " + City + " " + Country;
                return desc;
            }
            set { SetValue(() => AddressDescription, value); }
        }
    }
}