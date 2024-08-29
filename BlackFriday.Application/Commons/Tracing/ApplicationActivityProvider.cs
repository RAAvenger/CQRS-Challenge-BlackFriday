using System.Diagnostics;

namespace BlackFriday.Application.Commons.Tracing;

public class ApplicationActivityProvider
{
	public const string ActivitySourceName = "BlackFriday.Application";

	internal static ActivitySource ActivitySource { get; } = new ActivitySource(ActivitySourceName, "1.0.0");
}
