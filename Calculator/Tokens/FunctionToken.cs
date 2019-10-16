using System;
using System.Collections.Generic;
using System.Text;

namespace Calculator.Tokens
{
    public class FunctionToken : IToken
    {
        private Func<double,double> function;
        public string Name { get; private set; }

        public int Length { get; private set; }

        public FunctionType FunctionType;
        public FunctionToken(string name, Func<double, double> function, FunctionType functionType)
        {
            this.function = function;
            this.Name = name;
            FunctionType = functionType;
        }
        public ConstantToken Calculate(ConstantToken x)
        {
            return new ConstantToken(function(x.Value));
        }
        public bool TryParse(string expression, int i)
        {
            var k = i;

            for (int j = 0; j != Name.Length; j++, k++)
            {
                if (expression[k] != Name[j])
                    return false;
            }

            Length = k - i;
    
            return true;
        }

        public override string ToString()
        {
            return Name.ToString();
        }
    }
}
