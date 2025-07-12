namespace IAHttpCallsParser.Application.Models.IaOutput.ChatCompletions;

/// <summary>
///		Datos de una función
/// </summary>
public class ChatCompletionToolFunctionModel
{
    /// <summary>
    ///     Nombre de la función
    /// </summary>
    public string Name { get; set; } = default!;

    /// <summary>
    ///     Descripción de la función
    /// </summary>
    public string Description { get; set; } = default!;

    /// <summary>
    ///     Indica si es estricta o no
    /// </summary>
    public bool Strict { get; set; }
}
