using System;
using System.Text;
using System.Collections.Generic;

namespace lab
{
    public static class RPN
    {
        public static int Calculate(string input)
        {
            var output = GetExpression(input); 
            return Counting(output);
        }

        private static string GetExpression(string input)
        {
            input = GetNormalizeExpression(input);

            var output = new StringBuilder();
            var operStack = new Stack<char>(); 

            for (var i = 0; i < input.Length; i++) 
            {
                if (IsDigit(input[i]))
                {
                    if ((i + 1) != input.Length && IsDigit(input[i + 1]))
                        throw new ArgumentException();
                    while (!IsOperator(input[i]))
                    {
                        output.Append(input[i]); 
                        i++; 

                        if (i == input.Length) 
                            break;
                    }
                    output.Append(" ");
                    i--; 
                }
                else if (IsOperator(input[i]))
                {
                    switch (input[i])
                    {
                        case '(':
                            operStack.Push(input[i]);
                            break;
                        case ')':
                        {
                            var s = operStack.Pop();

                            while (s != '(')
                            {
                                output.Append(s.ToString() + ' ');
                                s = operStack.Pop();
                            }

                            break;
                        }
                        default:
                        {
                            if (operStack.Count > 0)
                                if (GetPriority(input[i]) <= GetPriority(operStack.Peek())) 
                                    output.Append(operStack.Pop() + " "); 

                            operStack.Push(char.Parse(input[i].ToString()));
                            break;
                        }
                    }
                }
                else
                    throw new ArgumentException();
            }

            while (operStack.Count > 0)
                output.Append(operStack.Pop() + " ");

            return output.ToString(); 
        }

        private static string GetNormalizeExpression(string input)
        {
            var result = new StringBuilder();

            foreach (var ch in input)
            {
                if (!IsDelimeter(ch))
                    result.Append(ch);
                if (!IsOperator(ch) && !IsDigitOrLetter(ch) && !IsDelimeter(ch))
                    throw new ArgumentException();
            }
            
            for (var i = 0; i < result.Length; ++i)
                if (result[i] == '!')
                {
                    if ((i + 1) < result.Length && (IsDigitOrLetter(result[i + 1]) || result[i + 1] == '('))
                    {
                        result.Remove(i, 1);
                        result.Insert(i, "(1^");
                        result.Insert(i + 4, ")");
                    }
                    else
                    {
                        throw new ArgumentException();
                    }
                }

            return result.ToString();
        }

        private static int Counting(string input)
        {
            var result = 0; 
            var temp = new Stack<int>();

            for (var i = 0; i < input.Length; i++) 
            {
                if (IsDigit(input[i]))
                {
                    var number = new StringBuilder();

                    while (!IsDelimeter(input[i]) && !IsOperator(input[i])) 
                    {
                        number.Append(input[i]);
                        i++;
                        if (i == input.Length) 
                            break;
                    }
                    temp.Push(int.Parse(number.ToString())); 
                    i--;
                }
                else if (IsOperator(input[i])) 
                {
                    if (temp.Count == 0)
                        throw new ArgumentException();
                    var a = temp.Pop();
                    if (temp.Count == 0)
                        throw new ArgumentException();
                    var b = temp.Pop();

                    if (a / 10 != 0 || b / 10 != 0)
                        throw new ArgumentException();

                    switch (input[i]) 
                    {
                        case '&': result = b & a; break; //И
                        case '|': result = b | a; break; //или
                        case '^': result = b ^ a; break; //XOR
                        case '>': result = b ^ 1 | a; break; //Импликация
                        case '~': result = a == b ? 1 : 0; break; //Эквивалентность
                        case '@': result = (b^1) & (a^1); break; //Стрелка Пирса
                        case '%': result = (b ^ 1) | (a ^ 1); break; //Штрих Шефера
                    }
                    temp.Push(result); 
                }
            }
            if (temp.Peek() / 10 != 0)
                throw new ArgumentException();
            if (temp.Count != 1)
                throw new ArgumentException();

            return temp.Peek(); 
        }

        private static bool IsDelimeter(char c)
        {
            return c == ' ';
        }

        private static bool IsDigit(char c)
        {
            return c == '0' || c == '1';
        }

        private static bool IsDigitOrLetter(char c)
        {
            return IsDigit(c) || char.IsLetter(c);
        }

        private static bool IsOperator(char с)
        {
            return "!&|^>~@%()".IndexOf(с) != -1;
        }

        private static bool IsBracket(char c)
        {
            return c == '(' || c == ')';
        }

        private static byte GetPriority(char s)
        {
            switch (s)
            {
                case '(': return 0;
                case ')': return 1;
                case '~': return 2;
                case '>': return 3;
                case '@': return 4;
                case '%': return 5;
                case '^': return 6;
                case '|': return 7;
                case '&': return 8;
                default: return 9;
            }
        }
    }
}
