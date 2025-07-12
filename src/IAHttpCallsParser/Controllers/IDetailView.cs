namespace IAHttpCallsParser.Controllers;

/// <summary>
///		Interface para un control de detalles
/// </summary>
internal interface IDetailView
{
	/// <summary>
	///		Graba los datos
	/// </summary>
	bool Save();

	/// <summary>
	///		Comprueba si se ha modificado el valor
	/// </summary>
	bool IsUpdated();
}