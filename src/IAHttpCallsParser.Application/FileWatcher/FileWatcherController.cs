namespace IAHttpCallsParser.Application.FileWatcher;

/// <summary>
///		Controlador para observar modificaciones de archivos
/// </summary>
public class FileWatcherController(IAHttpCallsParserManager manager, string folder, string? extension)
{
	// Variables privadas
	private FileSystemWatcher? _watcher = null;

	/// <summary>
	///		Comienza la observación de archivos
	/// </summary>
	public bool StartWatching()
	{
		// Detiene la observación
		StopWatching();
		// Asigna el manejador de eventos de observación de archivos
		if (!string.IsNullOrWhiteSpace(Folder) && Directory.Exists(Folder))
		{
			// Inicializa el observador
            _watcher = new FileSystemWatcher();  
            _watcher.Path = Folder;
            _watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;  
			if (!string.IsNullOrWhiteSpace(Extension))
				_watcher.Filter = GetMask(Extension);  
			// Asigna los manejadores de eventos
			_watcher.Created += (sender, args) => TreatFileUpdate(args);
            _watcher.Changed += (sender, args) => TreatFileUpdate(args);
            _watcher.EnableRaisingEvents = true;  
			// Indica que está observando
			IsWatching = true;
		}
		// Devuelve el valor que indica si está observando el directorio
		return IsWatching;

		// Obtiene la máscara de archivos a partir de la extensión
		string GetMask(string? extension)
		{
			if (string.IsNullOrWhiteSpace(extension))
				return "*.*";
			else
			{
				// Quita espacios
				extension = extension.Trim();
				// Añade el punto inicial
				if (!extension.StartsWith("."))
					extension = "." + extension;
				// Devuelve la extensión
				return $"*{extension}";
			}
		}
	}

	/// <summary>
	///		Detiene la observación de archvios
	/// </summary>
	public void StopWatching()
	{
		// Detiene la observación de archivos
		if (_watcher is not null)
		{
			_watcher.Dispose();
			_watcher = null;
		}
		// Indica que ya no está observando archivos
		IsWatching = false;
	}

	/// <summary>
	///		Trata el evento de modificación de archivo
	/// </summary>
	private void TreatFileUpdate(FileSystemEventArgs args)
	{
		if (args.ChangeType == WatcherChangeTypes.Changed)
			Manager.RaiseFileUpdated(args.FullPath);
	}

	/// <summary>
	///		Manager principal
	/// </summary>
	public IAHttpCallsParserManager Manager { get; } = manager;

	/// <summary>
	///		Carpeta que se controla
	/// </summary>
	public string Folder { get; } = folder;

	/// <summary>
	///		Extensión que se controla
	/// </summary>
	public string? Extension { get; } = extension;

	/// <summary>
	///		Indica si se está observando el cambio de archivos del directorio
	/// </summary>
	public bool IsWatching { get; private set; }
}