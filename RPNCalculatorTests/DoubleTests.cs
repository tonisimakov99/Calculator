using Microsoft.VisualStudio.TestTools.UnitTesting;
using Calculator;
using Calculator.Tokens;
using System;

namespace RPNCalculatorTests
{
    [TestClass]
    public class DoubleTests
    {
        private RPNParser parser;
        [TestInitialize]
        public void Initialization()
        {
            parser = new RPNParser();
            parser.AddBinaryOperation(new BinaryOperationToken('+', (a, b) => a + b, 0, Associativity.Left));
            parser.AddBinaryOperation(new BinaryOperationToken('-', (a, b) => a - b, 0, Associativity.Left));
            parser.AddBinaryOperation(new BinaryOperationToken('*', (a, b) => a * b, 1, Associativity.Left));
            parser.AddBinaryOperation(new BinaryOperationToken('/', (a, b) => a / b, 1, Associativity.Left));
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
        public void OneDouble()
        {
            var rpn = parser.Parse("6.4");
            var result = RPNCalculator.Calculate(rpn);
            Assert.AreEqual(0, result.CompareTo(6.4));
        }

        [TestMethod]
        public void OneNegativeDouble()
        {
            var rpn = parser.Parse("-6.1");
            var result = RPNCalculator.Calculate(rpn);
            Assert.AreEqual(0, result.CompareTo(-6.1));
        }

        //Не проходит в связи с особеннстью хранения вещественного числа в памяти
        [TestMethod]
        public void SumPositiveAndNegativeDouble()
        {
            var rpn = parser.Parse("1.3+(-6.4)");
            var result = RPNCalculator.Calculate(rpn);
            Assert.AreEqual(0, result.CompareTo(-5.1));
        }
        
        [TestMethod]
        public void SumDouble()
        {
            var rpn = parser.Parse("6.356+4.9");
            var result = RPNCalculator.Calculate(rpn);
            Assert.AreEqual(0, result.CompareTo(11.256));

        }

        //Не проходит в связи с особеннстью хранения вещественного числа в памяти
        [TestMethod]
        public void SubtractionDouble()
        {
            var rpn = parser.Parse("5.1-8.2");
            var result = RPNCalculator.Calculate(rpn);
            Assert.AreEqual(0, result.CompareTo(3.1));

        }
        [TestMethod]
        public void MultiplySumDouble()
        {
            var rpn = parser.Parse("6.0+4.3*5.2");
            var result = RPNCalculator.Calculate(rpn);
            Assert.AreEqual(0, result.CompareTo(28.36));
        }
        [TestMethod]
        public void ParenthesisDouble()
        {
            var rpn = parser.Parse("(6.0+4.3)*5.2");
            var result = RPNCalculator.Calculate(rpn);
            Assert.AreEqual(0, result.CompareTo(53.56));
        }

        //Не проходит в связи с особеннстью хранения вещественного числа в памяти
        [TestMethod]
        public void CompoundDegreeDouble()
        {
            var rpn = parser.Parse("2.5^1.8^1.1");
            var result = RPNCalculator.Calculate(rpn);
            Assert.AreEqual(0, result.CompareTo(5.7498521724779302));
        }

        //Не проходит в связи с особеннстью хранения вещественного числа в памяти
        [TestMethod]
        public void CompoundExpressionDouble()
        {
            var rpn = parser.Parse("-2.8+7.1-5.9+(7.4+8)*(9.2^7*(5.1-3+Foo(-10))-4.123)+45.2378+(-2.321)+((4.908)^(1)*(6)-(2)+(9))");
            var result = RPNCalculator.Calculate(rpn);
            Assert.AreEqual(0, result.CompareTo(-215233591));
        }

        [TestMethod ]
        public void CompoundExpression2Double()
        {
            var rpn = parser.Parse("9/6/2/10");
            var result = RPNCalculator.Calculate(rpn);
            Assert.AreEqual(0, result.CompareTo(0.075));
        }

        //Не проходит в связи с особеннстью хранения вещественного числа в памяти
        [TestMethod]
        public void CompoundExpression3Double()
        {
            var rpn = parser.Parse("9/(6*5+4)/2/10");
            var result = RPNCalculator.Calculate(rpn);
            Assert.AreEqual(0, result.CompareTo(0.0132352941176471));
        }
    }
}
