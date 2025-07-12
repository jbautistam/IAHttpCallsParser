
namespace IAHttpCallsParser.Application.Models.IaOutput.Embeddings;

/// <summary>
///		Modelo para los datos de embeding
/// </summary>
public class EmbeddingModel : IaOutputBaseModel
{
    /// <summary>
    ///     Obtiene una cadena con todas las entradas
    /// </summary>
	public string GetInput()
	{
        string result = string.Empty;

            // Añade las entradas
            foreach (string input in Input)
                result += input + Environment.NewLine + Environment.NewLine;
            // Quita los espacios
            if (!string.IsNullOrWhiteSpace(result))
                result = result.Trim();
            // Devuelve el resultado
            return result;
	}

    /// <summary>
    ///     Entradas
    /// </summary>
    public List<string> Input { get; set; } = [];

    /// <summary>
    ///     Modelo
    /// </summary>
    public string Model { get; set; } = default!;

    /// <summary>
    ///     Formato de codificación
    /// </summary>
    public string? Encoding_Format { get; set; } = default!;
}
