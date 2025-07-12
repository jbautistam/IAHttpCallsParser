namespace IAHttpCallsParser.Application.Models;

/// <summary>
///		Modelo con los datos del archivo
/// </summary>
public class ChatFileModel(string fileName)
{
	/// <summary>
	///		Nombre de archivo
	/// </summary>
	public string FileName { get; } = fileName;

	/// <summary>
	///		Mensajes del archivo
	/// </summary>
	public List<MessageModel> Messages { get; } = [];

	/// <summary>
	///		Mensaje de error
	/// </summary>
	public string? Error { get; set; }
}