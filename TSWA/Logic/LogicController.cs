using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using _ONP;

namespace TSWA
{
    public class LogicController
    {
        enum CurrentNumberBaseSystem { Binary, Octal, Decimal, Hexadecimal };

        string CurrentEquationState;
        CurrentNumberBaseSystem m_CurrentNumberBaseSystem;


        public LogicController() { Init(); }
        
        // Zarządza Logiką aplikacji 
        public void Init()
        {
            // Ustaw stany początkowe wpisane/ z pliku ?
            FileStream FileInput = new FileStream("KalkulatorUstawienia.xml", FileMode.Open);
            XmlReader xml = XmlReader.Create(FileInput);
            while (xml.Read()) {
                if (xml.Name == "InitialState") {
                    while (xml.Read()) {
                        if (xml.Name == "CurrentEquationState" && xml.NodeType != XmlNodeType.EndElement) {
                            xml.Read();
                            this.CurrentEquationState = xml.Value;
                        }
                        if (xml.Name == "CurrentNumberBaseSystem" && xml.NodeType != XmlNodeType.EndElement) {
                            xml.Read();
                            this.m_CurrentNumberBaseSystem =  (CurrentNumberBaseSystem)Int32.Parse(xml.Value);
                        }

                    }
                }
            }
            xml.Close();
            FileInput.Close();

        }

        public void ClearEquation()
        {
            this.CurrentEquationState = "0";

        }

        public void OnNumberButtonClick(string Number)
        {
            if(this.CurrentEquationState == "0") { ClearEquation(); }
            this.CurrentEquationState += Number; 
        }

        public void OnDelButtonClick() {

        }

        public void OnBasicOperatorClick(string Operator)
        {
            this.CurrentEquationState += Operator;
        }

        public void OnAdvOperatorClick(string Operator)
        {
            this.CurrentEquationState += Operator;
        }

        public void OnClearButtonClick()
        {

        }

        public void OnBackspaceButtonClick()
        {

        }

        public void OnEqualButtonClick()
        {
            Console.WriteLine("2 mod 2 = " + 2 % 2 + " expected = 1");
            CurrentEquationState = "2%8 ";
            ONP myONP = new ONP(CurrentEquationState);
            CurrentEquationState = myONP.ONPCalculationResult();
            int t = 5;
        }
    }
}
