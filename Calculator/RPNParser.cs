using System;
using System.Collections.Generic;
using System.Text;
using Calculator.Tokens;
using System.Linq;

namespace Calculator
{
    /// <summary>
    /// Парсер математических выражений, преобразует строку в обратную польскую запись
    /// </summary>
    public class RPNParser
    {
        private List<BinaryOperationToken> BinaryOperations = new List<BinaryOperationToken>();
        private List<FunctionToken> Functions = new List<FunctionToken>();

        private List<IToken> result;
        private Stack<IToken> operationStack;
        /// <summary>
        /// Обрабатывает входную строку и возвращает ее RPN.
        /// </summary>
        /// <param name="expression">Строка представляющая математическое выражение.</param>
        /// <returns>Список полученных токенов в порядке RPN.</returns>
        public List<IToken> Parse(string expression)
        {
            result = new List<IToken>();
            operationStack = new Stack<IToken>();

            var constant = new ConstantToken();
            var openParenthesis = new ParenthesisToken(true);
            var closeParenthesis = new ParenthesisToken(false);

            int i = 0;
            IToken previous = new ConstantToken();
            while (i < expression.Length)
            {
                IToken current;
                if (constant.TryParse(expression, i))        //Если следующий токен константа
                {
                    if (previous is FunctionToken)
                        throw new FormatException("Отсутсвуют скобки у параметра функции");

                    result.Add(new ConstantToken(constant.Value, constant.Length));
                    current = new ConstantToken(constant.Value, constant.Length);
                    i += constant.Length;
                }
                else if (TryParseFunction(expression, i, out var functionToken)) //Если следующий токен функция
                {
                    if (previous is FunctionToken)
                        throw new FormatException("Отсутсвуют скобки у параметра функции");

                    if (functionToken.FunctionType == FunctionType.Postfix)
                        result.Add(functionToken);


                    if (functionToken.FunctionType == FunctionType.Prefix)
                        operationStack.Push(functionToken);

                    current = functionToken;
                    i += functionToken.Length;
                }
                else if (openParenthesis.TryParse(expression, i)) //Если следующий токен открывающая скобка
                {
                    operationStack.Push(new ParenthesisToken(true));
                    current = openParenthesis;
                    i += openParenthesis.Length;
                }
                else if (closeParenthesis.TryParse(expression, i)) //Если следующий токен закрывающая скобка
                {
                    while (operationStack.Count != 0 && !(operationStack.Peek() is ParenthesisToken))
                        result.Add(operationStack.Pop());

                    if (operationStack.Count == 0)
                        throw new FormatException("Количество закрывающих скобок больше, чем открывающих");

                    operationStack.Pop();
                    current = closeParenthesis;
                    i += closeParenthesis.Length;
                }
                else if (TryParseBinaryOperation(expression, i, out var binaryOperation)) //Если следующий токен бинарная операция
                {

                    while ((operationStack.Count != 0 && ((operationStack.Peek() is FunctionToken)) && ((FunctionToken)operationStack.Peek()).FunctionType == FunctionType.Prefix)
                        || (operationStack.Count != 0 && ((operationStack.Peek() is BinaryOperationToken)) && ((((BinaryOperationToken)operationStack.Peek()).Priority > binaryOperation.Priority) ||
                        ((((BinaryOperationToken)operationStack.Peek()).Associativity == Associativity.Left) && ((BinaryOperationToken)operationStack.Peek()).Priority == binaryOperation.Priority))))
                    {
                        result.Add(operationStack.Pop());
                    }

                    //Есть операции которые являются одновременно унарными и бинарными, среди них алгеброических только 2: унарный минус и комлексное сопряженное.
                    //Комплексные числа в рамках этого парсера не рассматриваются, введение унарного минуса как отдельного вида токенов усложнит код, кроме того унарные операции 
                    //уже покрыты токенами функций, но добавлять его в качестве функции не логично, потому что у него отсутсвуют характерные скобки вокруг парметра, поэтому я решил 
                    //заменить его костылём. Унарный минус интерпретируется как бинарный, но он будет вычитать число из 0. Например 2+(-3) => 2+(0-3).
                    if (binaryOperation.ToString() == "-")
                    {
                        if ((previous is ParenthesisToken && ((ParenthesisToken)previous).isOpen) || i == 0)
                        {
                            result.Add(new ConstantToken(0, 1));
                        }
                    }
                        
                    operationStack.Push(binaryOperation);
                    current = binaryOperation;
                    i += binaryOperation.Length;
                }
                else
                {
                    throw new FormatException("Отсутствует подходящий токен");
                }
                previous = current;
            }

            while (operationStack.Count != 0)
                result.Add(operationStack.Pop());

            CheckResult(result);

            return result;
        }
        private void CheckResult(List<IToken> result)
        {

            if (result.Count == 0)
                throw new FormatException("Строка была пуста.");

            int constantTokenCount = 0;
            foreach(var token in result)
            {
                if (token is ParenthesisToken)
                    throw new FormatException("Количество открывающих скобок больше, чем закрывающих.");

                if (token is ConstantToken)
                    constantTokenCount++;

                if (token is BinaryOperationToken && constantTokenCount >= 2)
                    constantTokenCount--;
                else if (token is BinaryOperationToken && constantTokenCount < 2)
                    throw new FormatException("Отсутствуют 1 или 2 операнда у бинарного оператора.");

                if(token is FunctionToken && constantTokenCount < 1)
                    throw new FormatException("Отсутствует параметр функции.");
            }
        }

        //Перед тем как парсить строку необходимо инициализировать парсер, добавить в него токены которые могут быть в выражении
        
        /// <summary>
        /// Добавляет в парсер токен бинарной операции,т.е операции вида a*b, где 8 - любая операция .
        /// </summary>
        /// <param name="token"></param>
        public void AddBinaryOperation(BinaryOperationToken token)
        {
            var identicalBinaryTokens = BinaryOperations.Where(t => t.Symbol == token.Symbol);
            var identicalFunctionTokens = Functions.Where(t => t.Name == token.Symbol.ToString());

            if (identicalBinaryTokens.Count() != 0 || identicalFunctionTokens.Count() != 0)
                throw new Exception("Такой токен уже есть");
            else
                BinaryOperations.Add(token);
        }
        
        /// <summary>
        /// Добавляет в парсер токен функции. 
        /// </summary>
        /// <param name="token"></param>
        public void AddFunction(FunctionToken token)
        {
            var identicalBinaryTokens = BinaryOperations.Where(t => token.Name.Length == 1 && token.Name == t.Symbol.ToString());
            var identicalFunctionTokens = Functions.Where(t => t.Name == token.Name);

            if (identicalBinaryTokens.Count() != 0 || identicalFunctionTokens.Count() != 0)
                throw new Exception("Такой токен уже есть");
            else
                Functions.Add(token);
        }

        private bool TryParseBinaryOperation(string expression, int i, out BinaryOperationToken binaryOperation)
        {
            binaryOperation = default(BinaryOperationToken);
            foreach (var operation in BinaryOperations)
            {
                if (operation.TryParse(expression, i))
                {
                    binaryOperation = operation;
                    return true;
                }
            }
            return false;
        }

        private bool TryParseFunction(string expression, int i, out FunctionToken functionToken)
        {
            functionToken = default(FunctionToken);
            foreach (var function in Functions)
            {
                if (function.TryParse(expression, i))
                {
                    functionToken = function;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Возвращает уже инициализированный парсер, в котором уже есть большинство стандартных функций,
        /// избавлят от необходимости инициализировать самостоятельно.
        /// </summary>
        /// <returns></returns>
        public static RPNParser Default()
        {
            var rpnParser = new RPNParser();
            rpnParser.AddBinaryOperation(new BinaryOperationToken('-', (a, b) => a - b, 0, Associativity.Left));
            rpnParser.AddBinaryOperation(new BinaryOperationToken('+', (a, b) => a + b, 0, Associativity.Left));
            rpnParser.AddBinaryOperation(new BinaryOperationToken('*', (a, b) => a * b, 1, Associativity.Left));
            rpnParser.AddBinaryOperation(new BinaryOperationToken('/', (a, b) => a / b, 1, Associativity.Left));
            rpnParser.AddBinaryOperation(new BinaryOperationToken('%', (a, b) => a % b, 1, Associativity.Left));
            rpnParser.AddBinaryOperation(new BinaryOperationToken('^', (a, b) => Math.Pow(a, b), 2, Associativity.Right));

            rpnParser.AddFunction(new FunctionToken("!", (x) =>
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

            rpnParser.AddFunction(new FunctionToken("sin", (x) => Math.Sin(x), FunctionType.Prefix));
            rpnParser.AddFunction(new FunctionToken("cos", (x) => Math.Cos(x), FunctionType.Prefix));
            rpnParser.AddFunction(new FunctionToken("tan", (x) => Math.Tan(x), FunctionType.Prefix));
            rpnParser.AddFunction(new FunctionToken("sqrt", (x) => Math.Sqrt(x), FunctionType.Prefix));
            rpnParser.AddFunction(new FunctionToken("abs", (x) => Math.Abs(x),FunctionType.Prefix));
            rpnParser.AddFunction(new FunctionToken("log", (x) => Math.Log(x), FunctionType.Prefix));
            rpnParser.AddFunction(new FunctionToken("log10", (x) => Math.Log10(x), FunctionType.Prefix));
            rpnParser.AddFunction(new FunctionToken("round", (x) => Math.Round(x), FunctionType.Prefix));
            rpnParser.AddFunction(new FunctionToken("truncate", (x) => Math.Truncate(x), FunctionType.Prefix));
            rpnParser.AddFunction(new FunctionToken("arccosine", (x) => Math.Acos(x), FunctionType.Prefix));
            rpnParser.AddFunction(new FunctionToken("arcsine", (x) => Math.Asin(x), FunctionType.Prefix));
            rpnParser.AddFunction(new FunctionToken("arctan", (x) => Math.Atan(x), FunctionType.Prefix));

            return rpnParser;
        }
    }
}
