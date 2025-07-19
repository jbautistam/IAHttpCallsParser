namespace IAHttpCallsParser.Application.Models.IaOutput.Assistants;

/// <summary>
///		Modelo de las llamadas a project--/assistants
/// </summary>
public class AssistantModel : IaOutputBaseModel
{
	/// <summary>
	///		Id de la respuesta
	/// </summary>
	public string? Id { get; set; }

	/// <summary>
	///		Objeto de la respuesta
	/// </summary>
	public string? Object { get; set; }

	/// <summary>
	///		Fecha de creación de la respuesta
	/// </summary>
	public long Created_At { get; set; }

	/// <summary>
	///		Nombre del modelo
	/// </summary>
	public string Model { get; set; } = default!;

	/// <summary>
	///		Nombre del asistente
	/// </summary>
	public string Name { get; set; } = default!;

	/// <summary>
	///		Descripción del asistente
	/// </summary>
	public string Description { get; set; } = default!;

	/// <summary>
	///		Instrucciones del asistente
	/// </summary>
	public string Instructions { get; set; } = default!;

	/// <summary>
	///		Datos de herramientas
	/// </summary>
	public List<AssistantToolModel> Tools { get; set; } = [];
}