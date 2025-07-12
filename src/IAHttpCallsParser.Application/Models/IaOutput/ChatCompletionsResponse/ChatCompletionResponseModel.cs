namespace IAHttpCallsParser.Application.Models.IaOutput.ChatCompletionsResponse;

/// <summary>
///		Respuesta a la petición de chat
/// </summary>
public class ChatCompletionResponseModel : IaOutputBaseModel
{
    /// <summary>
    ///     Fecha de creación
    /// </summary>
    public long Created { get; set; }

    /// <summary>
    ///     Id
    /// </summary>
    public string Id { get; set; } = default!;

    /// <summary>
    ///     Datos del modelo
    /// </summary>
    public string Model { get; set; } = default!;

    /// <summary>
    ///     Nombre del objeto
    /// </summary>
    public string Object { get; set; } = default!;

    /// <summary>
    ///     Huella digital
    /// </summary>
    public string System_FingerPrint { get; set; } = default!;

    /// <summary>
    ///     Datos de selección de respuesta
    /// </summary>
    public List<ChatCompletionResponseChoiceModel> Choices { get; set; } = [];
}
