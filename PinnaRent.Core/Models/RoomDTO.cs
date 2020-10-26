using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PinnaRent.Core.Enumerations;

namespace PinnaRent.Core.Models
{
    public class RoomDTO : CommonFieldsA
    {
        [Required]
        [Index(IsUnique = true)]
        [MaxLength(50, ErrorMessage = "Room Number exceeded 50 letters")]
        [DisplayName("Room Number")]
        public string Number
        {
            get { return GetValue(() => Number); }
            set { SetValue(() => Number, value); }
        }

        [ForeignKey("Floor")]
        public int? FloorId { get; set; }
        public CategoryDTO Floor
        {
            get { return GetValue(() => Floor); }
            set { SetValue(() => Floor, value); }
        }
        
        public RoomTypes Type
        {
            get { return GetValue(() => Type); }
            set { SetValue(() => Type, value); }
        }

        public RoomStatus Status
        {
            get { return GetValue(() => Status); }
            set { SetValue(() => Status, value); }
        }

        public RoomServices Service
        {
            get { return GetValue(() => Service); }
            set { SetValue(() => Service, value); }
        }

        public double? SqrFeet
        {
            get { return GetValue(() => SqrFeet); }
            set { SetValue(() => SqrFeet, value); }
        }

        public decimal? SqrFeetAmount
        {
            get { return GetValue(() => SqrFeetAmount); }
            set { SetValue(() => SqrFeetAmount, value); }
        }

        public decimal? RentalFee
        {
            get { return GetValue(() => RentalFee); }
            set { SetValue(() => RentalFee, value); }
        }

        public decimal? ServiceFee
        {
            get { return GetValue(() => ServiceFee); }
            set { SetValue(() => ServiceFee, value); }
        }

        public bool IncluedTax
        {
            get { return GetValue(() => IncluedTax); }
            set { SetValue(() => IncluedTax, value); }
        }

        public bool HaveSeparateServiceCharge
        {
            get { return GetValue(() => HaveSeparateServiceCharge); }
            set { SetValue(() => HaveSeparateServiceCharge, value); }
        }

        public bool HavePenality
        {
            get { return GetValue(() => HavePenality); }
            set { SetValue(() => HavePenality, value); }
        }
        
        //[ForeignKey("LastRentalContrat")]
        //public int? LastRentalContratId { get; set; }
        //public RentalContratDTO LastRentalContrat
        //{
        //    get { return GetValue(() => LastRentalContrat); }
        //    set
        //    {
        //        SetValue(() => LastRentalContrat, value);
        //    }
        //}

        [ForeignKey("LastRentee")]
        public int? LastRenteeId { get; set; }
        public RenteeDTO LastRentee
        {
            get { return GetValue(() => LastRentee); }
            set
            {
                SetValue(() => LastRentee, value);
            }
        }

        [ForeignKey("LastRentalPayment")]
        public int? LastRentalPaymentId { get; set; }
        public RentalPaymentDTO LastRentalPayment
        {
            get { return GetValue(() => LastRentalPayment); }
            set
            {
                SetValue(() => LastRentalPayment, value);
            }
        }

        [ForeignKey("LastServicePayment")]
        public int? LastServicePaymentId { get; set; }
        public RentalPaymentDTO LastServicePayment
        {
            get { return GetValue(() => LastServicePayment); }
            set
            {
                SetValue(() => LastServicePayment, value);
            }
        }

        [ForeignKey("LastRentDeposit")]
        public int? LastRentDepositId { get; set; }
        public RentDepositDTO LastRentDeposit
        {
            get { return GetValue(() => LastRentDeposit); }
            set
            {
                SetValue(() => LastRentDeposit, value);
            }
        }
        
        public ICollection<RoomResourceDTO> Resources
        {
            get { return GetValue(() => Resources); }
            set
            {
                SetValue(() => Resources, value);
            }
        }

        public ICollection<RentalContratDTO> RoomContrats
        {
            get { return GetValue(() => RoomContrats); }
            set
            {
                SetValue(() => RoomContrats, value);
            }
        }

        /** We use the following Properties Mainly For Parameter Purpose**/
        [NotMapped]
        public bool IsArchived
        {
            get { return GetValue(() => IsArchived); }
            set { SetValue(() => IsArchived, value); }
        }

        [NotMapped]
        public PaymentTypes PaymentType
        {
            get { return GetValue(() => PaymentType); }
            set { SetValue(() => PaymentType, value); }
        }

        [NotMapped]
        public int ContractDiscontinuedId
        {
            get { return GetValue(() => ContractDiscontinuedId); }
            set { SetValue(() => ContractDiscontinuedId, value); }
        }

        [NotMapped]
        public DateTime? LastRoomReleasedDate
        {
            get { return GetValue(() => LastRoomReleasedDate); }
            set { SetValue(() => LastRoomReleasedDate, value); }
        }
        /*****************************************************/
        //[NotMapped]
        //public RentalContratDTO LastRentalContratTemp
        //{
        //    get { return GetValue(() => LastRentalContratTemp); }
        //    set { SetValue(() => LastRentalContratTemp, value); }
        //}

        [NotMapped]
        public RentalPaymentDTO LastRentalPaymentTemp
        {
            get { return GetValue(() => LastRentalPaymentTemp); }
            set { SetValue(() => LastRentalPaymentTemp, value); }
        }
        [NotMapped]
        public RentalPaymentDTO LastServicePaymentTemp
        {
            get { return GetValue(() => LastServicePaymentTemp); }
            set { SetValue(() => LastServicePaymentTemp, value); }
        }
        [NotMapped]
        public RentDepositDTO LastRenDepositTemp
        {
            get { return GetValue(() => LastRenDepositTemp); }
            set { SetValue(() => LastRenDepositTemp, value); }
        }

        [NotMapped]
        public string RentalFeeInWordsAmharic
        {
            get { return CommonUtility.GetNumberInWords(RentalFee.ToString(), false); }
            set { SetValue(() => RentalFeeInWordsAmharic, value); }
        }
        [NotMapped]
        public string RentalFeeInWordsEnglish
        {
            get { return CommonUtility.GetNumberInWords(RentalFee.ToString(), true); }
            set { SetValue(() => RentalFeeInWordsEnglish, value); }
        }
        [NotMapped]
        public string RentalFeeString
        {
            get { if (RentalFee != null) return RentalFee.Value.ToString("N");
                return "";
            }
            set { SetValue(() => RentalFeeString, value); }
        }
        
    }
}