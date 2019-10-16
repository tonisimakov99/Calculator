using System;
using System.Collections.Generic;
using System.Text;

namespace Calculator
{
    public interface IToken
    {
        bool TryParse(string expression, int i);
    }
}
