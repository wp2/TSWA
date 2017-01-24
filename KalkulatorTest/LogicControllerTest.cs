using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TSWA;

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

            /* TODO */
            //equation = "12-ABC1*(136/4)";
            //string[] expected_3 = { "", "12","", "-","", "ABC1","", "*","", "(","", "136",
            //    "", "/","", "4","", ")","" };
            //CollectionAssert.AreEqual(expected_2, equation.ParseEquation());

            //equation = "12-ABC1*(136/4)";
            //string[] expected_4 = { "", "12","", "-","", "ABC1","", "*","", "(","", "136",
            //    "", "/","", "4","", ")","" };
            //CollectionAssert.AreEqual(expected_2, equation.ParseEquation());

            //equation = "12-ABC1*(136/4)";
            //string[] expected_5 = { "", "12","", "-","", "ABC1","", "*","", "(","", "136",
            //    "", "/","", "4","", ")","" };
            //CollectionAssert.AreEqual(expected_2, equation.ParseEquation());

            //equation = "12-ABC1*(136/4)";
            //string[] expected_6 = { "", "12","", "-","", "ABC1","", "*","", "(","", "136",
            //    "", "/","", "4","", ")","" };
            //CollectionAssert.AreEqual(expected_2, equation.ParseEquation());

            //equation = "12-ABC1*(136/4)";
            //string[] expected_7 = { "", "12","", "-","", "ABC1","", "*","", "(","", "136",
            //    "", "/","", "4","", ")","" };
            //CollectionAssert.AreEqual(expected_2, equation.ParseEquation());

            //equation = "12-ABC1*(136/4)";
            //string[] expected_8 = { "", "12","", "-","", "ABC1","", "*","", "(","", "136",
            //    "", "/","", "4","", ")","" };
            //CollectionAssert.AreEqual(expected_2, equation.ParseEquation());

            //equation = "12-ABC1*(136/4)";
            //string[] expected_9 = { "", "12","", "-","", "ABC1","", "*","", "(","", "136",
            //    "", "/","", "4","", ")","" };
            //CollectionAssert.AreEqual(expected_2, equation.ParseEquation());

            //equation = "12-ABC1*(136/4)";
            //string[] expected_10 = { "", "12","", "-","", "ABC1","", "*","", "(","", "136",
            //    "", "/","", "4","", ")","" };
            //CollectionAssert.AreEqual(expected_2, equation.ParseEquation());
        }

        [TestMethod]
        public void IsStringNumber_test() {

        }

    }

    [TestClass]
    public class LogicControllerTest {


    }
}
