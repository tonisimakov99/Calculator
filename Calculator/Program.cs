using System;
using Calculator.Tokens;
using System.Collections.Generic;
namespace Calculator
{
    class Program
    {
        static void Main(string[] args)
        {
            var rpnParser = RPNParser.Default();
            while(true)
            {
                //
                Console.WriteLine("Введите выражение:");
                var expression = Console.ReadLine();
                
                try
                {
                    var rpn = rpnParser.Parse(expression);
                    var result = RPNCalculator.Calculate(rpn);
                    Console.WriteLine(String.Format("={0:F10}", result));
                }
                catch(FormatException e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
    }
}
