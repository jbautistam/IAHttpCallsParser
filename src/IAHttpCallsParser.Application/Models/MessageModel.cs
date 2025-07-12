namespace IAHttpCallsParser.Application.Models;

/// <summary>
///		Datos de un mensaje
/// </summary>
public class MessageModel
{
	/// <summary>
	///		Método Http
	/// </summary>
	public enum MethodType
	{
		/// <summary>Método GET</summary>
		Get,
		/// <summary>Método POST</summary>
		Post,
		/// <summary>Método PUT</summary>
		Put,
		/// <summary>Método PATCH</summary>
		Patch,
		/// <summary>Método OPTIONS</summary>
		Options,
		/// <summary>Método DELETE</summary>
		Delete
	}

	public MessageModel(ChatFileModel file, string request, string response)
	{
		// Inicializa los objetos
		File = file;
		Request = new MessageSectionModel(this, request, true);
		Response = new MessageSectionModel(this, response, false);
		// Interpreta los datos
		Parse();
	}

	/// <summary>
	///		Interpreta la solicitud / respuesta
	/// </summary>
	private void Parse()
	{
		Parse(true, Request);
		Parse(false, Response);
	}

	/// <summary>
	///		Interpreta los datos de una sección
	/// </summary>
	private void Parse(bool isRequest, MessageSectionModel section)
	{
		if (!string.IsNullOrWhiteSpace(section.Content))
		{
			string[] lines = section.Content.Replace("\r\n", "\r").Split('\r');
			int startIndex = 0;
			bool isAtBody = false;

				// Interpreta la línea del método
				if (isRequest)
					if (ParseMethod(lines[0]))
						startIndex = 1;
				// Interpreta las líneas
				for (int index = startIndex; index < lines.Length; index++)
					if (!isAtBody && string.IsNullOrWhiteSpace(lines[index]))
						isAtBody = true;
					else if (isAtBody)
						section.Body += lines[index] + Environment.NewLine;
					else if (!string.IsNullOrWhiteSpace(lines[index]))
					{
						(string key, string? value) = ParseHeader(lines[index]);
							
							if (!string.IsNullOrWhiteSpace(key))
								section.Headers.Add((key, value));
					}
		}
	}

	/// <summary>
	///		Interpreta la línea del método (POST https://aif-sergio.services.ai.azure.com/openai/deployments/gpt-4.1-mini/chat/completions?api-version=2025-03-01-preview)
	/// </summary>
	private bool ParseMethod(string line)
	{
		bool parsed = false;

			// Interpreta la línea
			if (!string.IsNullOrWhiteSpace(line))
			{
				int indexSpace = line.IndexOf(' ');

					// Si realmente ha encontrado el separador
					if (indexSpace > 0)
					{
						// Obtiene las partes
						Method = GetEnum(line.Substring(0, indexSpace), MethodType.Get);
						Url = line.Substring(indexSpace);
						if (!string.IsNullOrWhiteSpace(Url))
							Url = Url.Trim();
						// Indica que se ha interpretado correctamente
						parsed = true;
					}
			}
			// Devuelve el valor que indica si se ha interpretado
			return parsed;
	}

	/// <summary>
	///		Interpreta la cabecera
	/// </summary>
	private (string key, string? value) ParseHeader(string line)
	{
		string [] parts = line.Split(':', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
		string key = "key";
		string? value = null;

			// Añade los datos de cabecera / valor
			if (parts.Length > 0)
			{
				// Cabecera
				key = parts[0];
				// Valor
				for (int index = 1; index < parts.Length; index++)
					value += parts[index] + " ";
			}
			// Devuelve la cabecera
			return (key, value);
	}

	/// <summary>
	///		Obtiene el valor de un enumerado
	/// </summary>
	private TypeEnum GetEnum<TypeEnum>(string value, TypeEnum defaultValue) where TypeEnum : struct
	{
		if (Enum.TryParse(value, true, out TypeEnum result))
			return result;
		else
			return defaultValue;
	}

	/// <summary>
	///		Obtiene la página a partir de la URL
	/// </summary>
	public string GetPage()
	{
		string page = Url;

			// Obtiene la última parte de la URL
			if (!string.IsNullOrWhiteSpace(Url))
			{
				int indexChar = Url.IndexOf('?');

					// Obtiene la página sin el queryString
					if (indexChar >= 0)
						page = page[.. indexChar];
					// Obtiene los dos últimos directorios
					if (!string.IsNullOrWhiteSpace(page))
						page = GetSections(page);
			}
			// Devuelve la página
			return page;

		// Obtiene los dos últimos directorios o el último directorio de la URL
		string GetSections(string url)
		{
			string[] parts = url.Split('/', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

				// Obtiene los dos últimos directorios o el último directorio
				if (parts.Length >= 2)
					return parts[parts.Length - 2] + "/" + parts[parts.Length - 1];
				else if (parts.Length >= 1)
					return parts[parts.Length - 1];
				else
					return url;
		}
	}

	/// <summary>
	///		Archivo al que se asocia el mensaje
	/// </summary>
	public ChatFileModel File { get; }

	/// <summary>
	///		Método
	/// </summary>
	public MethodType Method { get; private set; } = MethodType.Get;

	/// <summary>
	///		Url de llamada
	/// </summary>
	public string Url { get; private set; } = default!;

	/// <summary>
	///		Datos de la solicitud
	/// </summary>
	public MessageSectionModel Request { get; }

	/// <summary>
	///		Datos de la respuesta
	/// </summary>
	public MessageSectionModel Response { get; }
}