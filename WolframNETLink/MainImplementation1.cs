using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wolfram.NETLink;

namespace WolframNETLink
{
    public static class MainImplementation1
    {
        public static void Run()
        {
            // Initialize the Wolfram Engine
            IKernelLink ml = MathLinkFactory.CreateKernelLink();

            try
            {
                // Discard the initial InputNamePacket the kernel will send
                ml.WaitAndDiscardAnswer();

                ml.Evaluate("FactorInteger[123456789]");
                ml.WaitForAnswer();

                // Read the result as a 2D array of integers
                object result = ml.GetArray(typeof(int), 2);
                if (result is int[,] factorResult)
                {
                    Console.WriteLine("Implementation 1 - Factors of 123456789:");
                    for (int i = 0; i < factorResult.GetLength(0); i++)
                    {
                        Console.WriteLine($"Prime: {factorResult[i, 0]}, Exponent: {factorResult[i, 1]}");
                    }
                }
                else
                {
                    Console.WriteLine("Unexpected result type.");
                }
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
