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
    public partial class MainWindow : Window
    {
        /* LogicMaster odpowiada za logike kalkulatora */
        LogicController LogicMaster;

        /* Konstruktor kalkulatora */
        public MainWindow() {
            InitializeComponent();
            LogicMaster = new LogicController();

            /* Wskazanie metody wywolywanej przy otrzymaniu eventa o koniecznosci pobrania nowej
             * wartosci dzialania i wyswietlenia jej */
            LogicMaster.UpdateDisplay += new LogicController.UpdateDisplayedText(UpdateDisplay);

            /* Ponizsze wywolanie pobiera iniciujace, domyslne dane do wyswietlenia */
            UpdateDisplay();

            /* Wskazanie metody wywolywanej przy otrzymaniu eventa o bledzie, jego argumenty
             * zawieraja dane do wyswietlenia w postaci stringa */
            LogicMaster.ErrorOccurred += new LogicController.DisplayErrorInfo(DisplayMessageBox);
        }

        /* Metoda wywolywana przy nacisnieciu przycisku numerycznego */
        private void Numeric_Button_Click(object sender, RoutedEventArgs e) {
            LogicMaster.AddNumberToEquation(((Button)sender).Tag.ToString());
        }

        /* Metoda wywolywana przy nacisnieciu przycisku - operacji matematycznej */
        private void Sign_Button_Click(object sender, RoutedEventArgs e) {
            LogicMaster.AddSignToEquation(((Button)sender).Tag.ToString().ToCharArray()[0]);
        }

        /* Metoda wywolywana przy nacisnieciu przycisku kasujacego ostatni wprowadzony znak */
        private void Del_Button_Click(object sender, RoutedEventArgs e) {
            LogicMaster.DeleteLastSignInEquation();
        }

        /* Metoda wywolywana przy nacisnieciu przycisku kasujacego ostatni wprowadzony znak */
        private void Clear_Button_Click(object sender, RoutedEventArgs e) {
            LogicMaster.ClearEquation();
        }

        private void Word_Button_Click(object sender, RoutedEventArgs e) {
            LogicMaster.onWordButtonClick();
        }

        /* Metoda wywolywana przy nacisnieciu przycisku rownania */
        private void Equal_Button_Click(object sender, RoutedEventArgs e) {
            LogicMaster.ExecuteCalculations();
        }

        /* Metoda aktualizacujace wyswietlany tekst */
        private void UpdateDisplay() {
            TextInformation newTextInformation = LogicMaster.GetTextInformation();
            Displayer.Text = newTextInformation.Equation;
            Displayer.FontSize = newTextInformation.FontSize;
            Displayer.Foreground = newTextInformation.Foreground;
        }

        /* Metoda wywolywana przy otrzymaniu eventa o aktualizacji wyswietlanego tekstu */
        private void UpdateDisplay(LogicController lc, EventArgs e) {
            UpdateDisplay();
        }

        /* Wyswietlenie komunikatu o bledzie  */
        private void DisplayMessageBox(LogicController lc, MyEventArgs e) {
            MessageBox.Show(e.ToString());
        }
    }
}

//private void Button_Click(object sender, RoutedEventArgs e) {
//    e.Handled = true;
//    var myValue = ((Button)sender).Tag.ToString();
//    MessageBox.Show(String.Format("{0}, {1}, {2}, {3}", sender.ToString(),e.ToString(), e.OriginalSource.ToString(), myValue));
//    int rowNumber = (int)((Button)sender).GetValue(Grid.RowProperty);
//    int colNumber = (int)((Button)sender).GetValue(Grid.ColumnProperty);
//}