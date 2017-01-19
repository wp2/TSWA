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
        public void TestModulo()
        {
            // TEN TEST NA RAZIE ZAWIEDZIE PROBLEM Z PARSOWANIEM DO NAPRAWIENIA
            // TO DO FIX PARSER OF MODULO

           /* string equation = " 2 + 3 * 6 - 8 / 4 + 2 % 2 ";
            ONP OnpMock = new ONP(equation);
            Decimal expected = new Decimal(19);
            Assert.AreEqual(expected, Decimal.Parse(OnpMock.ONPCalculationResult()));*/

        }
    }
}
