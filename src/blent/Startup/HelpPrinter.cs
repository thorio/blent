using Blent.Utility;
using Blent.Verb;
using CommandLine;
using CommandLine.Text;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Blent.Startup
{
	public static class HelpPrinter
	{
		public static void PrintHelp(IVerb verb, NotParsed<object> result)
		{
			if (verb?.Usage != null)
			{
				Output.Error.Write($"\nUSAGE: ", Color.Info);
				Output.Error.WriteLine($"{verb.GetVerbName()} {verb.Usage}");
			}

			var errors = RenderErrors(result);
			if (!string.IsNullOrWhiteSpace(errors))
			{
				Output.Error.WriteLine("\nERRORS:", Color.Danger);
				Output.Error.WriteLine(errors);
			}

			if (verb == null)
			{
				Output.Error.WriteLine("\nVERBS:", Color.Info);
				Output.Error.WriteLine(GenerateVerbHelp(result));
			}
			else
			{
				Output.Error.WriteLine("\nOPTIONS:", Color.Info);
				Output.Error.WriteLine(GenerateOptionsHelp(result));
			}
		}

		private static string GenerateVerbHelp(NotParsed<object> result)
		{
			return RenderHelpText(b => b.AddVerbs(result.TypeInfo.Choices.ToArray()));
		}

		private static string GenerateOptionsHelp(NotParsed<object> result)
		{
			var text = RenderHelpText(b =>
			{
				b.AddDashesToOption = true;
				b.AddOptions(result);
			});

			return Regex.Replace(text, @"\(pos\. \d*\)", t => new string(' ', t.Value.Length));
		}

		private static string RenderHelpText(Action<HelpText> configure)
		{
			var builder = new HelpText("", "")
			{
				AutoVersion = false,
			};
			configure(builder);

			return builder.ToString()
				.Trim(new[] { '\r', '\n' });
		}

		private static string RenderErrors(NotParsed<object> result)
		{
			var sentenceBuilder = SentenceBuilder.Create();
			return HelpText.RenderParsingErrorsText(result, sentenceBuilder.FormatError, sentenceBuilder.FormatMutuallyExclusiveSetErrors, 2);
		}
	}
}