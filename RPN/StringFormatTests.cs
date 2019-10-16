using Microsoft.VisualStudio.TestTools.UnitTesting;
using Calculator;
using Calculator.Tokens;
using System;

namespace RPNParserTests
{
    [TestClass]
    public class StringFormatTests
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
        public void OpenParenthesis()
        {
            Assert.ThrowsException<FormatException>(() => parser.Parse("(4+1)+(5-3"));
        }

        [TestMethod]
        public void CloseParenthesis()
        {
            Assert.ThrowsException<FormatException>(() => parser.Parse("5+3)"));
        }

        [TestMethod]
        public void OperationWithoutOperands()
        {
            Assert.ThrowsException<FormatException>(() => parser.Parse("5++7"));
            Assert.ThrowsException<FormatException>(() => parser.Parse("*"));
        }

        [TestMethod]
        public void OperationWithoutOperands2()
        {
            Assert.ThrowsException<FormatException>(() => parser.Parse("5+7-3+"));
            Assert.ThrowsException<FormatException>(() => parser.Parse("5+7-3+-2"));
        }
        [TestMethod]
        public void FunctintionWithoutParameter()
        {
            Assert.ThrowsException<FormatException>(() => parser.Parse("Foo()"));
        }

        [TestMethod]
        public void FunctintionWithoutParameter2()
        {
            Assert.ThrowsException<FormatException>(() => parser.Parse("2+Foo()-4*Foo(3)"));
        }
        [TestMethod]
        public void FunctionWithoutParameter3()
        {
            Assert.ThrowsException<FormatException>(() => parser.Parse("Foo(-)"));
        }

        [TestMethod]
        public void EmptyString()
        {
            Assert.ThrowsException<FormatException>(() => parser.Parse(""));
        }

        [TestMethod]
        public void EmptyParenthesis()
        {
            Assert.ThrowsException<FormatException>(() => parser.Parse("()"));
        }

        [TestMethod]
        public void FunctionWithoutParenthesis()
        {
            Assert.ThrowsException<FormatException>(() => parser.Parse("Foo)"));
        }

        [TestMethod]
        public void FunctionWithoutParenthesis2()
        {
            Assert.ThrowsException<FormatException>(() => parser.Parse("Foo5"));
        }
    }
}
