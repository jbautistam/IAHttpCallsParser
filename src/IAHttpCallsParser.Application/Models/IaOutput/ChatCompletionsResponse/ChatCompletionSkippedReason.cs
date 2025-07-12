namespace IAHttpCallsParser.Application.Models.IaOutput.ChatCompletionsResponse;

/// <summary>
///		Respuesta de Chat/Completion cuando está en streeming
/// </summary>
public class ChatCompletionSkippedReason : IaOutputBaseModel
{
	/// <summary>
	///		Texto de porqué se ha saltado este valor
	/// </summary>
	public string Skipped { get; set; } = default!;
}
