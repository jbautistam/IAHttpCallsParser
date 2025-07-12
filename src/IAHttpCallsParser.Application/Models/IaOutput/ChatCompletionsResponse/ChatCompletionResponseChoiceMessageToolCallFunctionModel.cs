namespace IAHttpCallsParser.Application.Models.IaOutput.ChatCompletionsResponse;

/// <summary>
///		Datos de selección de respuesta
/// </summary>
public class ChatCompletionResponseChoiceMessageToolCallFunctionModel
{
    /// <summary>
    ///     Argumentos de llamada a la función
    /// </summary>
    public string Arguments { get; set; } = default!;

    /// <summary>
    ///     Nombre de función
    /// </summary>
    public string Name { get; set; } = default!;
}
