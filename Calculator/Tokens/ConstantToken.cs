using System;
using System.Collections.Generic;
using System.Text;

namespace Calculator
{
    public class ConstantToken : IToken
    {
        public double Value { get; private set; }
        public int Length { get; private set; }

        public ConstantToken(double value, int length)
        {
            Value = value;
            Length = length;
        }
        public ConstantToken(double value)
        {
            Value = value;
        }
        public ConstantToken()
        {
            
        }
        public bool TryParse(string expression, int i)
        {
            if (Char.IsDigit(expression[i]))
            {
                var j = i;

                while (j < expression.Length && Char.IsDigit(expression[j]))
                    j++;

                if (j < expression.Length && expression[j] == '.')
                {
                    j++;
                    while (j < expression.Length && Char.IsDigit(expression[j]))
                        j++;
                }

                Length = j - i;
            
                Value = double.Parse(expression.Substring(i, Length).Replace('.',','));
            
                return true;
            }


            return false;
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
