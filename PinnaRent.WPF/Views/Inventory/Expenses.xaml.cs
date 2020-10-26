using System.ComponentModel;
using System.Windows;
using PinnaRent.WPF.ViewModel;

namespace PinnaRent.WPF.Views
{
    /// <summary>
    /// Interaction logic for Expenses.xaml
    /// </summary>
    public partial class Expenses : Window
    {
        public Expenses()
        {
            InitializeComponent();
        }

        private void ExpenseLoans_OnClosing(object sender, CancelEventArgs e)
        {
            ExpensesViewModel.CleanUp();
        }

        
    }
}
