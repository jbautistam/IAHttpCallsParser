namespace IAHttpCallsParser.Application.EventArguments;

/// <summary>
///		Argumentos del evento de modificación de archivo
/// </summary>
public class ChangedFileEventArgs(string fileName) : EventArgs
{
	/// <summary>
	///		Nombre de archivo modificado
	/// </summary>
	public string FileName { get; } = fileName;
}