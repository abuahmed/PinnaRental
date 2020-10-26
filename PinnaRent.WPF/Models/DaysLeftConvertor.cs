using System;
using System.Windows.Data;
using PinnaRent.Core.Models;

namespace PinnaRent.WPF.Models
{
    public class DaysLeftConvertor : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var room = value as RoomDTO;

            if (room != null)
            {

                var ldays = room.LastRentalPaymentTemp.DaysLeft;
                if (room.LastServicePaymentTemp.Contrat.Room != null)
                {
                    var servDays = room.LastServicePaymentTemp.DaysLeft;
                    if (servDays < ldays)
                        ldays = servDays;
                }

                if (ldays > 10 && ldays <=366)
                    return "Green";
                else if (ldays <= 10 && ldays >= 0)
                    return "Yellow";
                else if (ldays < 0)
                    return "Red";
                else return "White";
            }

            return "White";
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}