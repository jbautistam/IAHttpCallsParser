using System.Windows.Data;

using IAHttpCallsParser.ViewModels.ChatFile;

namespace IAHttpCallsParser.Converters;

/// <summary>
///		Conversor de iconos a partir del nombre de archivo
/// </summary>
public class NodeIconConverter : IValueConverter
{
	/// <summary>
	///		Convierte un tipo en un icono
	/// </summary>
	public object? Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
	{ 
		if (value is NodeMessageViewModel.NodeType nodeType)
			return GetIcon(nodeType);
		else
			return null;
	}

	/// <summary>
	///		Obtiene la imagen asociada a un icono
	/// </summary>
	private object? GetIcon(NodeMessageViewModel.NodeType type)
	{
		return $"/Resources/Images/{GetImage(type)}";

		// Obtiene el nombre de la imagen
		string GetImage(NodeMessageViewModel.NodeType type)
		{
			return type switch
					{
						NodeMessageViewModel.NodeType.File => "File.png",
						NodeMessageViewModel.NodeType.Call => "RestCall.png",
						NodeMessageViewModel.NodeType.ChatUser => "ChatUser.png",
						NodeMessageViewModel.NodeType.ChatBot => "ChatBot.png",
						NodeMessageViewModel.NodeType.ChatBotAssistant => "ChatBotAssistant.png",
						NodeMessageViewModel.NodeType.ChatEmbedding => "ChatEmbedding.png",
						NodeMessageViewModel.NodeType.ChatChoice => "ChatChoice.png",
						NodeMessageViewModel.NodeType.ChatTool => "ChatTool.png",
						NodeMessageViewModel.NodeType.ChatUnknown => "ChatUnknown.png",
						_ => "Folder.png"
					};
		}
	}

	/// <summary>
	///		Convierte un valor de vuelta
	/// </summary>
	public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
	{ 
		throw new NotImplementedException();
	}
}
