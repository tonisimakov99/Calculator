using Microsoft.VisualStudio.TestTools.UnitTesting;
using Calculator;
using Calculator.Tokens;
using System;

namespace RPNParserTests
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
            parser.AddBinaryOperation(new BinaryOperationToken('^', (a, b) => Math.Pow(a,b), 2, Associativity.Right));
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
            Assert.AreEqual(1, rpn.Count);
            Assert.AreEqual(6,((ConstantToken)rpn[0]).Value);
        }

        [TestMethod]
        public void OneNegativeInteger()
        {
            var rpn = parser.Parse("-6");
            Assert.AreEqual(3, rpn.Count);
            Assert.AreEqual(0, ((ConstantToken)rpn[0]).Value);
            Assert.AreEqual(6, ((ConstantToken)rpn[1]).Value);
            Assert.AreEqual('-', ((BinaryOperationToken)rpn[2]).Symbol);
        }
        [TestMethod]
        public void SumPositiveAndNegativeInteger()
        {
            var rpn = parser.Parse("1+(-6)");//1+0-6
            Assert.AreEqual(5, rpn.Count);
            Assert.AreEqual(1, ((ConstantToken)rpn[0]).Value);
            Assert.AreEqual(0, ((ConstantToken)rpn[1]).Value);
            Assert.AreEqual(6, ((ConstantToken)rpn[2]).Value);
            Assert.AreEqual('-', ((BinaryOperationToken)rpn[3]).Symbol);
            Assert.AreEqual('+', ((BinaryOperationToken)rpn[4]).Symbol);
        }
        [TestMethod]
        public void FactorialInteger()
        {
            var rpn = parser.Parse("6!");
            Assert.AreEqual(2, rpn.Count);
            Assert.AreEqual(6, ((ConstantToken)rpn[0]).Value);
            Assert.AreEqual("!",((FunctionToken)rpn[1]).Name);
        }


        [TestMethod]
        public void SumInteger()
        {
            var rpn = parser.Parse("6+4");
            Assert.AreEqual(3, rpn.Count);
            Assert.AreEqual(6, ((ConstantToken)rpn[0]).Value);
            Assert.AreEqual(4, ((ConstantToken)rpn[1]).Value);
            Assert.AreEqual('+', ((BinaryOperationToken)rpn[2]).Symbol);
            
        }

        [TestMethod]
        public void SubtractionInteger()
        {
            var rpn = parser.Parse("6-4");
            Assert.AreEqual(3, rpn.Count);
            Assert.AreEqual(6, ((ConstantToken)rpn[0]).Value);
            Assert.AreEqual(4, ((ConstantToken)rpn[1]).Value);
            Assert.AreEqual('-', ((BinaryOperationToken)rpn[2]).Symbol);

        }
        [TestMethod]
        public void MultiplySumInteger()
        {
            var rpn = parser.Parse("6+4*5");
            Assert.AreEqual(5, rpn.Count);
            Assert.AreEqual(6, ((ConstantToken)rpn[0]).Value);
            Assert.AreEqual(4, ((ConstantToken)rpn[1]).Value);
            Assert.AreEqual(5, ((ConstantToken)rpn[2]).Value);
            Assert.AreEqual('*', ((BinaryOperationToken)rpn[3]).Symbol);
            Assert.AreEqual('+', ((BinaryOperationToken)rpn[4]).Symbol);
        }
        [TestMethod]
        public void ParenthesisInteger()
        {
            var rpn = parser.Parse("(6+4)*5");
            Assert.AreEqual(5, rpn.Count);
            Assert.AreEqual(6, ((ConstantToken)rpn[0]).Value);
            Assert.AreEqual(4, ((ConstantToken)rpn[1]).Value);
            Assert.AreEqual('+', ((BinaryOperationToken)rpn[2]).Symbol);
            Assert.AreEqual(5, ((ConstantToken)rpn[3]).Value);
            Assert.AreEqual('*', ((BinaryOperationToken)rpn[4]).Symbol);
        }
        [TestMethod]
        public void UserFunctionInteger()
        {
            var rpn = parser.Parse("6+Foo(5)");
            Assert.AreEqual(4, rpn.Count);
            Assert.AreEqual(6, ((ConstantToken)rpn[0]).Value);
            Assert.AreEqual(5, ((ConstantToken)rpn[1]).Value);
            Assert.AreEqual("Foo", ((FunctionToken)rpn[2]).Name);
        }
        [TestMethod]
        public void CompoundDegreeInteger()
        {
            var rpn = parser.Parse("2^3^4");
            Assert.AreEqual(5, rpn.Count);
            Assert.AreEqual(2, ((ConstantToken)rpn[0]).Value);
            Assert.AreEqual(3, ((ConstantToken)rpn[1]).Value);
            Assert.AreEqual(4, ((ConstantToken)rpn[2]).Value);
            Assert.AreEqual('^', ((BinaryOperationToken)rpn[3]).Symbol);
            Assert.AreEqual('^', ((BinaryOperationToken)rpn[4]).Symbol);
        }
        [TestMethod]
        public void FunctionInFunction()
        {
            var rpn = parser.Parse("Foo(Foo(5))");
            Assert.AreEqual(3, rpn.Count);
            Assert.AreEqual(5, ((ConstantToken)rpn[0]).Value);
            Assert.AreEqual("Foo", ((FunctionToken)rpn[1]).Name);
            Assert.AreEqual("Foo", ((FunctionToken)rpn[1]).Name);
        }
    }
}
