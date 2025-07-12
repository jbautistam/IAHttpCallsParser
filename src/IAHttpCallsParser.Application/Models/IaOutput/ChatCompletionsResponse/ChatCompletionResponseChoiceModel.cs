namespace IAHttpCallsParser.Application.Models.IaOutput.ChatCompletionsResponse;

/// <summary>
///		Datos de selección de respuesta
/// </summary>
public class ChatCompletionResponseChoiceModel
{
    /// <summary>
    ///     Razón de la finalización
    /// </summary>
    public string Finish_Reason { get; set; } = default!;

    /// <summary>
    ///     Mensaje
    /// </summary>
    public ChatCompletionResponseChoiceMessageModel? Message { get; set; }
}
