using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wolfram.NETLink;

namespace WolframNETLink
{
    public static class MainImplementation3
    {
        public static async Task Run()
        {
            var wolframLogic = new WolframLogic();
            wolframLogic.OnNewEvaluation += (result) =>
            {
                Console.WriteLine($"New Evaluation Result: {result}");
            };

            wolframLogic.OnNewEvaluation += (result) =>
            {
                // send logic
                Console.WriteLine("   - sending now ... sent.");
            };

            await wolframLogic.StartAsync();
            Console.WriteLine("Press Enter to exit.");
            Console.ReadLine();
            wolframLogic.Stop();
        }
    }
}
