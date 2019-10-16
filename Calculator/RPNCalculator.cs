using System;
using System.Collections.Generic;
using System.Text;
using Calculator.Tokens;

namespace Calculator
{
    public static class RPNCalculator
    {
        private static Stack<IToken> buffer;
        public static double Calculate(List<IToken> rpn)
        {
            buffer = new Stack<IToken>();

            foreach (var token in rpn)
            {
                if (token is ConstantToken)
                    buffer.Push(token);

                if(token is BinaryOperationToken)
                {
                    var a = buffer.Pop();
                    var b = buffer.Pop();
                    if (a is ConstantToken && b is ConstantToken)
                    {
                        var operation = (BinaryOperationToken)token;
                        buffer.Push(operation.Calculate((ConstantToken)b, (ConstantToken)a));
                    }
                    else
                        throw new FormatException("Формат записи не корректен.");
                }

                if (token is FunctionToken)
                {
                    var x = buffer.Pop();
                    if (x is ConstantToken)
                    {
                        var operation = (FunctionToken)token;
                        buffer.Push(operation.Calculate((ConstantToken)x));
                    }
                    else
                        throw new FormatException("Формат записи не корректен.");
                }
            }

            if (buffer.Count != 1|| !(buffer.Peek()is ConstantToken))
                throw new FormatException("Формат записи не корректен.");

            return ((ConstantToken)buffer.Pop()).Value;
        }
    }
}
