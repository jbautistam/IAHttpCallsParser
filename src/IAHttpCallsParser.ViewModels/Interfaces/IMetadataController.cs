using Bau.Libraries.BauMvvm.ViewModels.Controllers;

namespace IAHttpCallsParser.ViewModels.Interfaces;

/// <summary>
///		Interface con los controladores de las vistas
/// </summary>
public interface IIAHttpCallsParserController
{
	/// <summary>
	///		Controlador principal
	/// </summary>
	IHostController HostController { get; }
}
