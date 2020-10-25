using Blent.Utility;
using Blent.Verb;
using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Blent.Startup
{
	public class CommandLineParser
	{
		private const string ArgumentTerminator = "--";

		private readonly List<IVerb> _verbs;
		private readonly string[] _args;
		private readonly string[] _argsRest;

		public CommandLineParser(string[] args)
		{
			_args = args.TakeWhile(a => a != ArgumentTerminator).ToArray();
			_argsRest = args.SkipWhile(a => a != ArgumentTerminator).Skip(1).ToArray();
			_verbs = GetVerbs();
		}

		public IEnumerable<Error> ParseAndExecuteVerb()
		{
			PerformanceTesting.Checkpoint("Begin Parse");
			switch (GetParser().ParseArguments(_args, GetVerbTypes()))
			{
				case Parsed<object> result:
					ExecuteVerb((IOptions)result.Value);
					break;
				case NotParsed<object> result:
					PrintHelp(result);
					return result.Errors;
			}
			return new Error[0];
		}

		private Parser GetParser()
		{
			return new Parser(p =>
			{
				p.HelpWriter = null;
				p.AutoVersion = false;
				p.CaseInsensitiveEnumValues = true;
			});
		}

		private void ExecuteVerb(IOptions options)
		{
			PerformanceTesting.Checkpoint("End Parse");
			options.PassthroughArguments = string.Join(" ", _argsRest);

			var verb = GetVerb(options.GetType());

			VerbExecuter.ExecuteVerb(verb, options);
		}

		private void PrintHelp(NotParsed<object> result)
		{
			PerformanceTesting.Checkpoint("Begin Help");
			var verb = GetVerb(result.TypeInfo.Current);
			HelpPrinter.PrintHelp(verb, result);
		}

		private IVerb GetVerb(Type type) =>
			_verbs.SingleOrDefault(v => v.GetOptionsType().Equals(type));

		private List<IVerb> GetVerbs()
		{
			return Assembly.GetExecutingAssembly().GetTypes()
				.Where(t => typeof(IVerb).IsAssignableFrom(t) && t.IsClass && !t.ContainsGenericParameters)
				.Select(t => Activator.CreateInstance(t))
				.Cast<IVerb>()
				.ToList();
		}

		private Type[] GetVerbTypes()
		{
			return _verbs
				.Select(v => v.GetOptionsType())
				.ToArray();
		}
	}
}
