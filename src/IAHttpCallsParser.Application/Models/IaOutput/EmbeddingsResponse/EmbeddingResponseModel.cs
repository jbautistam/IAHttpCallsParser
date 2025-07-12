namespace IAHttpCallsParser.Application.Models.IaOutput.EmbeddingsResponse;

/// <summary>
///		Modelo para los datos de respuesta del embeding
/// </summary>
public class EmbeddingResponseModel : IaOutputBaseModel
{
    /// <summary>
    ///     Tipo de objeto
    /// </summary>
    public string Object { get; set; } = default!;

    /// <summary>
    ///     Modelo
    /// </summary>
    public string Model { get; set; } = default!;

    /// <summary>
    ///     Datos de uso
    /// </summary>
    public EmbeddingResponseUsageModel? Usage { get; set; }
}
