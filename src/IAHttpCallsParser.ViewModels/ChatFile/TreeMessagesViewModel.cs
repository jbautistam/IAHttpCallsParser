using Bau.Libraries.BauMvvm.ViewModels;
using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems.Trees;
using IAHttpCallsParser.Application.Models;

namespace IAHttpCallsParser.ViewModels.ChatFile;

/// <summary>
///		ViewModel del explorador
/// </summary>
public class TreeMessagesViewModel : TreeViewModel
{
	// Variables privadas
	private string? _searchText, _lastSearch;

	public TreeMessagesViewModel(IaParserViewModel mainViewModel)
	{
		MainViewModel = mainViewModel;
		SearchNextCommand = new BaseCommand(_ => Search(SearchText, true), _ => !string.IsNullOrWhiteSpace(SearchText))
									.AddListener(this, nameof(SearchText));
		SearchPreviousCommand = new BaseCommand(_ => Search(SearchText, false), _ => !string.IsNullOrWhiteSpace(SearchText))
									.AddListener(this, nameof(SearchText));
		DeleteCommand = new BaseCommand(_ => Delete(), _ => SelectedNode is not null)
								.AddListener(this, nameof(SelectedNode));
	}

	/// <summary>
	///		Carga los nodos
	/// </summary>
	public override void Load()
	{
		object state = new();

			// Carga los nodos en el árbol
			//? _contexUi mantiene el contexto de sincronización que creó el ViewModel (que debería ser la interface de usuario)
			//? Al generarse las tablas en otro hilo o desde un evento, no se puede borrar ObservableCollection sin una
			//? excepción del tipo "Este tipo de CollectionView no admite cambios en su SourceCollection desde un hilo diferente del hilo Dispatcher"
			//? Por eso se tiene que añadir el mensaje de log desde el contexto de sincronización de la UI
			ContextUI.Send(_ => {
									ControlHierarchicalViewModel? previousSelectedNode = SelectedNode;
									List<ControlHierarchicalViewModel> nodesExpanded = GetNodesExpanded(Children);

										// Limpia la colección de hijos
										Children.Clear();
										// Añade los nodos raíz
										AddRootNodes();
										// Expande los nodos previamente abiertos
										ExpandNodes(Children, nodesExpanded);
								},
								state
						  );
	}

	/// <summary>
	///		Añade los nodos raíz
	/// </summary>
	protected override void AddRootNodes()
	{
		if (!string.IsNullOrWhiteSpace(MainViewModel.PathDocuments) && Directory.Exists(MainViewModel.PathDocuments))
		{
			List<FileInfo> files = GetFolderFiles(MainViewModel.PathDocuments);

				// Ordena la lista de archivos
				files.Sort((first, second) => -1 * first.CreationTime.CompareTo(second.CreationTime));
				// Cambia los archivos
				foreach (FileInfo file in files)
				{
					NodeMessageViewModel node = new(this, null, Path.GetFileNameWithoutExtension(file.FullName), NodeMessageViewModel.NodeType.File, true, 
													new ChatFileModel(file.FullName));

						// Añade el nodo al árbol
						Children.Add(node);
				}
		}
	}

	/// <summary>
	///		Obtiene la información de los archivos de una carpeta
	/// </summary>
	private List<FileInfo> GetFolderFiles(string folder)
	{
		List<FileInfo> files = [];

			// Añade la información de archivos
			foreach (string fileName in Directory.GetFiles(folder, "*.http"))
				files.Add(new FileInfo(fileName));
			// Devuelve la lista de archivos
			return files;
	}

	/// <summary>
	///		Borra el archivo
	/// </summary>
	private void Delete()
	{
		if (SelectedNode is not null)
		{
			ChatFileModel? selectedFile = null;

				// Obtiene el archivo seleccionado
				if (SelectedNode.Tag is ChatFileModel file)
					selectedFile = file;
				if (selectedFile is null && SelectedNode.Tag is MessageModel message)
					selectedFile = message.File;
				// Borra el archivo
				if (selectedFile is not null &&
					MainViewModel.IAHttpCallsParserController.HostController.SystemController.ShowQuestion($"Do you want to delete the file {Path.GetFileNameWithoutExtension(selectedFile.FileName)}?"))
				{
					// Borra el archivo
					Bau.Libraries.LibHelper.Files.HelperFiles.KillFile(selectedFile.FileName, true);
					// Actualiza el árbol
					Load();
				}
		}
	}

	/// <summary>
	///		Expande / colapsa el texto
	/// </summary>
	public void ExpandText()
	{
		if (SelectedNode is NodeMessageViewModel node)
		{
			if (!string.IsNullOrWhiteSpace(node.FullText))
			{
				string temporal = node.FullText;

					// Cambia los textos
					node.FullText = node.Text;
					node.Text = temporal;
			}
		}
	}

	/// <summary>
	///		Busca un texto en los nodos del árbol
	/// </summary>
	private void Search(string? search, bool next)
	{
		if (!string.IsNullOrWhiteSpace(search))
		{
			List<ControlHierarchicalViewModel> nodes = Flatten(GetNodesExpanded(Children));

				if (nodes.Count > 0)
				{
					ControlHierarchicalViewModel? firstNode = SelectedNode;
					ControlHierarchicalViewModel? foundNode = null;
					int index = 0;
					int increment = 1;

						// Si estamos buscar el anterior, cambiamos el incremento
						if (!next)
							increment = -1;
						// Obtiene el nodo inicial
						if (firstNode is null)
							firstNode = nodes[0];
						// Indice del nodo
						index = nodes.IndexOf(firstNode);
						// Ajusta el índice
						if (!string.IsNullOrWhiteSpace(_lastSearch) && _lastSearch.Equals(search, StringComparison.CurrentCultureIgnoreCase))
							index += increment;
						// Busca el siguiente texto
						while ((index >= 0 && index < nodes.Count) && foundNode is null)
						{
							// Comprueba el texto de búsqueda
							if (!string.IsNullOrWhiteSpace(nodes[index].Text) && nodes[index].Text.Contains(search, StringComparison.CurrentCultureIgnoreCase))
								foundNode = nodes[index];
							// Incrementa el índice
							index += increment;
						}
						// Si se ha encontrado el nodo, se selecciona
						if (foundNode is not null)
						{
							// Expande los padres
							ExpandTo(foundNode);
							// Cambia el nodo seleccionado y lo selecciona para que se vea en pantalla
							SelectedNode = foundNode;
							foundNode.IsSelected = true;
						}
						// Guarda el texto
						_lastSearch = search;
				}
		}
	}

	/// <summary>
	///		Expande hasta el nodo
	/// </summary>
	private void ExpandTo(ControlHierarchicalViewModel foundNode)
	{
		if (foundNode.Parent is not null)
		{
			// Abre el nodo padre
			foundNode.Parent.IsExpanded = true;
			// Expande los antecesores
			ExpandTo(foundNode.Parent);
		}
	}

	/// <summary>
	///		Aplana la estructura de nodos de un árbol en una lista
	/// </summary>
	private List<ControlHierarchicalViewModel> Flatten(List<ControlHierarchicalViewModel> nodes)
	{
		List<ControlHierarchicalViewModel> result = [];
			
			// Añade los nodos del nivel y sus hijos
			foreach (ControlHierarchicalViewModel node in nodes)
			{
				// Añade el nodo
				result.Add(node);
				// Añade los nodos hijo aplanados
				if (node.Children.Count > 0)
					result.AddRange(Flatten(node.Children.ToList()));
			}
			// Devuelve el resultado
			return result;
	}

	/// <summary>
	///		ViewModel principal
	/// </summary>
	public IaParserViewModel MainViewModel { get; }

	/// <summary>
	///		Texto de búsqueda
	/// </summary>
	public string? SearchText
	{
		get { return _searchText; }
		set { CheckProperty(ref _searchText, value); }
	}

	/// <summary>
	///		Comando para buscar texto en los siguientes nodos
	/// </summary>
	public BaseCommand SearchNextCommand { get; }

	/// <summary>
	///		Comando para buscar el texto en los nodos anteriores
	/// </summary>
	public BaseCommand SearchPreviousCommand { get; }

	/// <summary>
	///		Comando para borrar un archivo
	/// </summary>
	public BaseCommand DeleteCommand { get; }
}