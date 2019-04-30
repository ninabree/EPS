using NUnit.Framework;
using System;
using ExpenseProcessingSystem;

namespace ExpenseProcessingSystem.Tests
{
    [TestFixture]
    public class SampleTests
    {
        private SampleClassForTesting _class;

        [OneTimeSetUp]
        public void TestSetup()
        {
            _class = new SampleClassForTesting();

        }

        [Test]
        public void SampleClass_Sum_ReturnEqualValue()
        {
            //Arrange 
            float num1 = 2014.43F;
            float num2 = 283.09F;

            //Act
            var result = _class.ReturnSum(num1, num2);

            //Assert
            Assert.That(result, Is.EqualTo(2297.52F));
        }
        [Test]
        public void SampleClass_Diff_ReturnEqualValue()
        {
            //Arrange 
            float num1 = 2014.43F;
            float num2 = 283.09F;

            //Act
            var result = _class.ReturnDiff(num1, num2);

            //Assert
            Assert.That(result, Is.EqualTo(1731.34009F));
        }
        [Test]
        public void SampleClass_Prod_ReturnEqualValue()
        {
            //Arrange 
            float num1 = 2014.43F;
            float num2 = 283.09F;

            //Act
            var result = _class.ReturnProd(num1, num2);

            //Assert
            Assert.That(result, Is.EqualTo(570264.9887F));
        }
        [Test]
        public void SampleClass_Qou_ReturnEqualValue()
        {
            //Arrange 
            float num1 = 2014.43F;
            float num2 = 283.09F;

            //Act
            var result = _class.ReturnQou(num1, num2);

            //Assert
            Assert.That(result, Is.EqualTo(7.11586428F));
        }
    }
}
