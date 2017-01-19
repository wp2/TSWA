using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TSWA;
using _ONP;
namespace KalkulatorTest
{
    [TestClass]
    public class OnpTest
    {
        

        [TestMethod]
        public void TestEquation1()
        {
           
            string equation = " 2 + 3 * 6 - 8 / 4 ";
            ONP OnpMock = new ONP(equation);
            Decimal expected = new Decimal(18);
            Assert.AreEqual(expected, Decimal.Parse(OnpMock.ONPCalculationResult()));

        }

        [TestMethod]
        public void TestEquation2()
        {

            string equation = " ( 8 + 9 ) / (2 + 2) * (2 + 3) * 6 - 8 / 4 ";
            ONP OnpMock = new ONP(equation);
            Decimal expected = new Decimal(125.5);
            Assert.AreEqual(expected, Decimal.Parse(OnpMock.ONPCalculationResult()));

        }

        [TestMethod]
        public void TestEquation3()
        {

            string equation = " ( 8 + 8 )  * (2 + 2) * 6 - 6 / 2 ^ 2 ";
            ONP OnpMock = new ONP(equation);
            Decimal expected = new Decimal(382.5);
            Assert.AreEqual(expected, Decimal.Parse(OnpMock.ONPCalculationResult()));

        }

        [TestMethod]
        public void TestEquationPotega()
        {

            string equation = " 2 ^ 2 ";
            ONP OnpMock = new ONP(equation);
            Decimal expected = new Decimal(4);
            Assert.AreEqual(expected, Decimal.Parse(OnpMock.ONPCalculationResult()));

        }

        [TestMethod]
        public void TestEquationDzielenie()
        {

            string equation = " 8 / 2 ";
            ONP OnpMock = new ONP(equation);
            Decimal expected = new Decimal(4);
            Assert.AreEqual(expected, Decimal.Parse(OnpMock.ONPCalculationResult()));

        }

        [TestMethod]
        public void TestEquationNawiasPotega()
        {

            string equation = " ( 2 + 2 ) ^ 2 ";
            ONP OnpMock = new ONP(equation);
            Decimal expected = new Decimal(16);
            Assert.AreEqual(expected, Decimal.Parse(OnpMock.ONPCalculationResult()));

        }

        [TestMethod]
        public void TestModulo()
        {
            

            string equation = " 2 + 3 * 6 - 8 / 4 + 2 % 4 ";
            ONP OnpMock = new ONP(equation);
            Decimal expected = new Decimal(20);
            Assert.AreEqual(expected, Decimal.Parse(OnpMock.ONPCalculationResult()));

        }

        [TestMethod]
        public void TestModuloAlone()
        {
            string equation = " 2 % 8 ";
            ONP OnpMock = new ONP(equation);
            Decimal expected = new Decimal(2);
            string onpResult = OnpMock.ONPCalculationResult();
            Assert.AreEqual(expected, Decimal.Parse(onpResult));
        }

        [TestMethod]
        public void TestModuloAlone2()
        {

            Console.WriteLine("2 mod 2 = " + 2 % 2 + " expected = 1");
            string equation = " 2 % 2 ";
            ONP OnpMock = new ONP(equation);
            Decimal expected = new Decimal(0);
            string onpResult = OnpMock.ONPCalculationResult();
            Assert.AreEqual(expected, Decimal.Parse(onpResult));

        }

        [TestMethod]
        public void TestBinaryAND()
        {
            long number = 4000000000;            
            LogicalOperations logic = new LogicalOperations(number);
            logic.PreformAND(5);
            Assert.AreEqual(number & 5, logic.ConvertToDecimal());
        }

        [TestMethod]
        public void TestBinaryOR()
        {
            long number = 4000000000;            
            LogicalOperations logic = new LogicalOperations(number);
            logic.PreformOR(5);
            Assert.AreEqual(number | 5, logic.ConvertToDecimal());
        }

        [TestMethod]
        public void TestBinaryXOR()
        {
            long number = 4000000000;          
            LogicalOperations logic = new LogicalOperations(number);
            logic.PreformXOR(5);
            Assert.AreEqual(number ^ 5, logic.ConvertToDecimal());
        }

        [TestMethod]
        public void TestBinaryNOT()
        {
            long number = 4000000000;           
            LogicalOperations logic = new LogicalOperations(number);
            logic.PreformNOT();
            Assert.AreEqual(~number, logic.ConvertToDecimal());
        }

        [TestMethod]
        public void TestBinaryString()
        {
            long number = 4000000000;            
            String binaryRepresentation = Convert.ToString(number, 2);
            LogicalOperations logic = new LogicalOperations(number);            
            Assert.AreEqual(binaryRepresentation, logic.ConvertToBinary());
        }

        [TestMethod]
        public void TestHexString()
        {
            long number = 4000000000;
            String hexRepresentation = Convert.ToString(number, 16);
            LogicalOperations logic = new LogicalOperations(number);
            Assert.AreEqual(hexRepresentation, logic.ConvertToHex());
        }

        [TestMethod]
        public void TestOctalString()
        {
            long number = 4000000000;
            String octalRepresentation = Convert.ToString(number, 8);
            LogicalOperations logic = new LogicalOperations(number);
            Assert.AreEqual(octalRepresentation, logic.ConvertToOctal());
        }
    }
}
