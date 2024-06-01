using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wolfram.NETLink;

namespace WolframNETLink
{
    public static class MainImplementation2
    {
        public static void Run()
        {
            // Initialize the Wolfram Engine
            IKernelLink ml = MathLinkFactory.CreateKernelLink();

            try
            {
                // Discard the initial InputNamePacket the kernel will send
                ml.WaitAndDiscardAnswer();

                // Evaluate the expression FactorInteger[123456789]
                ml.Evaluate("FactorInteger[123456789]");
                ml.WaitForAnswer();

                // Get the result as an expression
                Expr result = ml.GetExpr();

                // Process the result
                if (result.ListQ())
                {
                    Console.WriteLine("Implementation 2 - Factors of 123456789:");
                    foreach (Expr factor in result.Args)
                    {
                        if (factor.ListQ() && factor.Length == 2)
                        {
                            int prime = (int)factor.Part(1).AsInt64();
                            int exponent = (int)factor.Part(2).AsInt64();
                            Console.WriteLine($"Prime: {prime}, Exponent: {exponent}");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Unexpected result type.");
                }

                // Example: Other things you can do with the Expr object
                // Print the head of the expression
                Console.WriteLine("Head of the result: " + result.Head);

                // Check the type of the expression
                Console.WriteLine("Is the result a list? " + result.ListQ());

                // Get the length of the expression (number of parts)
                Console.WriteLine("Number of parts: " + result.Length);

                // Convert the entire expression to a string
                Console.WriteLine("Expression as a string: " + result.StringQ());
            }
            catch (MathLinkException e)
            {
                Console.WriteLine($"MathLinkException: {e.Message}");
            }
            finally
            {
                // Always close the link when done
                ml.Close();
            }
        }
    }
}
