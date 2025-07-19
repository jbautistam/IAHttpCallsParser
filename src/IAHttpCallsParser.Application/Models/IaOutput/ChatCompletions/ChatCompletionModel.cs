namespace IAHttpCallsParser.Application.Models.IaOutput.ChatCompletions;

/// <summary>
///		Modelo de las llamadas a chat/completions
/// </summary>
public class ChatCompletionModel : IaOutputBaseModel
{
	/// <summary>
	///		Datos de herramientas
	/// </summary>
	public List<ChatCompletionToolModel> Tools { get; set; } = [];

	/// <summary>
	///		Mensajes
	/// </summary>
	public List<ChatCompletionMessageModel> Messages { get; set; } = [];

	/// <summary>
	///		Nombre del modelo
	/// </summary>
	public string Model { get; set; } = default!;

	/// <summary>
	///		Modo de selección de la herramienta
	/// </summary>
	public string Tool_Choice { get; set; } = default!;
}