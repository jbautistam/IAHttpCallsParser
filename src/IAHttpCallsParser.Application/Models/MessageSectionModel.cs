using System.Text.Json;

namespace IAHttpCallsParser.Application.Models;

/// <summary>
///		Datos de una sección del mensaje (request o response)
/// </summary>
public class MessageSectionModel(MessageModel message, string content, bool isRequest)
{
	// Variables privadas
	private IaOutput.IaOutputBaseModel? _parsed = null;

	/// <summary>
	///		Interpreta el cuerpo de la sección
	/// </summary>
	private IaOutput.IaOutputBaseModel? Parse(string body)
	{
		// Interpreta el cuerpo
		if (!string.IsNullOrWhiteSpace(body))
			try
			{
				string page = Message.GetPage();

					// Interpreta el contenido
					if (page.Equals("chat/completions", StringComparison.CurrentCultureIgnoreCase))
					{
						if (IsRequest)
							return ParseJson<IaOutput.ChatCompletions.ChatCompletionModel>(body);
						else
						{
							if (!body.Contains("\"skipped\"", StringComparison.CurrentCultureIgnoreCase))
								return ParseJson<IaOutput.ChatCompletionsResponse.ChatCompletionResponseModel>(body);
							else
								return ParseJson<IaOutput.ChatCompletionsResponse.ChatCompletionSkippedReason>(body);
						}
					}
					else if (page.Equals("text-embedding-ada-002/embeddings", StringComparison.CurrentCultureIgnoreCase) ||
							 page.EndsWith("embeddings", StringComparison.CurrentCultureIgnoreCase))
					{
						if (IsRequest)
							return ParseJson<IaOutput.Embeddings.EmbeddingModel>(body);
						else
							return ParseJson<IaOutput.EmbeddingsResponse.EmbeddingResponseModel>(body);
					}
			}
			catch (Exception exception)
			{
				System.Diagnostics.Debug.WriteLine($"Error when parse body {exception.Message}");
			}
		// Si ha llegado hasta aquí es porque no ha podido interpretar el cuerpo
		return null;

		// Serializa una cadena JSON
		TypeData? ParseJson<TypeData>(string? json)
		{
			if (!string.IsNullOrWhiteSpace(json))
				return JsonSerializer.Deserialize<TypeData>(json, 
															new JsonSerializerOptions
																	{
																		PropertyNameCaseInsensitive = true
																	}
															);
			else
				return default;
		}
	}

	/// <summary>
	///		Mensaje al que se asocia la solicitud
	/// </summary>
	public MessageModel Message { get; } = message;

	/// <summary>
	///		Indica si es una solicitud o una respuesta
	/// </summary>
	public bool IsRequest { get; } = isRequest;

	/// <summary>
	///		Contenido completo
	/// </summary>
	public string Content { get; } = content;

	/// <summary>
	///		Cabeceras
	/// </summary>
	public List<(string key, string? value)> Headers { get; } = [];

	/// <summary>
	///		Cuerpo del mensaje
	/// </summary>
	public string Body { get; internal set; } = default!;

	/// <summary>
	///		Cuerpo interpretado
	/// </summary>
	public IaOutput.IaOutputBaseModel? Parsed
	{
		get
		{
			// Interpreta el cuerpo
			if (_parsed is null)
				_parsed = Parse(Body);
			// Devuelve el objeto
			return _parsed;
		}
	}
}