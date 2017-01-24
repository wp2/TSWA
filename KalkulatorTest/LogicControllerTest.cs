using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TSWA;
using System.Windows.Media;

namespace KalkulatorTest {

    [TestClass]
    public class CharExtensionsTest {

        [TestMethod]
        public void IsHex_test() {
            char c = ' ';
            bool expected = true;

            c = 'a';
            Assert.AreEqual(expected, c.IsHex());
            c = 'd';
            Assert.AreEqual(expected, c.IsHex());
            c = 'f';
            Assert.AreEqual(expected, c.IsHex());
            c = 'A';
            Assert.AreEqual(expected, c.IsHex());
            c = 'D';
            Assert.AreEqual(expected, c.IsHex());
            c = 'F';
            Assert.AreEqual(expected, c.IsHex());
            c = '0';
            Assert.AreEqual(expected, c.IsHex());
            c = '1';
            Assert.AreEqual(expected, c.IsHex());
            c = '3';
            Assert.AreEqual(expected, c.IsHex());
            c = 'D';
            Assert.AreEqual(expected, c.IsHex());
            c = '5';
            Assert.AreEqual(expected, c.IsHex());
            c = '9';

            expected = false;
            c = 'g';
            Assert.AreEqual(expected, c.IsHex());
            c = 'h';
            Assert.AreEqual(expected, c.IsHex());
            c = ' ';
            Assert.AreEqual(expected, c.IsHex());
            c = 'p';
            Assert.AreEqual(expected, c.IsHex());
            c = ';';
            Assert.AreEqual(expected, c.IsHex());
            c = '.';
            Assert.AreEqual(expected, c.IsHex());
            c = '/';
            Assert.AreEqual(expected, c.IsHex());
            c = ']';
            Assert.AreEqual(expected, c.IsHex());
        }

        [TestMethod]
        public void IsBinary_test() {
            char c = ' ';
            bool expected = true;

            c = '1';
            Assert.AreEqual(expected, c.IsBinary());
            c = '0';
            Assert.AreEqual(expected, c.IsBinary());

            expected = false;
            c = '2';
            Assert.AreEqual(expected, c.IsBinary());
            c = 'a';
            Assert.AreEqual(expected, c.IsBinary());
        }

        [TestMethod]
        public void IsMathematicalOperator_test() {
            char c = ' ';
            bool expected = true;

            c = '+';
            Assert.AreEqual(expected, c.IsMathematicalOperator());
            c = '-';
            Assert.AreEqual(expected, c.IsMathematicalOperator());
            c = '*';
            Assert.AreEqual(expected, c.IsMathematicalOperator());
            c = '/';
            Assert.AreEqual(expected, c.IsMathematicalOperator());
            c = '%';
            Assert.AreEqual(expected, c.IsMathematicalOperator());

            expected = false;
            c = '2';
            Assert.AreEqual(expected, c.IsMathematicalOperator());
            c = 'a';
            Assert.AreEqual(expected, c.IsMathematicalOperator());
            c = ';';
            Assert.AreEqual(expected, c.IsMathematicalOperator());
            c = '^';
            Assert.AreEqual(expected, c.IsMathematicalOperator());
        }

        [TestMethod]
        public void IsParenthesis_test() {
            char c = ' ';
            bool expected = true;

            c = '(';
            Assert.AreEqual(expected, c.IsParenthesis());
            c = ')';
            Assert.AreEqual(expected, c.IsParenthesis());

            expected = false;
            c = '[';
            Assert.AreEqual(expected, c.IsParenthesis());
            c = ']';
            Assert.AreEqual(expected, c.IsParenthesis());
        }

    }

    [TestClass]
    public class StringExtensionsTest {

        [TestMethod]
        public void IsThereAnyMathematicalOperatorInString_test() {
            string testString = "";
            bool expected = true;

            testString = "211+ab3";
            Assert.AreEqual(expected, testString.IsThereAnyMathematicalOperatorInString());
            testString = "98*(12+1)-1";
            Assert.AreEqual(expected, testString.IsThereAnyMathematicalOperatorInString());
            testString = "22/2";
            Assert.AreEqual(expected, testString.IsThereAnyMathematicalOperatorInString());
            testString = "abc-2";
            Assert.AreEqual(expected, testString.IsThereAnyMathematicalOperatorInString());
            testString = "897%3";
            Assert.AreEqual(expected, testString.IsThereAnyMathematicalOperatorInString());
            testString = "2*5";
            Assert.AreEqual(expected, testString.IsThereAnyMathematicalOperatorInString());

            expected = false;
            testString = "211 ab3";
            Assert.AreEqual(expected, testString.IsThereAnyMathematicalOperatorInString());
            testString = "63o()3";
            Assert.AreEqual(expected, testString.IsThereAnyMathematicalOperatorInString());
            testString = "mn^123$3";
            Assert.AreEqual(expected, testString.IsThereAnyMathematicalOperatorInString());
            testString = "12^2";
            Assert.AreEqual(expected, testString.IsThereAnyMathematicalOperatorInString());
            testString = "12&3";
            Assert.AreEqual(expected, testString.IsThereAnyMathematicalOperatorInString());
            testString = "[4<3]";
            Assert.AreEqual(expected, testString.IsThereAnyMathematicalOperatorInString());
        }

        [TestMethod]
        public void ExtractLastNumberFromString_test() {
            string number = "";
            string expected = "";

            number = "(12+45/2)*2";
            expected = "2";
            Assert.AreEqual(expected, number.ExtractLastNumberFromString(false));
            number = "143+7021-313/(12*35)";
            expected = "35";
            Assert.AreEqual(expected, number.ExtractLastNumberFromString(false));
            number = "(((12-1A)))";
            expected = "1A";
            Assert.AreEqual(expected, number.ExtractLastNumberFromString(false));
            number = "AB-12C";
            expected = "12C";
            Assert.AreEqual(expected, number.ExtractLastNumberFromString(false));

            number = "13-2";
            expected = "-2";
            Assert.AreEqual(expected, number.ExtractLastNumberFromString(true));
            number = "87+76";
            expected = "76";
            Assert.AreEqual(expected, number.ExtractLastNumberFromString(true));
            number = "(1*(2-3)/2)";
            expected = "2";
            Assert.AreEqual(expected, number.ExtractLastNumberFromString(true));
            number = "AB-12C";
            expected = "-12C";
            Assert.AreEqual(expected, number.ExtractLastNumberFromString(true));
        }

        [TestMethod]
        public void ParseEquation_test() {
            string equation = "(21+1)";
            string[] expected_1 = { "", "(", "", "21", "", "+", "", "1", "", ")", "" };
            CollectionAssert.AreEqual(expected_1, equation.ParseEquation());

            equation = "12-ABC1*(136/4)";
            string[] expected_2 = { "", "12","", "-","", "ABC1","", "*","", "(","", "136",
                "", "/","", "4","", ")","" };
            CollectionAssert.AreEqual(expected_2, equation.ParseEquation());

            equation = "1*C13/(1)";
            string[] expected_3 = { "", "1", "", "*", "", "C13", "", "/", "", "(", "", "1", "", ")", "" };
            CollectionAssert.AreEqual(expected_3, equation.ParseEquation());

            equation = "-15";
            string[] expected_4 = { "", "-", "", "15", "" };
            CollectionAssert.AreEqual(expected_4, equation.ParseEquation());

            equation = "(5*(3/6)-1)";
            string[] expected_5 = { "","(","", "5","", "*","","(","", "3","", "/","", "6","",
                ")","","-","","1","",")",""};
            CollectionAssert.AreEqual(expected_5, equation.ParseEquation());

            equation = "5";
            string[] expected_6 = { "", "5","" };
            CollectionAssert.AreEqual(expected_6, equation.ParseEquation());

            equation = "1010-111*(10+111)";
            string[] expected_7 = { "", "1010","", "-","", "111","", "*","", "(","", "10",
                "", "+","", "111","", ")","" };
            CollectionAssert.AreEqual(expected_7, equation.ParseEquation());
        }

        [TestMethod]
        public void IsStringNumber_test() {
            string strNumber = "12";
            bool expected = true;
            Assert.AreEqual(expected, strNumber.IsStringNumber());
            strNumber = "1A3";
            Assert.AreEqual(expected, strNumber.IsStringNumber());
            strNumber = "ABC";
            Assert.AreEqual(expected, strNumber.IsStringNumber());
            strNumber = "A9";
            Assert.AreEqual(expected, strNumber.IsStringNumber());
            strNumber = "7F";
            Assert.AreEqual(expected, strNumber.IsStringNumber());

            expected = false;
            strNumber = "+";
            Assert.AreEqual(expected, strNumber.IsStringNumber());
            strNumber = "";
            Assert.AreEqual(expected, strNumber.IsStringNumber());
            strNumber = "(";
            Assert.AreEqual(expected, strNumber.IsStringNumber());
            strNumber = "^";
            Assert.AreEqual(expected, strNumber.IsStringNumber());
        }

    }

    [TestClass]
    public class LogicControllerTest {

        [TestMethod]
        public void Init_test() {
            LogicController LogicMaster = new LogicController();
            Brush expectedColor = Brushes.Black;
            Assert.AreEqual(expectedColor, LogicMaster.CurrentFontColor);

            LogicController.NumberBaseSystem expecteSystem = LogicController.NumberBaseSystem.Decimal;
            Assert.AreEqual(expecteSystem, LogicMaster.CurrentNumberBaseSystem);

            LogicController.FontSizes expectedSize = LogicController.FontSizes.Big;
            Assert.AreEqual(expectedSize, LogicMaster.CurrentFontSize);

            string expectedEquation = "0";
            Assert.AreEqual(expectedEquation, LogicMaster.CurrentEquationState);

            LogicController.WordLengths expectedLength = LogicController.WordLengths.QWORD;
            Assert.AreEqual(expectedLength, LogicMaster.CurrentWordLength);
        }

        [TestMethod]
        public void ClearEquation_test() {
            //LogicController LogicMaster = new LogicController();
            //LogicMaster.CurrentEquationState = "12+7";
            //string expectedEquation = "0";
            //LogicMaster.ClearEquation();
            //Assert.AreEqual(expectedEquation, LogicMaster.CurrentEquationState);
        }

    }
}
