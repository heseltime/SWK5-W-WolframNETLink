using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wolfram.NETLink;

namespace WolframNETLink
{
    public delegate void NewEvaluationEventHandler(string result);

    public interface IWolframLogic
    {
        event NewEvaluationEventHandler OnNewEvaluation;
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
        private readonly System.Timers.Timer _timer;
        private readonly IKernelLink _ml;

        public event NewEvaluationEventHandler OnNewEvaluation;

        public WolframLogic()
        {
            _ml = MathLinkFactory.CreateKernelLink();
            _timer = new System.Timers.Timer(30000); // 1/2 minute interval
            _timer.Elapsed += async (sender, e) => await HandleElapsedAsync();
            _timer.AutoReset = true;
            _timer.Enabled = true;
        }

        private async Task HandleElapsedAsync()
        {
            var expression = NextExpression();
            var result = await EvaluateExpressionAsync(expression);
            OnNewEvaluation?.Invoke(result);
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

        public async Task StartAsync()
        {
            await Task.Run(() => _timer.Start());
        }

        public void Stop()
        {
            _timer.Stop();
            _ml.Close();
        }
    }
}
