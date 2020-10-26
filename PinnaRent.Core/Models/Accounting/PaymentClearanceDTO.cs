using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PinnaRent.Core.Models
{
    //public class PaymentClearanceDTO : CommonFieldsA
    //{
    //    [ForeignKey("ClientAccount")]
    //    public int ClientAccountId { get; set; }
    //    public BankAccountDTO ClientAccount
    //    {
    //        get { return GetValue(() => ClientAccount); }
    //        set { SetValue(() => ClientAccount, value); }
    //    }

    //    [ForeignKey("Payment")]
    //    public int PaymentId { get; set; }
    //    public PaymentDTO Payment
    //    {
    //        get { return GetValue(() => Payment); }
    //        set { SetValue(() => Payment, value); }
    //    }

    //    public DateTime? DepositedOnDate
    //    {
    //        get { return GetValue(() => DepositedOnDate); }
    //        set { SetValue(() => DepositedOnDate, value); }
    //    }

    //    //for logging purpose
    //    public string DepositBy
    //    {
    //        get { return GetValue(() => DepositBy); }
    //        set { SetValue(() => DepositBy, value); }
    //    }

    //    [MaxLength(50, ErrorMessage = "Statement Number exceeded 50 letters")]
    //    public string Reference
    //    {
    //        get { return GetValue(() => Reference); }
    //        set { SetValue(() => Reference, value); }
    //    }
        
    //    public DateTime? PostDate
    //    {
    //        get { return GetValue(() => PostDate); }
    //        set { SetValue(() => PostDate, value); }
    //    }
    //    public decimal? PostedAmount
    //    {
    //        get { return GetValue(() => PostedAmount); }
    //        set { SetValue(() => PostedAmount, value); }
    //    }
    //    //for logging purpose
    //    public DateTime? ClearedOnDate
    //    {
    //        get { return GetValue(() => ClearedOnDate); }
    //        set { SetValue(() => ClearedOnDate, value); }
    //    }
    //    //for logging purpose
    //    public string ClearedBy
    //    {
    //        get { return GetValue(() => ClearedBy); }
    //        set { SetValue(() => ClearedBy, value); }
    //    }

    //    [NotMapped]
    //    public string DepositDateString
    //    {
    //        get
    //        {
    //            return DepositedOnDate != null
    //                ? DepositedOnDate.Value.ToString("dd-MM-yyyy") + "(" +
    //                  ReportUtility.GetEthCalendarFormated(DepositedOnDate.Value, "/") + ")"
    //                : "";
    //        }
    //        set { SetValue(() => DepositDateString, value); }
    //    }
        
    //    [NotMapped]
    //    public string ClearedDateString
    //    {
    //        get
    //        {
    //            return ClearedOnDate != null
    //                ? ClearedOnDate.Value.ToString("dd-MM-yyyy") + "(" +
    //                  ReportUtility.GetEthCalendarFormated(ClearedOnDate.Value, "/") + ")"
    //                : "";
    //        }
    //        set { SetValue(() => ClearedDateString, value); }
    //    }
        
    //}
}