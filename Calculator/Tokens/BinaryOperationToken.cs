using System;
using System.Collections.Generic;
using System.Text;

namespace Calculator.Tokens
{
    public class BinaryOperationToken : IToken
    {
        private Func<double, double, double> operation;
        public char Symbol { get; private set; }

        public int Length { get; private set; }
        public int Priority { get; private set; }

        public Associativity Associativity;
        public BinaryOperationToken(char symbol, Func<double, double, double> operation, int priority, Associativity associativity)
        {
            this.operation = operation;
            this.Symbol = symbol;
            Priority = priority;
            Associativity = associativity;
            Length = 1;

        }

        public ConstantToken Calculate(ConstantToken a, ConstantToken b)
        {
            return new ConstantToken(operation(a.Value, b.Value));
        }
        public bool TryParse(string expression, int i)
        {
            if (expression[i] == Symbol)
                return true;

            return false;
        }

        public override string ToString()
        {
            return Symbol.ToString();
        }
    }
}
