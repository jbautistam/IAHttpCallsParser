using Bau.Libraries.BauMvvm.ViewModels;
using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems.ComboItems;
using IAHttpCallsParser.Application;
using IAHttpCallsParser.Application.Models;

namespace IAHttpCallsParser.ViewModels;

/// <summary>
///		ViewModel principal
/// </summary>
public class IaParserViewModel : BaseObservableObject
{
	// Variables privadas
	private string? _folder, _pathDocuments;
    private ChatFile.TreeMessagesViewModel _treeMessagesViewModel = default!;
    private string? _request, _response, _lastFile, _fileContent;
	private ComboViewModel _comboFontSizes = default!;
	private double _fontSizeText, _fontSizeHeader;

	public IaParserViewModel(Interfaces.IIAHttpCallsParserController iaHttpCallsParserController)
	{
		// Inicializa las propiedades
		IAHttpCallsParserController = iaHttpCallsParserController;
		TreeMessagesViewModel = new ChatFile.TreeMessagesViewModel(this);
		// Inicializa el manager de mensajes
		IaHttpParserManager = new IAHttpCallsParserManager();
		IaHttpParserManager.FileUpdated += (sender, args) => TreatFileUpdated(args.FileName);
		// Inicializa el viewModel
		InitViewModel();
		// Inicializa los comandos
		OpenFolderCommand = new BaseCommand(_ => UpdateFolder());
		RefreshCommand = new BaseCommand(_ => Refresh());
	}

    /// <summary>
    ///		Inicializa los datos
    /// </summary>
    private void InitViewModel()
    {
        // Inicializa el árbol
        TreeMessagesViewModel = new ChatFile.TreeMessagesViewModel(this);
        TreeMessagesViewModel.Load();
        TreeMessagesViewModel.PropertyChanged += (sender, args) => {
                                                                        if (!string.IsNullOrWhiteSpace(args.PropertyName) &&
																				args.PropertyName.Equals(nameof(TreeMessagesViewModel.SelectedNode), 
																										 StringComparison.CurrentCultureIgnoreCase))
                                                                            LoadFileContentViewModel();
                                                                   };
		// Carga el combo de tamaños de fuente
		FontSizeText = 14;
		LoadComboFontSizes();
        // Indica que aún no ha habido modificaciones
        IsUpdated = false;
    }

	/// <summary>
	///		Carga el combo de tamaños de fuente
	/// </summary>
	private void LoadComboFontSizes()
	{
		// Inicializa el combo
		ComboFontSizes = new ComboViewModel(this);
		// Añade los elementos
		for (int index = 8; index <= 50; index += 2)
			AddItem(index);
		// Añade el manejador de eventos
		ComboFontSizes.PropertyChanged += (sender, args) => {
																if (!string.IsNullOrWhiteSpace(args.PropertyName) &&
																		args.PropertyName.Equals(nameof(ComboFontSizes.SelectedItem),
																								 StringComparison.CurrentCultureIgnoreCase) &&
																		ComboFontSizes.SelectedTag is double fontSize)
																	FontSizeText = fontSize;
															};

		// Añade un elemento al combo
		void AddItem(double fontSize)
		{
			// Añade el elemento
			ComboFontSizes.AddItem((int) fontSize, ((int) fontSize).ToString(), fontSize);
			// Selecciona el tamaño de fuente
			if (fontSize == FontSizeText)
				ComboFontSizes.SelectedIndex = ComboFontSizes.Items.Count - 1;
		}
	}

	/// <summary>
	///		Carga los datos
	/// </summary>
	public void Load(string? folder, string? pathDocuments)
	{
		// Guarda las carpetas
		Folder = folder;
		PathDocuments = pathDocuments;
		// Carga el archivo
		if (!string.IsNullOrWhiteSpace(folder))
		{
			// Inicializa el observador de archivos
			IaHttpParserManager.Initialize(folder, "http");
			// Añade los archivos que se encuentre ya creados al directorio de documentos (puede que recupere archivos que se han borrado
			if (Directory.Exists(folder))
				foreach (string fileName in Directory.GetFiles(folder, "*.http"))
					TreatFileUpdated(fileName);
		}
		// Actualiza la pantalla
		Refresh();
	}

	/// <summary>
	///		Trata el evento de archivos del directorio modificados
	/// </summary>
	private void TreatFileUpdated(string fileName)
	{
		if (!string.IsNullOrWhiteSpace(PathDocuments) && !string.IsNullOrWhiteSpace(fileName) && File.Exists(fileName))
		{
			FileInfo file = new(fileName);

				// Copia el archivo en el directorio de documentos
				Bau.Libraries.LibHelper.Files.HelperFiles.MakePath(PathDocuments);
				Bau.Libraries.LibHelper.Files.HelperFiles.CopyFile(fileName, 
																   Path.Combine(PathDocuments, 
																				$"IaParser_{file.CreationTime:yyyy_MM_dd_HH_mm_ss}{Path.GetExtension(fileName)}")
																  );
				// Actualiza el árbol
				Refresh();
		}
	}

	/// <summary>
	///		Cambia la carpeta donde están los archivos
	/// </summary>
	public void UpdateFolder() 
	{
		// Solicita el directorio
		IAHttpCallsParserController.HostController.DialogsController.OpenDialogSelectPath(IAHttpCallsParserController.HostController.DialogsController.LastPathSelected, out string? folder);
		// Carga los datos
		if (!string.IsNullOrWhiteSpace(folder))
			Load(folder, _pathDocuments);
	}

    /// <summary>
    ///     Carga el contenido
    /// </summary>
	private void LoadFileContentViewModel()
	{
        if (TreeMessagesViewModel.SelectedNode is not null)
        {
			ChatFileModel? selectedChatFile = null;
			MessageModel? selectedMessage = null;

				// Obtiene los datos del árbol
				if (TreeMessagesViewModel.SelectedNode.Tag is ChatFileModel chatFile)
					selectedChatFile = chatFile;
				else if (TreeMessagesViewModel.SelectedNode.Tag is MessageModel message)
				{
					selectedChatFile = message.File;
					selectedMessage = message;
				}
				// Guarda los datos del archivo
				if (selectedChatFile is not null)
				{
					if (string.IsNullOrWhiteSpace(LastFile) || !LastFile.Equals(selectedChatFile.FileName, StringComparison.CurrentCultureIgnoreCase))
					{
						LastFile = selectedChatFile.FileName;
						FileContent = Bau.Libraries.LibHelper.Files.HelperFiles.LoadTextFile(LastFile);
					}
				}
				// Guarda los datos del mensaje
				if (selectedMessage is null)
				{
					Request = string.Empty;
					Response = string.Empty;
				}
				else
				{
					Request = selectedMessage.Request.Body;
					Response = selectedMessage.Response.Body;
				}
        }
	}

	/// <summary>
	///		Actualiza la pantalla
	/// </summary>
	public void Refresh()
	{
		TreeMessagesViewModel.Load();
	}

    /// <summary>
    ///     Arbol de mensajes
    /// </summary>
    public ChatFile.TreeMessagesViewModel TreeMessagesViewModel
    {
        get { return _treeMessagesViewModel; }
        set { CheckObject(ref _treeMessagesViewModel!, value); }
    }

	/// <summary>
	///		Controlador
	/// </summary>
	public Interfaces.IIAHttpCallsParserController IAHttpCallsParserController { get; }

	/// <summary>
	///		Manager de la aplicación de mensajes de la IA
	/// </summary>
	internal IAHttpCallsParserManager IaHttpParserManager { get; }

    /// <summary>
    ///     Texto con el cuerpo de la solicitud
    /// </summary>
    public string? Request
    {
        get { return _request; }
        set { CheckProperty(ref _request, value); }
    }

    /// <summary>
    ///     Texto con el cuerpo de la respuesta
    /// </summary>
    public string? Response
    {
        get { return _response; }
        set { CheckProperty(ref _response, value); }
    }

	/// <summary>
	///		Ultimo archivo cargado
	/// </summary>
	public string? LastFile
	{
		get { return _lastFile; }
		set { CheckProperty(ref _lastFile, value); }
	}

	/// <summary>
	///		Contenido del arhivo
	/// </summary>
	public string? FileContent
	{
		get { return _fileContent; }
		set { CheckProperty(ref _fileContent, value); }
	}

	/// <summary>
	///		Nombre del directorio donde escuchamos los archivos
	/// </summary>
	public string? Folder 
	{ 
		get { return _folder; } 
		set { CheckProperty(ref _folder, value); }
	}

	/// <summary>
	///		Nombre de directorio donde copiamos los archivos modificados
	/// </summary>
	public string? PathDocuments
	{ 
		get { return _pathDocuments; } 
		set { CheckProperty(ref _pathDocuments, value); }
	}

	/// <summary>
	///		Combo de tamaños de fuente
	/// </summary>
	public ComboViewModel ComboFontSizes
	{
		get { return _comboFontSizes; }
		set { CheckObject(ref _comboFontSizes!, value); }
	}

	/// <summary>
	///		Tamaño de la fuente de texto
	/// </summary>
	public double FontSizeText
	{
		get { return _fontSizeText; }
		set 
		{ 
			if (CheckProperty(ref _fontSizeText, value))
				FontSizeHeader = value - 2;
		}
	}
	
	/// <summary>
	///		Tamaño de la fuente de cabecera / pie
	/// </summary>
	public double FontSizeHeader
	{
		get { return _fontSizeHeader; }
		set { CheckProperty(ref _fontSizeHeader, value); }
	}

	/// <summary>
	///		Comando para cambiar de directorio
	/// </summary>
	public BaseCommand OpenFolderCommand { get; }

	/// <summary>
	///		Comando para actualizar el árbol
	/// </summary>
	public BaseCommand RefreshCommand { get; }
}