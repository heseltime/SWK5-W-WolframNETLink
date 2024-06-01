using System;
using Wolfram.NETLink;

namespace WolframAlphaLocal
{
    class Program
    {
        static void Main(string[] args)
        {
            // Initialize the Wolfram Engine
            IKernelLink ml = MathLinkFactory.CreateKernelLink();

            try
            {
                // Discard the initial InputNamePacket the kernel will send
                ml.WaitAndDiscardAnswer();

                // Send a simple command to Wolfram Mathematica
                ml.Evaluate("2+2");
                ml.WaitForAnswer();

                // Read the result as an integer
                int result = ml.GetInteger();
                Console.WriteLine("Result of 2+2: " + result);

                // Send a more complex command
                ml.Evaluate("FactorInteger[123456789]");
                ml.WaitForAnswer();

                // Read the result as a 2D array of integers
                int[,] factorResult = (int[,])ml.GetArray(typeof(int), 2);
                Console.WriteLine("Factors of 123456789:");
                for (int i = 0; i < factorResult.GetLength(0); i++)
                {
                    Console.WriteLine($"Prime: {factorResult[i, 0]}, Exponent: {factorResult[i, 1]}");
                }
            }
            catch (MathLinkException e)
            {
                Console.WriteLine($"MathLinkException: {e.Message}");
            }
            finally
            {
                // Close the link when done
                ml.Close();
            }
        }
    }
}
