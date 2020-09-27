using Blent.Verb.Base;
using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Blent.CommandLineParser
{
	public class Parser
	{
		private const string ArgumentTerminator = "--";

		private readonly List<IVerb> _verbs;
		private readonly string[] _args;
		private readonly string[] _argsRest;

		public Parser(string[] args)
		{
			_args = args.TakeWhile(a => a != ArgumentTerminator).ToArray();
			_argsRest = args.SkipWhile(a => a != ArgumentTerminator).Skip(1).ToArray();
			_verbs = GetVerbs();
		}

		public IEnumerable<Error> ParseAndExecuteVerb()
		{
			switch (GetParser().ParseArguments(_args, GetVerbTypes()))
			{
				case Parsed<object> result:
					RunVerb((IOptions)result.Value);
					break;
				case NotParsed<object> result:
					PrintHelp(result);
					return result.Errors;
			}
			return new Error[0];
		}

		private CommandLine.Parser GetParser()
		{
			return new CommandLine.Parser(p =>
			{
				p.HelpWriter = null;
			});
		}

		private void RunVerb(IOptions options)
		{
			ProcessArguments(options);
			options.Rest = _argsRest;

			var type = options.GetType();
			_verbs.Single(v => v.GetOptionsType().Equals(type))
				.Execute(options);
		}

		private void ProcessArguments(IOptions options)
		{
			if (options.AppDirectory != null)
			{
				Configuration.Settings.AppDirectory = options.AppDirectory;
			}
		}

		private void PrintHelp(NotParsed<object> result)
		{
			var helpText = CommandLine.Text.HelpText.AutoBuild(result, s =>
			{
				s.Heading = "";
				s.Copyright = "";
				s.AdditionalNewLineAfterOption = false;
				return s;
			}).ToString().Trim(new[] { '\r', '\n' });

			helpText = Environment.NewLine + helpText + Environment.NewLine;
			helpText = helpText.Replace("ERROR(S):" + Environment.NewLine, "");

			Console.Error.Write(helpText);
		}

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
