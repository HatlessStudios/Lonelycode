using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Jint;

namespace Lonelycode
{
    public class Interpreter
    {
        public static void Main(String[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Please specify the path of the file to execute");
                return;
            }
            Engine engine = new Engine();
            StreamReader reader = new StreamReader(args[0], Encoding.UTF8);
            int count = 0;
            long total = 1L;
            char[] buffer = new char[4096];
            Console.WriteLine("Parsing file");
            while ((count = reader.Read(buffer, 0, buffer.Length)) > 0)
            {
                for (int index = 0; index < count; index++)
                {
                    if (buffer[index] != ' ')
                    {
                        Console.WriteLine("Illegal character at position %d: %s", index, buffer[index]);
                        return;
                    }
                }
                total += count;
            }
            StringBuilder js = new StringBuilder();
            Operation selected = Operation.PLUS;
            long current = 3L;
            bool swap = true;
            while ((total & 1) != 1) js.Append(selected.ToChar());
            while (total > 1L)
            {
                while (total > 1L && total % current == 0L)
                {
                    total /= current;
                    if (swap) selected = (Operation)((int)(selected + 1) % 6);
                    else js.Append(selected.ToChar());
                }
                swap = !swap;
                while (total > 1L)
                {
                    bool valid = true;
                    double limit = Math.Sqrt(current += 2);
                    for (int factor = 3; factor <= limit; factor += 2)
                    {
                        if (current % factor == 0)
                        {
                            valid = false;
                            break;
                        }
                    }
                    if (valid) break;
                }
            }
            Console.WriteLine("Executing program");
            Console.WriteLine("Result: {0}", engine.Execute(js.ToString()).GetCompletionValue());
        }
    }
    
    internal enum Operation : byte
    {
        PLUS,
        NOT,
        OPEN_BRACKET,
        CLOSE_BRACKET,
        OPEN_PARENTHESIS,
        CLOSE_PARENTHESIS
}

    internal static class OperationExtension
    {
        public static char ToChar(this Operation op)
        {
            switch (op)
            {
                case Operation.PLUS:
                    return '+';
                case Operation.NOT:
                    return '!';
                case Operation.OPEN_BRACKET:
                    return '[';
                case Operation.CLOSE_BRACKET:
                    return ']';
                case Operation.OPEN_PARENTHESIS:
                    return '(';
                case Operation.CLOSE_PARENTHESIS:
                    return ')';
                default:
                    throw new ArgumentException("unrecognized operation");
            }
        }
    }
}
