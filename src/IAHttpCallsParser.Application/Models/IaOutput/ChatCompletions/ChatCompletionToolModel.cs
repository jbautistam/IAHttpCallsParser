namespace IAHttpCallsParser.Application.Models.IaOutput.ChatCompletions;

/// <summary>
///		Datos de herramientas
/// </summary>
public class ChatCompletionToolModel
{
	/// <summary>
	///		Datos de la función
	/// </summary>
	public ChatCompletionToolFunctionModel? Function { get; set; }

	/// <summary>
	///		Tipo de la herramienta
	/// </summary>
	public string Type { get; set; } = default!;
}
