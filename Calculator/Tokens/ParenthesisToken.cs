using System;
using System.Collections.Generic;
using System.Text;

namespace Calculator.Tokens
{
    class ParenthesisToken : IToken
    {
        public bool isOpen { get; private set; }
        public ParenthesisToken(bool isOpen)
        {
            this.isOpen = isOpen;
            Length = 1;
        }

        public int Length { get; private set; }

        public bool TryParse(string expression, int i)
        {
            if (isOpen && expression[i] == '(')
            {
                return true;
            }

            if (!isOpen && expression[i] == ')')
            {
                return true;
            }

            return false;
        }

        public override string ToString()
        {
            return isOpen ? '('.ToString() : ')'.ToString();
        }
    }
}
