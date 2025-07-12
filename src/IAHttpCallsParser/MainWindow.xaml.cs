using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using Bau.Controls.CodeEditor;
using IAHttpCallsParser.ViewModels;
using IAHttpCallsParser.ViewModels.ChatFile;

namespace IAHttpCallsParser;

/// <summary>
///		Ventana principal de la aplicación
/// </summary>
public partial class MainWindow : Window
{
	public MainWindow()
	{
		InitializeComponent();
	}

	/// <summary>
	///		Inicializa la ventana
	/// </summary>
	private void InitWindow()
	{
		// Inicializa el contexto
		DataContext = ViewModel = new IaParserViewModel(new Controllers.IAHttpCallsParserController("IAHttpCallsParser.Studio", this));
		// Carga el viewModel
		ViewModel.Load(GetLastFolder(), System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "IaParser"));
        // Inicializa los manejadores de eventos
        ViewModel.PropertyChanged += (sender, args) => {
                                                            if (!string.IsNullOrWhiteSpace(args.PropertyName))
                                                            {
                                                                if (args.PropertyName.Equals(nameof(ViewModel.Request), StringComparison.CurrentCultureIgnoreCase))
                                                                    UpdateText(udtRequest, ViewModel.Request);
                                                                else if (args.PropertyName.Equals(nameof(ViewModel.Response), StringComparison.CurrentCultureIgnoreCase))
                                                                    UpdateText(udtResponse, ViewModel.Response);
                                                                else if (args.PropertyName.Equals(nameof(ViewModel.FileContent), StringComparison.CurrentCultureIgnoreCase))
                                                                    UpdateText(udtFile, ViewModel.FileContent);
                                                            }
                                                        };
        // Inicializa los editores
        InitEditor(udtRequest);
        InitEditor(udtResponse);
        // Selecciona el tamaño del combo
        ViewModel.ComboFontSizes.SelectedId = Properties.Settings.Default.FontSize;
	}

    /// <summary>
    ///     Inicializa el editor
    /// </summary>
	private void InitEditor(ctlEditor udtEditor)
	{
        udtEditor.ShowLinesNumber = true;
        udtEditor.FontSize = 12;
        udtEditor.ChangeHighLightByExtension(".json");
	}

    /// <summary>
    ///     Actualiza el texto del editor
    /// </summary>
	private void UpdateText(ctlEditor udtEditor, string? text)
	{
        udtEditor.Text = Prettify(text);

        // Formatea una cadena JSON
        string Prettify(string? json)
        {
            if (!string.IsNullOrWhiteSpace(json))
            {
                try
                {
                    using (JsonDocument doucment = JsonDocument.Parse(json))
                    {
                        return JsonSerializer.Serialize(doucment, new JsonSerializerOptions { WriteIndented = true });
                    }
                }
                catch
                {
                    return json ?? string.Empty;
                }
            }
            else
                return string.Empty;
        }
	}

	/// <summary>
	///		Sale de la aplicación
	/// </summary>
	private void ExitApp()
	{
		// Cambia las propiedades de configuración
		Properties.Settings.Default.LastFolder = ViewModel.Folder;
		Properties.Settings.Default.FontSize = (int) ViewModel.FontSizeText;
		// y graba la configuración
		Properties.Settings.Default.Save();
	}

	/// <summary>
	///		Obtiene el nombre del último directorio
	/// </summary>
	private string? GetLastFolder() => Properties.Settings.Default.LastFolder;

	private void Window_Initialized(object sender, EventArgs e)
	{
		InitWindow();
	}

	private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
	{
		ExitApp();
	}

	private void trvExplorer_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
	{ 
		if (ViewModel is not null && trvExplorer.DataContext is IaParserViewModel)
            ViewModel.TreeMessagesViewModel.SelectedNode = (sender as TreeView)?.SelectedItem as NodeMessageViewModel;
	}

	private void mnuExitApp_Click(object sender, RoutedEventArgs e)
	{
		Close();
    }

	private void trvExplorer_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
	{
		if (ViewModel is not null && trvExplorer.DataContext is IaParserViewModel)
        {
            ViewModel.TreeMessagesViewModel.SelectedNode = (sender as TreeView)?.SelectedItem as NodeMessageViewModel;
            ViewModel.TreeMessagesViewModel.ExpandText();
        }
    }

	/// <summary>
	///		ViewModel del intérprete de archivos de IA
	/// </summary>
	public IaParserViewModel ViewModel { get; private set; } = default!;
}