namespace IAHttpCallsParser.Application.Models.IaOutput.ChatCompletions;

/// <summary>
///		Datos de mensaje
/// </summary>
public class ChatCompletionMessageModel
{
	/// <summary>
	///		Rol del mensaje
	/// </summary>
	public string Role { get; set; } = default!;

	/// <summary>
	///		Contenido del mensaje
	/// </summary>
	public string Content { get; set; } = default!;

	/// <summary>
	///		Nombre del agente
	/// </summary>
	public string? Name { get; set; }

    /// <summary>
    ///     Llamadas a herramientas
    /// </summary>
    public List<ChatCompletionsResponse.ChatCompletionResponseChoiceMessageToolCallModel> Tool_Calls { get; set; } = [];
}
