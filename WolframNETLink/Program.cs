using System;
using Wolfram.NETLink;
using WolframNETLink;

namespace WolframAlphaLocal
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Please specify which implementation to run: 1, 2, or 3.");
            string input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    MainImplementation1.Run();
                    break;
                case "2":
                    MainImplementation2.Run();
                    break;
                case "3":
                    MainImplementation3.Run();
                    break;
                default:
                    Console.WriteLine("Invalid input. Please specify 1, 2, or 3.");
                    break;
            }
        }
    }
}
