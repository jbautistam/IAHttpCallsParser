namespace IAHttpCallsParser.Application.Models.IaOutput.ChatCompletionsResponse;

/// <summary>
///		Datos de selección de respuesta
/// </summary>
public class ChatCompletionResponseChoiceMessageToolCallModel
{
    /// <summary>
    ///     Id de la función
    /// </summary>
    public string Id { get; set; } = default!;

    /// <summary>
    ///     Tipo de función
    /// </summary>
    public string Type { get; set; } = default!;

    /// <summary>
    ///     Datos de la función utilizada
    /// </summary>
    public ChatCompletionResponseChoiceMessageToolCallFunctionModel? Function { get; set; }
}
