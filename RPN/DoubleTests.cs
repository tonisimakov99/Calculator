using Microsoft.VisualStudio.TestTools.UnitTesting;
using Calculator;
using Calculator.Tokens;
using System;
using System.Collections.Generic;

namespace RPNParserTests
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
        public void OneDouble()
        {
            var rpn = parser.Parse("6.2");
            Assert.AreEqual(1, rpn.Count);
            var delta = 6.2 - ((ConstantToken)rpn[0]).Value;
            Assert.IsTrue((delta < 0 ? -1 * delta : delta) < Double.Epsilon);
        }

        [TestMethod]
        public void OneNegativeDouble()
        {
            var rpn = parser.Parse("-6.2");
            Assert.AreEqual(3, rpn.Count);
            Assert.AreEqual(0, ((ConstantToken)rpn[0]).Value);
            var delta = 6.2 - ((ConstantToken)rpn[1]).Value;
            Assert.IsTrue((delta < 0 ? -1 * delta : delta) < Double.Epsilon);
            Assert.AreEqual('-', ((BinaryOperationToken)rpn[2]).Symbol);
        }

        [TestMethod]
        public void SumDouble()
        {
            var rpn = parser.Parse("6.2+4.8");
            Assert.AreEqual(3, rpn.Count);
            var delta = 6.2 - ((ConstantToken)rpn[0]).Value;
            Assert.IsTrue((delta < 0 ? -1 * delta : delta) < Double.Epsilon);
           
            delta = 4.8 - ((ConstantToken)rpn[1]).Value;
            Assert.IsTrue((delta < 0 ? -1 * delta : delta) < Double.Epsilon);
            Assert.AreEqual('+', ((BinaryOperationToken)rpn[2]).Symbol);

        }
    }
}
