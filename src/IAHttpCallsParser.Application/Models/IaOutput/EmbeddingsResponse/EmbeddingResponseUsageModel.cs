namespace IAHttpCallsParser.Application.Models.IaOutput.EmbeddingsResponse;

/// <summary>
///		Modelo para los datos de respuesta del embeding
/// </summary>
public class EmbeddingResponseUsageModel : IaOutputBaseModel
{
    /// <summary>
    ///     Número de tokens del prompt
    /// </summary>
    public int Prompt_Tokens { get; set; }

    /// <summary>
    ///     Número de tokens total
    /// </summary>
    public int Total_Tokens { get; set; }
}
