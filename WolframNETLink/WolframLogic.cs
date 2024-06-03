using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wolfram.NETLink;

namespace WolframNETLink
{
    public delegate void NewEvaluationEventHandler(string result);  // delegate type defines signature:
                                                                    //  event handlers match this signature

    public interface IWolframLogic
    {
        event NewEvaluationEventHandler OnNewEvaluation;            // event type enables the class or object
                                                                    //  to notify other classes or objects,
                                                                    //  based on delegates
                                                                    //  ... typically named OnXEvent,
                                                                    //      declaration with event keyword,
                                                                    //      followed by delegate type 
                                                                    //      - then name.
    }

    public class WolframLogic : IWolframLogic
    {
        private readonly List<string> _expressions = new List<string>
        {
            "FactorInteger[123456789]",
            "PrimeQ[9999991]",
            "Sin[Pi/4]^2 + Cos[Pi/4]^2"
        };
        private readonly Random _random = new Random();
        private readonly IKernelLink _ml;

        public event NewEvaluationEventHandler OnNewEvaluation;

        public WolframLogic()
        {
            _ml = MathLinkFactory.CreateKernelLink();
        }

        public async Task StartAsync()
        {
            while (true)
            {
                var expression = NextExpression();
                var result = await EvaluateExpressionAsync(expression);

                OnNewEvaluation?.Invoke(result);  // Fire the event to notify subscribers of the new evaluation result

                await Task.Delay(30000);  // Delay to simulate time-consuming evaluation, or can be adjusted as needed
            }
        }

        private string NextExpression()
        {
            int index = _random.Next(_expressions.Count);
            return _expressions[index];
        }

        private async Task<string> EvaluateExpressionAsync(string expression)
        {
            return await Task.Run(() =>
            {
                try
                {
                    _ml.WaitAndDiscardAnswer();
                    _ml.Evaluate(expression);
                    _ml.WaitForAnswer();
                    var result = _ml.GetExpr().ToString();
                    return result;
                }
                catch (MathLinkException e)
                {
                    return $"MathLinkException: {e.Message}";
                }
            });
        }

        public void Stop()
        {
            _ml.Close();
        }
    }
}
