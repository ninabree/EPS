using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExpenseProcessingSystem.Tests
{
    [TestFixture]
    class NumberToTextTests
    {
        private NumberToText _class;

        [OneTimeSetUp]
        public void TestSetup()
        {
            _class = new NumberToText();

        }

        [Test]
        public void NumberToText_ReturnSmallNumericText()
        {
            //Arrange 
            int num1 = 14;

            //Act
            var result = _class.SmallNumberToWords(num1);

            //Assert
            Assert.That(result, Is.EqualTo("FOURTEEN"));
        }

        [Test]
        public void NumberToText_ReturnNumericText()
        {
            //Arrange 
            int num1 = 2014;

            //Act
            var result = _class.NumberToWords(num1);

            //Assert
            Assert.That(result, Is.EqualTo("TWO THOUSAND FOURTEEN"));
        }

        [Test]
        public void NumberToText_ReturnDoubleNumericText()
        {
            //Arrange 
            double num1 = 422014.75;

            //Act
            var result = _class.DoubleNumberToWords(num1);

            //Assert
            Assert.That(result, Is.EqualTo("FOUR HUNDRED TWENTY TWO THOUSAND FOURTEEN AND SEVENTY FIVE CENTAVOS"));
        }

        [Test]
        public void NumberToText_ReturnNegativeDoubleNumericText()
        {
            //Arrange 
            double num1 = -422014.75;

            //Act
            var result = _class.DoubleNumberToWords(num1);

            //Assert
            Assert.That(result, Is.EqualTo("MINUS FOUR HUNDRED TWENTY TWO THOUSAND FOURTEEN AND SEVENTY FIVE CENTAVOS"));
        }

        [Test]
        public void NumberToText_ReturnMillionDoubleNumericText()
        {
            //Arrange 
            double num1 = 10000005422014.75;

            //Act
            var result = _class.DoubleNumberToWords(num1);

            //Assert
            Assert.That(result, Is.EqualTo("FIVE MILLION FOUR HUNDRED TWENTY TWO THOUSAND FOURTEEN AND SEVENTY FIVE CENTAVOS"));
        }
    }
}
