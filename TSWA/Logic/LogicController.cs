using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media;
using System.Text;
using System.Xml;
using _ONP;

namespace TSWA
{
    /* Struktura przechowujaca info o wyswietlanym tekscie: tresc, rozmiar czcionki, kolor czcionki */
    public struct TextInformation {

        public TextInformation(string a_Equation, LogicController.FontSizes a_FontSize, Brush a_Foreground) {
            _equation = a_Equation;
            _fontSize = (int)a_FontSize;
            _foreground = a_Foreground;
            
        }

        private string _equation;
        private int _fontSize;
        private Brush _foreground;

        public string Equation {
            get {
                return _equation;
            }
        }

        public int FontSize {
            get {
                return _fontSize;
            }
        }

        public Brush Foreground {
            get {
                return _foreground;
            }
        }

    }

    /* Klasa umozliwiajaca przesylanie stringow jako argumenty eventow */
    public class MyEventArgs : EventArgs {
        public string MyEventString { get; set; }

        public MyEventArgs(string myString) {
            MyEventString = myString;
        }
    }

    /* Klasa rozszerzajaca funkcjonalnosc chara */
    public static class Extensions {
        public static bool IsHex(this char c) {
            return (c >= '0' && c <= '9') ||
                     (c >= 'a' && c <= 'f') ||
                     (c >= 'A' && c <= 'F');
        }

        public static bool IsBinary(this char c) {
            return (c >= '0' && c <= '1');
        }

        public static bool IsMathematicalOperator(this char c) {
            return (c == '+' || c == '-' || c == '*' || c == '/' || c == '%');
        }
    }

    // Zarządza Logiką aplikacji 
    public class LogicController
    {
        /* Event do aktualizacji wyswietlanego tekstu */
        public event UpdateDisplayedText UpdateDisplay;
        public EventArgs eUpdateArgs = null;
        public delegate void UpdateDisplayedText(LogicController lc, EventArgs eUpdateArgs);

        /* Event informujacy o bledzie */
        public event DisplayErrorInfo ErrorOccurred;
        public MyEventArgs eErrorArgs = null;
        public delegate void DisplayErrorInfo(LogicController lc, MyEventArgs eErrorArgs);

        /* Tablica dostpenych operacji matematycznych */
        static char[] SignsArray = { '+', '-', '*', '/', '%' };

        /* Zestaw dostepnych systemow liczbowych i zmienna przechowujaca obecny system liczbowy */
        enum NumberBaseSystem { Binary = 2, Octal = 8, Decimal= 10, Hexadecimal = 16 };
        NumberBaseSystem CurrentNumberBaseSystem;

        /* Zestaw dostepnych rozmiarow czcionek i zmienna przechowujaca obecny rozmiar czcionki */
        public enum FontSizes { VerySmall = 15, Small = 25, Medium = 35, Big = 45, VeryBig = 50 };
        FontSizes CurrentFontSize;

        /* Obecny kolor czcionki */
        Brush CurrentFontColor;

        /* Obecne dzialanie matematyczne */
        string CurrentEquationState;

        /* Zestaw dostepnych dlugosci slow i zmienna przechowujaca obecna dlugosc slowa */
        public enum WordLengths { BYTE = 8, WORD = 16, DWORD = 32, QWORD = 64 };
        WordLengths CurrentWordLength;

        
        public LogicController() {
            Init();
        }
        
        private void Init()
        {
            // Ustaw stany początkowe wpisane z pliku
            FileStream FileInput = new FileStream("KalkulatorUstawienia.xml", FileMode.Open);
            XmlReader xml = XmlReader.Create(FileInput);
            while (xml.Read()) {
                if (xml.Name == "InitialState") {
                    while (xml.Read()) {
                        if (xml.Name == "CurrentEquationState" && xml.NodeType != XmlNodeType.EndElement) {
                            xml.Read();
                            CurrentEquationState = xml.Value;
                        }
                        else if (xml.Name == "CurrentNumberBaseSystem" && xml.NodeType != XmlNodeType.EndElement) {
                            xml.Read();
                            CurrentNumberBaseSystem = (NumberBaseSystem)Int32.Parse(xml.Value);
                        }
                        else if (xml.Name == "CurrentFontSize" && xml.NodeType != XmlNodeType.EndElement) {
                            xml.Read();
                            CurrentFontSize = (FontSizes)Int32.Parse(xml.Value);
                        }
                        else if (xml.Name == "CurrentFontColor" && xml.NodeType != XmlNodeType.EndElement) {
                            xml.Read();
                            CurrentFontColor = new BrushConverter().ConvertFromString(xml.Value) as SolidColorBrush;  
                        }
                        else if(xml.Name == "CurrentWordLength" && xml.NodeType != XmlNodeType.EndElement) {
                            xml.Read();
                            CurrentWordLength = (WordLengths)Enum.Parse(typeof(WordLengths), xml.Value);
                        }
                    }
                }
            }
            xml.Close();
            FileInput.Close();
        }

        /* Czysci dzialanie */
        public void ClearEquation() {
            CurrentEquationState = "0";
            UpdateDisplay(this, eUpdateArgs);
        }

        /* Dodaje cyfre do dzialania */
        public void AddNumberToEquation(string Number)
        {
            if(CurrentEquationState == "0") {
                CurrentEquationState = Number;
            }
            else {
                CurrentEquationState += Number;
            }
            UpdateDisplay(this, eUpdateArgs);
        }

        /* Usuwa ostatni wprowadzony znak */
        public void DeleteLastSignInEquation() {
            if (CurrentEquationState != "0" && CurrentEquationState.Length > 1) {
                CurrentEquationState = CurrentEquationState.Remove(CurrentEquationState.Length - 1);
            }
            else if (CurrentEquationState != "0" && CurrentEquationState.Length == 1) {
                CurrentEquationState = "0";
            }
            UpdateDisplay(this, eUpdateArgs);
        }

        /* Dodaje znak do dzialania */
        public void AddSignToEquation(char Operator)
        {
            if(true == CurrentEquationState[CurrentEquationState.Length - 1].IsMathematicalOperator()) {
                CurrentEquationState = CurrentEquationState.Remove(CurrentEquationState.Length - 1, 1);
            }
            CurrentEquationState += Operator;
            UpdateDisplay(this, eUpdateArgs);
        }

        /* Dodaje nawias do dzialania */
        public void AddParenthesisToEquation(char Parenthesis) {
            int numberOfOpeningdParenthesis = CurrentEquationState.Split('(').Length - 1;
            int numberOfClosingdParenthesis = CurrentEquationState.Split(')').Length - 1;
            if (Parenthesis == ')') {
                if (numberOfOpeningdParenthesis > numberOfClosingdParenthesis) {
                    if (true == (CurrentEquationState[CurrentEquationState.Length - 1]).IsHex() ||
                        CurrentEquationState[CurrentEquationState.Length - 1] == ')') {
                        CurrentEquationState += Parenthesis;
                    }
                }
            }
            else {
                if(CurrentEquationState == "0") {
                    CurrentEquationState = Parenthesis.ToString();
                }
                else {
                    if (CurrentEquationState[CurrentEquationState.Length - 1].IsMathematicalOperator()) {
                        CurrentEquationState += Parenthesis;
                    } 
                }
            }
            UpdateDisplay(this, eUpdateArgs);
        }

        /* Przekazuje dzialanie do obliczenia */
        public void ExecuteCalculations() {
            ONP myONP = new ONP(CurrentEquationState);
            CurrentEquationState = myONP.ONPCalculationResult();

            /* W przypadku bledu we wprowadzonym dzialaniu wyslij eventa (GUI go odbiera i wyswietla) */
            if(CurrentEquationState == "ERROR") {
                ErrorOccurred(this, new MyEventArgs("Blad we wprowadzonym dzialaniu!"));
                CurrentEquationState = "0";
            }
            UpdateDisplay(this, eUpdateArgs);
        }

        /* Wyslanie struktury z informacjami nt. wyswietlanego tekstu */
        public TextInformation GetTextInformation() {
            return new TextInformation(CurrentEquationState, CurrentFontSize, CurrentFontColor);
        }

        /* Zmiana dlugosci uzywanego slowa */
        public void ChangeWordLength() {
            switch(CurrentWordLength) {
                case WordLengths.QWORD:
                    CurrentWordLength = WordLengths.DWORD;
                    break;
                case WordLengths.DWORD:
                    CurrentWordLength = WordLengths.WORD;
                    break;
                case WordLengths.WORD:
                    CurrentWordLength = WordLengths.BYTE;
                    break;
                case WordLengths.BYTE:
                    CurrentWordLength = WordLengths.QWORD;
                    break;
            }
        }
    }
}
