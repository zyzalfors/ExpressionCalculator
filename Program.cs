using System;
using System.Data;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Calc
{
	
    internal static class Calc
    {
        private static DataTable table;

        private static readonly string[] operations = {
                                                       "Pi",
                                                       "*", 
                                                       "/", 
                                                       "+",
                                                       "-", 
                                                       "Abs", 
                                                       "Acos",
                                                       "Acosh",
                                                       "Asin",
                                                       "Asinh",
                                                       "Atan",
                                                       "Atan2",   
                                                       "Atanh",
                                                       "Cbrt",
                                                       "Ceil",
                                                       "Cos",
                                                       "Cosh",
                                                       "Exp",
                                                       "Floor",
                                                       "Log",
                                                       "Ln",
                                                       "Log10",
                                                       "Log2",
                                                       "Pow",
                                                       "Sign",
                                                       "Sin",
                                                       "Sinh",
                                                       "Sqrt",
                                                       "Tan",
                                                       "Tanh"
                                                      };

        private static readonly string pattern = @"[+-]?([0-9]+\.?[0-9]*|\.[0-9]+)(E-([0-9]+))?";

        private static string Digest(string expression)
        {
            string digestedExpression = expression.Replace(" ", string.Empty);
            string oldDigestedExpression = string.Empty;
            while(digestedExpression != oldDigestedExpression)
            {
                oldDigestedExpression = digestedExpression;
                foreach(string operation in operations)
                {
                    if(operation == "Pi")
                    { 
                        foreach(Match match in Regex.Matches(digestedExpression, operation, RegexOptions.IgnoreCase))
                        {
                            digestedExpression = digestedExpression.Replace(match.Value, CalculateOperation(operation).ToString());
                        }
                    }
                    else if(operation == "*")
                    {
                        foreach(Match match in Regex.Matches(digestedExpression, pattern + @"\*" + pattern, RegexOptions.IgnoreCase))
                        {
                            digestedExpression = digestedExpression.Replace(match.Value, "+" + CalculateExpression(match.Value));
                        }
                    }
                    else if(operation == "/")
                    {
                        foreach(Match match in Regex.Matches(digestedExpression, pattern + "/" + pattern, RegexOptions.IgnoreCase))
                        {
                            digestedExpression = digestedExpression.Replace(match.Value, "+" + CalculateExpression(match.Value));
                        }
                    }
                    else if(operation == "+")
                    {
                        foreach(Match match in Regex.Matches(digestedExpression, pattern + @"\+" + pattern, RegexOptions.IgnoreCase))
                        {
                            digestedExpression = digestedExpression.Replace(match.Value, "+" + CalculateExpression(match.Value));
                        }
                    }
                    else if (operation == "-")
                    {
                        foreach(Match match in Regex.Matches(digestedExpression, pattern + "-" + pattern, RegexOptions.IgnoreCase))
                        {
                            digestedExpression = digestedExpression.Replace(match.Value, "+" + CalculateExpression(match.Value));
                        }
                    }
                    else if(operation == "Pow" || operation == "Atan2")
                    {
                        foreach(Match globalMatch in Regex.Matches(digestedExpression, operation + @"\(" + pattern + "," + pattern + @"\)", RegexOptions.IgnoreCase))
                        {
                            foreach(Match numMatch in Regex.Matches(globalMatch.Value, pattern, RegexOptions.IgnoreCase))
                            {
                                if(numMatch.NextMatch().Value.Trim() == string.Empty) { continue; }
                                digestedExpression = digestedExpression.Replace(globalMatch.Value, CalculateOperation(operation, Convert.ToDouble(numMatch.Value), Convert.ToDouble(numMatch.NextMatch().Value)).ToString());
                            }
                        }
                    }
                    else
                    {
                        foreach(Match globalMatch in Regex.Matches(digestedExpression, operation + @"\(" + pattern + @"\)", RegexOptions.IgnoreCase))
                        {
                            foreach(Match numMatch in Regex.Matches(globalMatch.Value, pattern, RegexOptions.IgnoreCase))
                            {
                                digestedExpression = digestedExpression.Replace(globalMatch.Value, CalculateOperation(operation, Convert.ToDouble(numMatch.Value)).ToString());
                            }
                        }
                    }
                }
            }
            return digestedExpression;
        }

        private static double CalculateOperation(string operation, double x = 1, double y = 1)
        {
            switch(operation)
            {
                case "+":
                    return x + y;
                case "-":
                    return x - y;
                case "*":
                    return x * y;
                case "/":
                    return x / y;
                case "Abs":
                    return Math.Abs(x);
                case "Acos":
                    return Math.Acos(x);
                case "Acosh":
                    return Math.Acosh(x);
                case "Asin":
                    return Math.Asin(x);
                case "Asinh":
                    return Math.Asinh(x);
                case "Atan":
                    return Math.Atan(x);
                case "Atan2":
                    return Math.Atan2(x, y);
                case "Atanh":
                    return Math.Atanh(x);
                case "Cbrt":
                    return Math.Cbrt(x);
                case "Ceil":
                    return Math.Ceiling(x);
                case "Cos":
                    return Math.Cos(x);
                case "Cosh":
                    return Math.Cosh(x);
                case "Exp":
                    return Math.Exp(x);
                case "Floor":
                    return Math.Floor(x);
                case "Log":
                case "Ln":
                    return Math.Log(x);
                case "Log10":
                    return Math.Log10(x);
                case "Log2":
                    return Math.Log2(x);
                case "Pi":
                    return Math.PI;
                case "Pow":
                    return Math.Pow(x, y);
                case "Sign":
                    return Math.Sign(x);
                case "Sin":
                    return Math.Sin(x);
                case "Sinh":
                    return Math.Sinh(x);
                case "Sqrt":
                    return Math.Sqrt(x);
                case "Tan":
                    return Math.Tan(x);
                case "Tanh":
                    return Math.Tanh(x);
                default:
                    return 0;
            }
        }

        private static string CalculateExpression(string expression)
        {
            try
            {
				string result = table.Compute(expression, null).ToString();
                return result;
            }
            catch
            {
                return "Invalid expression";
            }        
        }

        private static void Main(string[] args)
        {
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
            CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en-US");
            table = new DataTable();
            if (args.Length == 1)
            {
                Console.WriteLine(CalculateExpression(Digest(args[0])));
                return;
            }            
            Console.WriteLine("Expression Calculator v1.0.0");
            while(true)
            {
                Console.Write("> ");
                Console.WriteLine(CalculateExpression(Digest(Console.ReadLine())));
            }
        }
    }
}
