using IAHttpCallsParser.ViewModels.Interfaces;
using Bau.Libraries.BauMvvm.ViewModels.Controllers;
using Bau.Libraries.BauMvvm.Views.Wpf.Controllers;

namespace IAHttpCallsParser.Controllers;

/// <summary>
///		Controlador principal
/// </summary>
public class IAHttpCallsParserController : IIAHttpCallsParserController
{
	public IAHttpCallsParserController(string applicationName, System.Windows.Window mainWindow)
	{
		HostController = new HostController(applicationName, mainWindow);
		HostHelperController = new HostHelperController(mainWindow);
		MainWindow = mainWindow;
	}

	/// <summary>
	///		Controlador principal
	/// </summary>
	public IHostController HostController { get; }

	/// <summary>
	///		Controlador de Windows
	/// </summary>
	public HostHelperController HostHelperController { get; }

	/// <summary>
	///		Ventana principal
	/// </summary>
	public System.Windows.Window MainWindow { get; }
}
