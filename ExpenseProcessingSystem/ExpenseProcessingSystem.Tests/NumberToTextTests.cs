using NUnit.Framework;
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
            var result = _class.decimalNumberToWords(num1);

            //Assert
            Assert.That(result, Is.EqualTo("TWO THOUSAND FOURTEEN"));
        }

        [Test]
        public void NumberToText_ReturnDoubleNumericText()
        {
            //Arrange 
            decimal num1 = 422014.75M;

            //Act
            var result = _class.decimalNumberToWords(num1);

            //Assert
            Assert.That(result, Is.EqualTo("FOUR HUNDRED TWENTY TWO THOUSAND FOURTEEN AND 75/100"));
        }

        [Test]
        public void NumberToText_ReturnNegativeDoubleNumericText()
        {
            //Arrange 
            decimal num1 = -422014.75M;

            //Act
            var result = _class.decimalNumberToWords(num1);

            //Assert
            Assert.That(result, Is.EqualTo("MINUS FOUR HUNDRED TWENTY TWO THOUSAND FOURTEEN AND 75/100"));
        }

        [Test]
        public void NumberToText_ReturnMillionDoubleNumericText()
        {
            //Arrange 
            decimal num1 = 10000005422014.75M;

            //Act
            var result = _class.decimalNumberToWords(num1);

            //Assert
            Assert.That(result, Is.EqualTo("TEN TRILLION FIVE MILLION FOUR HUNDRED TWENTY TWO THOUSAND FOURTEEN AND 75/100"));
        }

        [Test]
        public void NumberToText_ReturnNoDecimalNumericText()
        {
            //Arrange 
            decimal num1 = 10000005422014M;

            //Act
            var result = _class.decimalNumberToWords(num1);

            //Assert
            Assert.That(result, Is.EqualTo("TEN TRILLION FIVE MILLION FOUR HUNDRED TWENTY TWO THOUSAND FOURTEEN"));
        }

        [Test]
        public void NumberToText_ReturnWithCurrencyNumericText()
        {
            //Arrange 
            decimal num1 = 1000000005422014.50M;

            //Act
            var result = _class.decimalNumberToWords(num1, "PHP");

            //Assert
            Assert.That(result, Is.EqualTo("ONE QUADRILLION FIVE MILLION FOUR HUNDRED TWENTY TWO THOUSAND FOURTEEN AND 50/100 PESOS ONLY"));
        }
    }
}
