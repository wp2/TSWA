using System;
using System.Windows;
using System.Windows.Controls;

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

            /* Wywola metode blokujaca lub odblokowujaca (w zaleznosci od argumentu eventu) WordButton */
            LogicMaster.ChangeWordButton += new LogicController.ChangeWordButtonState(ChangeWordButtonState);

            /* */
            LogicMaster.UnlockButtons += new LogicController.UnlockSpecifiedButtons(UnlockButtonsWithSpecifiedTag);

            /* */
            LogicMaster.LockButtons += new LogicController.LockSpecifiedButtons(LockButtonsWithSpecifiedTag);

            /* Ustawia poczatkowa czcionke dla przyciskow zmieniajacych system liczbowy */
            SetButtonFont(BinButton, LogicMaster.GetFontForNumberBaseButton(BinButton.Tag.ToString()));
            SetButtonFont(OctButton, LogicMaster.GetFontForNumberBaseButton(OctButton.Tag.ToString()));
            SetButtonFont(DecButton, LogicMaster.GetFontForNumberBaseButton(DecButton.Tag.ToString()));
            SetButtonFont(HexButton, LogicMaster.GetFontForNumberBaseButton(HexButton.Tag.ToString()));
        }

        /* Metoda wywolywana przy nacisnieciu przycisku numerycznego */
        private void Numeric_Button_Click(object sender, RoutedEventArgs e) {
            LogicMaster.AddNumberToEquation(((Button)sender).Content.ToString());
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

        /* Metoda wywolywana przy nacisnieciu przycisku zmiany dlugosci slowa */
        private void Word_Button_Click(object sender, RoutedEventArgs e) {
            TextInformation newTextInformation = LogicMaster.ChangeWordLength();
            WordButton.Content = newTextInformation.Content;
            SetButtonFont(WordButton, newTextInformation);
        }

        /* Metoda wywolywana przy nacisnieciu przycisku rownania */
        private void Equal_Button_Click(object sender, RoutedEventArgs e) {
            LogicMaster.ExecuteCalculations();
        }

        /* Metoda aktualizacujace wyswietlany tekst */
        private void UpdateDisplay() {
            TextInformation newTextInformation = LogicMaster.GetTextInformation();
            Displayer.Text = newTextInformation.Content;
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

        /* Metoda wywolywana przy nacisnieciu przycisku z nawiasem */
        private void Parenthesis_Button_Click(object sender, RoutedEventArgs e) {
            LogicMaster.AddParenthesisToEquation(((Button)sender).Tag.ToString().ToCharArray()[0]);
        }

        private void ChangeWordButtonState(LogicController lc, MyEventArgs e) {
            WordButton.IsEnabled = bool.Parse(e.MyEventString);
            BinButton.IsEnabled = bool.Parse(e.MyEventString);
            OctButton.IsEnabled = bool.Parse(e.MyEventString);
            DecButton.IsEnabled = bool.Parse(e.MyEventString);
            HexButton.IsEnabled = bool.Parse(e.MyEventString);
        }

        private void Base_Button_Click(object sender, RoutedEventArgs e) {
            LogicMaster.ChangeBaseNumberSystem(((Button)sender).Tag.ToString());
            SetButtonFont(BinButton, LogicMaster.GetFontForNumberBaseButton(BinButton.Tag.ToString()));
            SetButtonFont(OctButton, LogicMaster.GetFontForNumberBaseButton(OctButton.Tag.ToString()));
            SetButtonFont(DecButton, LogicMaster.GetFontForNumberBaseButton(DecButton.Tag.ToString()));
            SetButtonFont(HexButton, LogicMaster.GetFontForNumberBaseButton(HexButton.Tag.ToString()));
        }

        private void SetButtonFont(Button btn, TextInformation txtInfo) {
            btn.FontSize = txtInfo.FontSize;
            btn.Foreground = txtInfo.Foreground;
        }

        private void LockButtonsWithSpecifiedTag(LogicController lc, MyEventArgs e) {
            for (int i = 0; i < MainGrid.Children.Count; ++i) {
                if (MainGrid.Children[i] is Button) {
                    Button tmpButton = (Button)MainGrid.Children[i];
                    if (tmpButton.Tag != null && tmpButton.Name.ToString() == "" && tmpButton.Tag.ToString() == e.MyEventString) {
                        MainGrid.Children[i].IsEnabled = false;
                    }
                }
            }
        }

        private void UnlockButtonsWithSpecifiedTag(LogicController lc, MyEventArgs e) {
            for (int i = 0; i < MainGrid.Children.Count; ++i) {
                if (MainGrid.Children[i] is Button) {
                    Button tmpButton = (Button)MainGrid.Children[i];
                    if (tmpButton.Tag != null && tmpButton.Name == "" && tmpButton.Tag.ToString() == e.MyEventString) {
                        MainGrid.Children[i].IsEnabled = true;
                    }
                }
            }
        }

    }
}


//for(int i = 0; i < MainGrid.Children.Count; ++i) {
//    if(MainGrid.Children[i] is Button) {
//        Button tmpButton = (Button)MainGrid.Children[i];
//        if(tmpButton.Tag != null && tmpButton.Tag.ToString() == "Binary") {
//            MainGrid.Children[i].IsEnabled = false;
//        }
//    }
//}

//private void Button_Click(object sender, RoutedEventArgs e) {
//    e.Handled = true;
//    var myValue = ((Button)sender).Tag.ToString();
//    MessageBox.Show(String.Format("{0}, {1}, {2}, {3}", sender.ToString(),e.ToString(), e.OriginalSource.ToString(), myValue));
//    int rowNumber = (int)((Button)sender).GetValue(Grid.RowProperty);
//    int colNumber = (int)((Button)sender).GetValue(Grid.ColumnProperty);
//}

