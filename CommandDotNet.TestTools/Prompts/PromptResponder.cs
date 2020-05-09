using System;
using System.Collections.Generic;
using System.Linq;
using CommandDotNet.Extensions;

namespace CommandDotNet.TestTools.Prompts
{
    public class PromptResponder : IPromptResponder
    {
        private readonly List<IAnswer> _filteredAnswers = new List<IAnswer>();
        private readonly List<IAnswer> _unfilteredAnswers = new List<IAnswer>();
        private Queue<ConsoleKeyInfo>? _currentAnswer;

        public PromptResponder(IEnumerable<IAnswer> answers)
        {
            answers.ForEach(a => GetListFor(a).Add(a));
        }

        public ConsoleKeyInfo OnReadKey(TestConsole testConsole)
        {
            if (_currentAnswer is null)
            {
                var answer = GetNextAnswer(testConsole);
                _currentAnswer = new Queue<ConsoleKeyInfo>(answer.ConsoleKeys);
            }
            else if (_currentAnswer.Count == 0)
            {
                _currentAnswer = null;
                return ConsoleKeyInfos.EnterKey;
            }

            return _currentAnswer.Dequeue();
        }

        private List<IAnswer> GetListFor(IAnswer a) => a.PromptFilter is null ? _unfilteredAnswers : _filteredAnswers;

        private IAnswer GetNextAnswer(TestConsole testConsole)
        {
            var promptLine = testConsole.OutLastLine;
            var answer = _filteredAnswers.FirstOrDefault(a => a.PromptFilter?.Invoke(promptLine) ?? false)
                         ?? _unfilteredAnswers.FirstOrDefault(a => a.PromptFilter is null);

            if (answer == null)
            {
                throw new UnexpectedPromptFailureException($"no answer available for the prompt: {promptLine}");
            }

            if (answer.ShouldFail)
            {
                throw new UnexpectedPromptFailureException($"forbidden prompt: {promptLine}");
            }

            if (!answer.Reuse)
            {
                GetListFor(answer).Remove(answer);
            }

            return answer;
        }
    }
}