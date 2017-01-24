using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media;
using System.Linq;
using System.Text;
using System.Xml;
using System.Text.RegularExpressions;
using _ONP;
using Microsoft.VisualBasic;

namespace TSWA
{
    /* Struktura przechowujaca info o wyswietlanym tekscie: tresc, rozmiar czcionki, kolor czcionki */
    public struct TextInformation {

        public TextInformation(string a_Content, LogicController.FontSizes a_FontSize, Brush a_Foreground) {
            _content = a_Content;
            _fontSize = (int)a_FontSize;
            _foreground = a_Foreground;
            
        }

        private string _content;
        private int _fontSize;
        private Brush _foreground;

        public string Content {
            get {
                return _content;
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
    public static class CharExtensions {
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

        public static bool IsParenthesis(this char c) {
            return (c == '(' || c == ')');
        }
    }

    public static class StringExtensions {

        /* Sprawdza czy w stringu znajduje sie jakis operator matematyczny */
        public static bool IsThereAnyMathematicalOperatorInString(this string strToCheck) {
            for(int i = 0; i < strToCheck.Length; ++i) {
                if(true == strToCheck[i].IsMathematicalOperator()) {
                    return true;
                }
            }
            return false;
        }

        /* Zwraca ostatnia liczbe z dzialania */
        public static string ExtractLastNumberFromString(this string strToParse, bool searchWithMinus = true) {
            string[] numbers;
            if (true == searchWithMinus) {
                numbers = Regex.Split(strToParse, @"(-?\w*\d*[\w\d]+)");
            }
            else {
                numbers = Regex.Split(strToParse, @"(\w*\d*[\w\d]+)");
            }
            return numbers[numbers.Length - 2];
        }

        public static string[] ParseEquation(this string strToParse) {
            return Regex.Split(strToParse, @"(\(|\))|(\d*\w*[\w\d]+)|(\+|\-|\*|\%|\/)");
        }

        public static bool IsStringNumber(this string strNumber) {
            return Regex.IsMatch(strNumber, @"(\w*\d*[\w\d]+)");
        }
    }

    // Zarządza Logiką aplikacji 
    public class LogicController
    {
        /* Event do aktualizacji wyswietlanego tekstu */
        public event UpdateDisplayedText UpdateDisplay;
        public EventArgs eUpdateArgs = null;
        public delegate void UpdateDisplayedText(LogicController lc, EventArgs eUpdateArgs = null);

        /* Event informujacy o bledzie */
        public event DisplayErrorInfo ErrorOccurred;
        //public MyEventArgs eErrorArgs = null;
        public delegate void DisplayErrorInfo(LogicController lc, MyEventArgs eErrorArgs);

        /* Event do blokowania i odblokowywania zmiany dlugosci slowa */
        public event ChangeWordButtonState ChangeWordButton;
        //public MyEventArgs eWordButtonArgs = null;
        public delegate void ChangeWordButtonState(LogicController lc, MyEventArgs eWordButtonArgs);

        public event UnlockSpecifiedButtons UnlockButtons;
        public delegate void UnlockSpecifiedButtons(LogicController lc, MyEventArgs groupToLock);

        public event LockSpecifiedButtons LockButtons;
        public delegate void LockSpecifiedButtons(LogicController lc, MyEventArgs groupToLock);

        /* Tablica dostpenych operacji matematycznych */
        static char[] SignsArray = { '+', '-', '*', '/', '%' };

        /* Zestaw dostepnych systemow liczbowych i zmienna przechowujaca obecny system liczbowy */
        public enum NumberBaseSystem { Binary = 2, Octal = 8, Decimal= 10, Hexadecimal = 16 };
        public NumberBaseSystem CurrentNumberBaseSystem { get; set; }

        /* Zestaw dostepnych rozmiarow czcionek i zmienna przechowujaca obecny rozmiar czcionki */
        public enum FontSizes { VerySmall = 12, Small = 25, Medium = 35, Big = 45, VeryBig = 50 };
        public FontSizes CurrentFontSize { get; set; }

        /* Obecny kolor czcionki */
        public Brush CurrentFontColor { get; set; }

        //public string MyEventString { get; set; }

        /* Obecne dzialanie matematyczne */
        public string CurrentEquationState { get; set; }

        /* Zestaw dostepnych dlugosci slow i zmienna przechowujaca obecna dlugosc slowa */
        public enum WordLengths { BYTE = 8, WORD = 16, DWORD = 32, QWORD = 64 };
        public WordLengths CurrentWordLength { get; set; }

        bool m_fSearchWithMinus;
        
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
            m_fSearchWithMinus = true;
        }

        /* Czysci dzialanie */
        public void ClearEquation() {
            CurrentEquationState = "0";
            UpdateDisplay(this, eUpdateArgs);
            ChangeWordButton(this, new MyEventArgs("true"));
        }

        /* Dodaje cyfre do dzialania */
        public void AddNumberToEquation(string Number) {
            if(CurrentEquationState == "0") {
                CurrentEquationState = Number;
            }
            else {
                string tmpNumber = (CurrentEquationState + Number).ExtractLastNumberFromString(m_fSearchWithMinus);
                if (true == TryConversionToDecimal(ref tmpNumber)) {
                    CurrentEquationState += Number;
                }
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
            if (false == CurrentEquationState.IsThereAnyMathematicalOperatorInString()) {
                ChangeWordButton(this, new MyEventArgs("true"));
            }
            UpdateDisplay(this, eUpdateArgs);
        }

        /* Dodaje znak do dzialania */
        public void AddSignToEquation(char Operator) {
            if(true == CurrentEquationState[CurrentEquationState.Length - 1].IsMathematicalOperator()) {
                CurrentEquationState = CurrentEquationState.Remove(CurrentEquationState.Length - 1, 1);
            }
            CurrentEquationState += Operator;
            ChangeWordButton(this, new MyEventArgs("false"));
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
            string tempTODELETE = CurrentEquationState;
            if(NumberBaseSystem.Decimal != CurrentNumberBaseSystem) {
                string[] parsedEquation = CurrentEquationState.ParseEquation();
                AnalyzeParsedEquation(parsedEquation);
            }

            ONP myONP = new ONP(CurrentEquationState);
            CurrentEquationState = myONP.ONPCalculationResult();

            /* W przypadku bledu we wprowadzonym dzialaniu wyslij eventa (GUI go odbiera i wyswietla) */
            if(CurrentEquationState == "ERROR") {
                ErrorOccurred(this, new MyEventArgs("Blad we wprowadzonym dzialaniu!"));
                CurrentEquationState = "0";
            }
            else {
                CutDecimalPortion();
                // ewentualne skrocenie liczby gdyby byla za duza dla danego rozmiaru slowa
                TrimOutcomeNumber();
                ChangeNumberBaseSystem(CurrentEquationState, NumberBaseSystem.Decimal, CurrentNumberBaseSystem);
            }
            ChangeWordButton(this, new MyEventArgs("true"));
            UpdateDisplay(this, eUpdateArgs);
        }

        /* Wyslanie struktury z informacjami nt. wyswietlanego tekstu */
        public TextInformation GetTextInformation() {
            return new TextInformation(CurrentEquationState, CurrentFontSize, CurrentFontColor);
        }

        /* Zmiana dlugosci uzywanego slowa */
        public TextInformation ChangeWordLength() {
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
            ChangeNumberBaseSystem(ConvertNumberToDecimal(CurrentEquationState.ExtractLastNumberFromString(m_fSearchWithMinus)), NumberBaseSystem.Decimal, CurrentNumberBaseSystem);
            return new TextInformation(CurrentWordLength.ToString(), FontSizes.VerySmall, Brushes.Blue);
        }

        /* Ustawia nowy system liczbowy */
        public void ChangeBaseNumberSystem(string NumberSystem) {
            NumberBaseSystem previousBaseSystem = CurrentNumberBaseSystem;
            string tempNumber = ConvertNumberToDecimal(CurrentEquationState.ExtractLastNumberFromString(m_fSearchWithMinus));

            CurrentNumberBaseSystem = (NumberBaseSystem)Enum.Parse(typeof(NumberBaseSystem), NumberSystem);
            switch (CurrentNumberBaseSystem) {
                case NumberBaseSystem.Binary:
                    LockButtons(this, new MyEventArgs("Octal"));
                    LockButtons(this, new MyEventArgs("Decimal"));
                    LockButtons(this, new MyEventArgs("Hexadecimal"));
                    m_fSearchWithMinus = false;
                    break;
                case NumberBaseSystem.Octal:
                    UnlockButtons(this, new MyEventArgs("Octal"));
                    LockButtons(this, new MyEventArgs("Decimal"));
                    LockButtons(this, new MyEventArgs("Hexadecimal"));
                    m_fSearchWithMinus = false;
                    break;
                case NumberBaseSystem.Decimal:
                    UnlockButtons(this, new MyEventArgs("Octal"));
                    UnlockButtons(this, new MyEventArgs("Decimal"));
                    LockButtons(this, new MyEventArgs("Hexadecimal"));
                    m_fSearchWithMinus = true;
                    break;
                case NumberBaseSystem.Hexadecimal:
                    UnlockButtons(this, new MyEventArgs("Octal"));
                    UnlockButtons(this, new MyEventArgs("Decimal"));
                    UnlockButtons(this, new MyEventArgs("Hexadecimal"));
                    m_fSearchWithMinus = false;
                    break;
            }
            ChangeNumberBaseSystem(tempNumber, NumberBaseSystem.Decimal, CurrentNumberBaseSystem);
        }

        /* Zwraca info o czcionce */
        public TextInformation GetFontForNumberBaseButton(string Tag) {
            Brush color = Brushes.Black;
            if (CurrentNumberBaseSystem == (NumberBaseSystem)Enum.Parse(typeof(NumberBaseSystem), Tag)) {
                color = Brushes.Blue;
            }
            return new TextInformation(CurrentWordLength.ToString(), FontSizes.VerySmall, color);
        }

        /* Zmienia wyswietlana liczbe (w dowolnym systemie liczbowym w zaleznosci 
         * od wybranej dlugosci slowa. Na wejsciu przyjmuje liczbe w systemie 
         * dziesietnym. */
        public void ChangeNumberBaseSystem(string decNumber, NumberBaseSystem fromBase, NumberBaseSystem goalBase) {

            switch (CurrentWordLength) {
                case WordLengths.QWORD:
                    CurrentEquationState = Convert.ToString(Convert.ToInt64((long.Parse(decNumber)).ToString(), (int)fromBase), (int)goalBase).ToUpper();
                    break;
                case WordLengths.DWORD:
                    CurrentEquationState = Convert.ToString(Convert.ToInt32(((int)(long.Parse(decNumber))).ToString(), (int)fromBase), (int)goalBase).ToUpper();
                    break;
                case WordLengths.WORD:
                    CurrentEquationState = Convert.ToString(Convert.ToInt16(((short)(long.Parse(decNumber))).ToString(), (int)fromBase), (int)goalBase).ToUpper();
                    break;
                case WordLengths.BYTE:
                    CurrentEquationState = Convert.ToString(Convert.ToSByte(((sbyte)(long.Parse(decNumber))).ToString(), (int)fromBase), (int)goalBase).ToUpper();
                    switch (CurrentNumberBaseSystem) {
                        case NumberBaseSystem.Binary:
                            if(CurrentEquationState.Length > 8) CurrentEquationState = CurrentEquationState.Substring(CurrentEquationState.Length - 8);
                            break;
                        case NumberBaseSystem.Decimal:
                            break;
                        case NumberBaseSystem.Hexadecimal:
                            if (CurrentEquationState.Length > 2) CurrentEquationState = CurrentEquationState.Substring(CurrentEquationState.Length - 2);
                            break;
                        case NumberBaseSystem.Octal:
                            break;
                    }
                    break;
            }
            UpdateDisplay(this, eUpdateArgs);
        }

        /* Sprawdza czy da sie zapisac liczbe w systemie dziesietnym dla danego rozmiaru slowa.
         * Jesli jest to mozliwe, zwracana jest wartosc true, a w argumencie Number znajduje sie
         * skonwertowana wartosc. Jesli nie jest to mozliwe zwracana jest wartosc false, a w argumencie
         * Number znajduje sie poczatkowa wartosc. */
        public bool TryConversionToDecimal(ref string Number) {
            bool bReturn = true;
            try {
                switch (CurrentWordLength) {
                    case WordLengths.QWORD:
                        Number = Convert.ToInt64(Number, (int)CurrentNumberBaseSystem).ToString();
                        break;
                    case WordLengths.DWORD:
                        Number = Convert.ToInt32(Number, (int)CurrentNumberBaseSystem).ToString();
                        break;
                    case WordLengths.WORD:
                        Number = Convert.ToInt16(Number, (int)CurrentNumberBaseSystem).ToString();
                        break;
                    case WordLengths.BYTE:
                        Number = Convert.ToSByte(Number, (int)CurrentNumberBaseSystem).ToString();
                        break;
                }
            }
            catch (OverflowException) {
                bReturn = false;
            }
            return bReturn;
        }

        /* Konwertuje liczbe podana w stringu na liczbe w systemie dziesietnym. 
         * Konwersja uwzglednia obecna dlugosc slowa. */
        public string ConvertNumberToDecimal(string Number) {
            switch (CurrentWordLength) {
                case WordLengths.QWORD:
                    Number = Convert.ToInt64(Number, (int)CurrentNumberBaseSystem).ToString();
                    break;
                case WordLengths.DWORD:
                    Number = ((int)Convert.ToInt64(Number, (int)CurrentNumberBaseSystem)).ToString();
                    break;
                case WordLengths.WORD:
                    Number = ((short)Convert.ToInt32(Number, (int)CurrentNumberBaseSystem)).ToString();
                    break;
                case WordLengths.BYTE:
                    Number = ((sbyte)Convert.ToInt16(Number, (int)CurrentNumberBaseSystem)).ToString();
                    break;
            }
            return Number;
        }

        /* Usuwa czesc dziesietna (ulamkowa) z liczby */
        public void CutDecimalPortion() {
            int resultIndex = CurrentEquationState.IndexOf(',');
            if(-1 != resultIndex) {
                CurrentEquationState = CurrentEquationState.Substring(0, resultIndex);
            }
        }

        /* TODO */
        public void TrimOutcomeNumber() {
            //if(false == TryConversionToDecimal(ref CurrentEquationState)) {
            //    switch (CurrentWordLength) {
            //        case WordLengths.QWORD:
            //            CurrentEquationState = CurrentEquationState.Substring(CurrentEquationState.Length - 16);
            //            break;
            //        case WordLengths.DWORD:
            //            CurrentEquationState = CurrentEquationState.Substring(CurrentEquationState.Length - 8);
            //            break;
            //        case WordLengths.WORD:
            //            CurrentEquationState = CurrentEquationState.Substring(CurrentEquationState.Length - 4);
            //            break;
            //        case WordLengths.BYTE:
            //            CurrentEquationState = CurrentEquationState.Substring(CurrentEquationState.Length - 2);
            //            break;
            //    }
            //}
        }

        public void AnalyzeParsedEquation(string[] parsedEuqation) {
            CurrentEquationState = "";
            foreach(string s in parsedEuqation) {
                if(s.Length == 0) {
                    continue;
                }
                if(s.Length == 1 && (s[0].IsParenthesis() || s[0].IsMathematicalOperator())) {
                    CurrentEquationState += s;
                }
                else if(true == s.IsStringNumber()) {
                    CurrentEquationState += ConvertNumberToDecimal(s);
                }
                
            }
        }

    }
}

