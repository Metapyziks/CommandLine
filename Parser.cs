using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandLine
{
    public sealed class Parser
    {
        private String[] _args;
        private bool[] _used;

        public Parser(String[] args)
        {
            _args = args;
            _used = new bool[args.Length];
        }

        private static bool InputMatchesOption(String input, String option)
        {
            if (option.Length == 1 && !input.StartsWith("--")) {
                return option.Skip(1).Any(x => x == option[0]);
            }

            return input.Length > 1 && input.Substring(2) == option;
        }

        private static bool InputMatchesOptions(String input, String[] options)
        {
            if (!input.StartsWith("-")) return false;
            return options.Any(x => InputMatchesOption(input, x));
        }

        public T GetValue<T>(params String[] options)
        {
            var values = GetValues<T>(options);
            if (values.Count() == 0) return default(T);
            return values.Last();
        }

        public IEnumerable<T> GetValues<T>(params String[] options)
        {
            var values = new List<String>();
            for (int i = 0; i < _args.Length - 1; ++i) {
                var arg = _args[i];
                if (InputMatchesOptions(arg, options)) {
                    _used[i] = _used[i + 1] = true;
                    values.Add(_args[i + 1]);
                }
            }

            return values.Select(x => (T) Convert.ChangeType(x, typeof(T)));
        }

        public IEnumerable<String> GetRemaining()
        {
            return _args.Where((x, i) => !_used[i]);
        }
    }
}
