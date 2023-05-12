using System.Text;
using Delogger;
using Delogger.Extensions;
using Delogger.Scope.Log;

namespace Tests.Utils.Mock.Delogger;

public class DLogger : IDLogger
{
	public void Log(string message, string[]? tags = null, object[]? args = null, KeyValuePair<string, object>[]? attachments = null, WriteFlagsEnum flags = WriteFlagsEnum.All)
	{
		if (tags == null)
		{
			tags = Array.Empty<string>();
		}

		var sb = new StringBuilder($"[{DateTime.Now.ToString("dd.MM.yyyy")} {DateTime.Now.ToString("HH:mm:ss.fff")}]");

		if (tags.Length > 0)
		{
			sb.Append($"[{string.Join(',', tags)}]");
		}

		sb.Append($": {message.SafeFormat(args ?? Array.Empty<object>())}");

		sb.Append(FormatSubInfoAndAttachments(attachments));


		Console.WriteLine(sb);
	}

	private string FormatSubInfoAndAttachments(KeyValuePair<string, object>[] attachments)
	{
		var subSb = new StringBuilder();
		subSb.Append('\n');

		if (attachments.Length > 0)
		{
			subSb.AppendJoin('\n', attachments.Select(x => $"{x.Key}: {x.Value}"));
			subSb.AppendLine();
		}

		var subString = subSb.ToString();

		if (!string.IsNullOrWhiteSpace(subString))
		{
			subString = subString.Replace("\n", "\n│");
			subString = ReplaceLast(subString, "\n│", "\n└");
			return subString;
		}
		return "";
	}

	private static string ReplaceLast(string text, string search, string replace)
	{
		var pos = text.LastIndexOf(search);
		if (pos < 0)
			return text;
		return string.Concat(text.AsSpan(0, pos), replace, text.AsSpan(pos + search.Length));
	}

	public void Dispose() { }
}