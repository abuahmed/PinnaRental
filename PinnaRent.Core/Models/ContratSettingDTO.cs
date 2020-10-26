namespace PinnaRent.Core.Models
{
    public class ContratSettingDTO : CommonFieldsA
    {
        public string Ministry
        {
            get { return GetValue(() => Ministry); }
            set { SetValue(() => Ministry, value); }
        }
        public string Office
        {
            get { return GetValue(() => Office); }
            set { SetValue(() => Office, value); }
        }
        public string City
        {
            get { return GetValue(() => City); }
            set { SetValue(() => City, value); }
        }

        public int? AdditionalPenalityDays
        {
            get { return GetValue(() => AdditionalPenalityDays); }
            set { SetValue(() => AdditionalPenalityDays, value); }
        }

        public string GoverningArticleCode
        {
            get { return GetValue(() => GoverningArticleCode); }
            set { SetValue(() => GoverningArticleCode, value); }
        }
        public string TerminationArticleCode
        {
            get { return GetValue(() => TerminationArticleCode); }
            set { SetValue(() => TerminationArticleCode, value); }
        }
        public decimal? TerminationAmount
        {
            get { return GetValue(() => TerminationAmount); }
            set { SetValue(() => TerminationAmount, value); }
        }
        public string DueDaysToDiscontinueContrat
        {
            get { return GetValue(() => DueDaysToDiscontinueContrat); }
            set { SetValue(() => DueDaysToDiscontinueContrat, value); }
        }
    }
}