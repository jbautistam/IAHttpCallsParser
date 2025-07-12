namespace IAHttpCallsParser.Application;

/// <summary>
///		Manager de IAHttpCallsParser
/// </summary>
public class IAHttpCallsParserManager() : IDisposable
{
	// Eventos públicos
	public event EventHandler<EventArguments.ChangedFileEventArgs>? FileUpdated;
	// Variables privadas
	private FileWatcher.FileWatcherController? _watcher = null;

	/// <summary>
	///		Inicializa la aplicación
	/// </summary>
	public void Initialize(string folder, string? extension)
	{
		// Libera el observador si ya existía
		if (_watcher is not null)
			_watcher.StopWatching();
		// Genera el observador y lo arranca
		_watcher = new FileWatcher.FileWatcherController(this, folder, extension);
		_watcher.StartWatching();
	}

	/// <summary>
	///		Interpreta el archivo
	/// </summary>
	public Models.ChatFileModel Parse(string fileName) => new Parsers.IaHttpParser().Parse(fileName);

	/// <summary>
	///		Lanza el evento de archivo modificado
	/// </summary>
	internal void RaiseFileUpdated(string fileName)
	{
		FileUpdated?.Invoke(this, new EventArguments.ChangedFileEventArgs(fileName));
	}

	/// <summary>
	///		Libera la memoria
	/// </summary>
	protected virtual void Dispose(bool disposing)
	{
		if (!IsDisposed)
		{
			// Libera los objetos
			if (disposing && _watcher is not null)
			{
				// Detiene la observación del directorio
				_watcher.StopWatching();
				// Libera la memoria
				_watcher = null;
			}
			// Indica que se ha liberado la memoria
			IsDisposed = true;
		}
	}

	/// <summary>
	///		Libera la memoria
	/// </summary>
	void IDisposable.Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	/// <summary>
	///		Indica si se ha liberado la memoria
	/// </summary>
	public bool IsDisposed { get; private set; }
}
