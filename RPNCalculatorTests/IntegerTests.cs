using Microsoft.VisualStudio.TestTools.UnitTesting;
using Calculator;
using Calculator.Tokens;
using System;

namespace RPNCalculatorTests
{
    [TestClass]
    public class IntegerTests
    {
        private RPNParser parser;
        [TestInitialize]
        public void Initialization()
        {
            parser = new RPNParser();
            parser.AddBinaryOperation(new BinaryOperationToken('+', (a, b) => a + b, 0, Associativity.Left));
            parser.AddBinaryOperation(new BinaryOperationToken('-', (a, b) => a - b, 0, Associativity.Left));
            parser.AddBinaryOperation(new BinaryOperationToken('*', (a, b) => a * b, 1, Associativity.Left));
            parser.AddBinaryOperation(new BinaryOperationToken('%', (a, b) => a % b, 1, Associativity.Left));
            parser.AddBinaryOperation(new BinaryOperationToken('^', (a, b) => Math.Pow(a, b), 2, Associativity.Right));
            parser.AddFunction(new FunctionToken("Foo", x => x + 5, FunctionType.Prefix));
            parser.AddFunction(new FunctionToken("!", (x) =>
            {
                if (x < 0 || x - (int)x > Double.Epsilon)
                    throw new Exception("Факториал определен для целых положительных чисел");

                double result = 1;
                for (int i = 2; i != (int)x + 1; i++)
                {
                    result *= i;
                }
                return result;
            }, FunctionType.Postfix));
        }

        [TestMethod]
        public void OneInteger()
        {
            var rpn = parser.Parse("6");
            var result = RPNCalculator.Calculate(rpn);
            Assert.AreEqual(0, result.CompareTo(6));
        }

        [TestMethod]
        public void OneNegativeInteger()
        {
            var rpn = parser.Parse("-6");
            var result = RPNCalculator.Calculate(rpn);
            Assert.AreEqual(0, result.CompareTo(-6));
        }
        [TestMethod]
        public void SumPositiveAndNegativeInteger()
        {
            var rpn = parser.Parse("1+(-6)");
            var result = RPNCalculator.Calculate(rpn);
            Assert.AreEqual(0, result.CompareTo(-5));
        }
        [TestMethod]
        public void FactorialInteger()
        {
            var rpn = parser.Parse("6!");
            var result = RPNCalculator.Calculate(rpn);
            Assert.AreEqual(0, result.CompareTo(720));
        }


        [TestMethod]
        public void SumInteger()
        {
            var rpn = parser.Parse("6+4");
            var result = RPNCalculator.Calculate(rpn);
            Assert.AreEqual(0, result.CompareTo(10));
        }

        [TestMethod]
        public void SubtractionInteger()
        {
            var rpn = parser.Parse("6-4");
            var result = RPNCalculator.Calculate(rpn);
            Assert.AreEqual(0, result.CompareTo(2));
        }
        [TestMethod]
        public void MultiplySumInteger()
        {
            var rpn = parser.Parse("6+4*5");
            var result = RPNCalculator.Calculate(rpn);
            Assert.AreEqual(0, result.CompareTo(26));
        }
        [TestMethod]
        public void ParenthesisInteger()
        {
            var rpn = parser.Parse("(6+4)*5");
            var result = RPNCalculator.Calculate(rpn);
            Assert.AreEqual(0, result.CompareTo(50));
        }
        [TestMethod]
        public void UserFunctionInteger()
        {
            var rpn = parser.Parse("6+Foo(5)");
            var result = RPNCalculator.Calculate(rpn);
            Assert.AreEqual(0, result.CompareTo(16));
        }
        [TestMethod]
        public void CompoundDegreeInteger()
        {
            var rpn = parser.Parse("2^3^4");
            var result = RPNCalculator.Calculate(rpn);
            Assert.AreEqual(0, result.CompareTo(2417851639229258349412352.0));
        }
        [TestMethod]
        public void CompoundExpressionInteger()
        {
            var rpn = parser.Parse("-2+7-5+(7+8)*(9^7*(5-3+Foo(-10))-4)+45+(-2)+((4)^(1)*(6)-(2)+(9))");
            var result = RPNCalculator.Calculate(rpn);
            Assert.AreEqual(0, result.CompareTo(-215233591));
        }
        [TestMethod]
        public void CompoundExpression2Integer()
        {
            var rpn = parser.Parse("((6)-(2))");
            var result = RPNCalculator.Calculate(rpn);
            Assert.AreEqual(0, result.CompareTo(4));
        }
        [TestMethod]
        public void CompoundExpression3Integer()
        {
            var rpn = parser.Parse("((4)^(1)*(6)-(2)+(9))");
            var result = RPNCalculator.Calculate(rpn);
            Assert.AreEqual(0, result.CompareTo(31));
        }
        [TestMethod]
        public void CompoundExpression4Integer()
        {
            var rpn = parser.Parse("-2+7-5+(7+8)*(9^7*(5-3+Foo(-10))-4)");
            var result = RPNCalculator.Calculate(rpn);
            Assert.AreEqual(0, result.CompareTo(-215233665));
        }
    }
}
