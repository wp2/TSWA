using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TSWA {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            e.Handled = true;
            var myValue = ((Button)sender).Tag.ToString();
            MessageBox.Show(String.Format("{0}, {1}, {2}, {3}", sender.ToString(),e.ToString(), e.OriginalSource.ToString(), myValue));
            int rowNumber = (int)((Button)sender).GetValue(Grid.RowProperty);
            int colNumber = (int)((Button)sender).GetValue(Grid.ColumnProperty);
        }

        private void Numeric_Button_Click(object sender, RoutedEventArgs e) {
            e.Handled = true;
            var myValue = ((Button)sender).Tag.ToString();

            var currentlyDisplayed = Displayer.Text.ToString();
            if(string.Compare(currentlyDisplayed,"0") == 0) {
                currentlyDisplayed = myValue;
            }
            else {
                currentlyDisplayed = string.Concat(currentlyDisplayed, myValue);
            }
            Displayer.Text = currentlyDisplayed;
        }
    }
}
