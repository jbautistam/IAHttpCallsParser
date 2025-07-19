using IAHttpCallsParser.Application.Models;

namespace IAHttpCallsParser.Application.Parsers;

/// <summary>
///		Intérprete de los mensajes HTTP de la IA
/// </summary>
internal class IaHttpParser
{
	/// <summary>
	///		Interpreta un archivo
	/// </summary>
	internal ChatFileModel Parse(string fileName)
	{
		ChatFileModel file = new(fileName);

			// Interpreta el archivo
			if (string.IsNullOrWhiteSpace(fileName) || !File.Exists(fileName))
				file.Error = $"Can't find the file '{fileName}'";
			else
				try
				{
					Parse(file, File.ReadAllLines(fileName));
				}
				catch (Exception exception)
				{
					file.Error = $"Error when parse {exception.Message}";
				}
			// Devuelve el archivo interpretado
			return file;
	}

	/// <summary>
	///		Interpreta un archivo
	/// </summary>
	private void Parse(ChatFileModel file, string[] lines)
	{
		List<string> sections = SplitSections(lines);

			// Añade los mensajes
			for (int index = 0; index < sections.Count; index += 2)
			{
				string request = ConvertRequest(sections[index]);
				string response = string.Empty;

					// Añade el contenido a la respuesta
					if (index + 1 < sections.Count)
						response = ConvertResponse(sections[index + 1]);
					// Añade el mensaje al archivo
					file.Messages.Add(new MessageModel(file, request, response));
			}
	}

	/// <summary>
	///		Divide las secciones
	/// </summary>
	private List<string> SplitSections(string [] lines)
	{
		List<string> sections = [];
		string content = string.Empty;

			// Recorre las líneas
			for (int index = 0; index < lines.Length; index++)
			{
				if (!string.IsNullOrWhiteSpace(lines[index]))
				{
					if (lines[index].Trim().StartsWith("###", StringComparison.CurrentCultureIgnoreCase))
					{
						// Añade el contenido a una nueva sección
						sections.Add(content);
						// Limpia el contenido
						content = string.Empty;
					}
					else
						content += lines[index] + Environment.NewLine;
				}
				else // ... añade un salto de línea
					content += Environment.NewLine;
			}
			// Añade el contenido final
			if (!string.IsNullOrWhiteSpace(content))
				sections.Add(content);
			// Devuelve la lista de secciones
			return sections;
	}

	/// <summary>
	///		Convierte la solicitud
	/// </summary>
	private string ConvertRequest(string request)
	{
		string result = string.Empty;

			// Quita las líneas de inicio (##)
			if (!string.IsNullOrWhiteSpace(request))
			{
				List<string> lines = request.Trim().Replace("\r\n", "\r").Split('\r').ToList();

					// Quita la cabecera si es necesario
					if (lines.Count > 0 && !string.IsNullOrWhiteSpace(lines[0]) && lines[0].Trim().Equals("###"))
					{
						// Quita el ##
						lines.RemoveAt(0);
						// Quita el salto de línea siguiente
						if (lines.Count > 0 && string.IsNullOrWhiteSpace(lines[0]))
							lines.RemoveAt(0);
					}
					// Añade todas las líneas a la solicitud
					foreach (string line in lines)
						result += line + Environment.NewLine;
			}
			// Devuelve el resultado
			return result;
	}

	/// <summary>
	///		Convierte la respuesta
	/// </summary>
	private string ConvertResponse(string response)
	{
		string result = string.Empty;
		bool foundBody = false;

			// Convierte la respuesta
			if (!string.IsNullOrWhiteSpace(response))
				foreach (string part in response.Replace("\r\n", "\r").Split('\r', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
					if (!string.IsNullOrWhiteSpace(part))
					{
						string line = part.Trim();

							// Quita los comentarios
							if (line.StartsWith("#"))
								line = line[1 ..];
							// Quita los espacios
							line = line.Trim();
							// Añade un salto de línea al cuerpo
							if ((line.StartsWith("{") || line.StartsWith("[")) && !foundBody)
							{
								line = Environment.NewLine + line;
								foundBody = true;
							}
							// Añade el resultado
							result += line + Environment.NewLine;
					}
			// Quita los espacios
			if (!string.IsNullOrWhiteSpace(result))
				result = result.Trim();
			// Devuelve el resultado
			return result;
	}
}