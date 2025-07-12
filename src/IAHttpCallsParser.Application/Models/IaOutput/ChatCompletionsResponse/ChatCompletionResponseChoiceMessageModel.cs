namespace IAHttpCallsParser.Application.Models.IaOutput.ChatCompletionsResponse;

/// <summary>
///		Datos de un mensaje de respuesta
/// </summary>
public class ChatCompletionResponseChoiceMessageModel
{
    /// <summary>
    ///     Contenido
    /// </summary>
    public string? Content { get; set; }

    /// <summary>
    ///     Role
    /// </summary>
    public string Role { get; set; } = default!;

    /// <summary>
    ///     Llamadas a herramientas
    /// </summary>
    public List<ChatCompletionResponseChoiceMessageToolCallModel> Tool_Calls { get; set; } = [];
}
