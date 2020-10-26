using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PinnaRent.Core.Enumerations;

namespace PinnaRent.Core.Models
{
    public class RenteeDTO : CommonFieldsF
    {
        public RenteeTypes Type
        {
            get { return GetValue(() => Type); }
            set { SetValue(() => Type, value); }
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

        public bool HadRepresentee
        {
            get { return GetValue(() => HadRepresentee); }
            set { SetValue(() => HadRepresentee, value); }
        }

        [ForeignKey("Representee")]
        public int? RepresenteeId { get; set; }
        public RepresenteeDTO Representee
        {
            get { return GetValue(() => Representee); }
            set { SetValue(() => Representee, value); }
        }

        [ForeignKey("RentDeposit")]
        public int? RentDepositId { get; set; }
        public RentDepositDTO RentDeposit
        {
            get { return GetValue(() => RentDeposit); }
            set
            {
                SetValue(() => RentDeposit, value);
            }
        }

        public ICollection<RentalContratDTO> RentalContrats
        {
            get { return GetValue(() => RentalContrats); }
            set
            {
                SetValue(() => RentalContrats, value);
            }
        }
   
        [NotMapped]
        public string NoOfContrats
        {
            get
            {
                return (RentalContrats.Count-1).ToString() + " Renewal(s)";
            }
            set { SetValue(() => NoOfContrats, value); }
        }
    }
}