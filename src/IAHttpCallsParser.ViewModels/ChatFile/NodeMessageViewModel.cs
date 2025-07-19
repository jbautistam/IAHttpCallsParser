using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems.Trees;
using Bau.Libraries.BauMvvm.ViewModels.Media;
using IAHttpCallsParser.Application.Models;
using IAHttpCallsParser.Application.Models.IaOutput.EmbeddingsResponse;
using IAHttpCallsParser.Application.Models.IaOutput.Embeddings;
using IAHttpCallsParser.Application.Models.IaOutput.ChatCompletionsResponse;
using IAHttpCallsParser.Application.Models.IaOutput.ChatCompletions;
using IAHttpCallsParser.Application.Models.IaOutput.Assistants;

namespace IAHttpCallsParser.ViewModels.ChatFile;

/// <summary>
///		ViewModel de un nodo de mensaje
/// </summary>
public class NodeMessageViewModel : ControlHierarchicalViewModel
{
	/// <summary>
	///		Tipo de nodo
	/// </summary>
	public enum NodeType
	{
		/// <summary>Archivo</summary>
		File,
		/// <summary>Llamada a un método</summary>
		Call,
		/// <summary>Nodo raíz</summary>
		Root,
		/// <summary>Herramienta del chat</summary>
		ChatTool,
		/// <summary>Mensaje de chat del usuario</summary>
		ChatUser,
		/// <summary>Mensaje de chat de la IA</summary>
		ChatBot,
		/// <summary>Mensaje de chat del asistente de la IA</summary>
		ChatBotAssistant,
		/// <summary>Respuesta o solicitud desconocida</summary>
		ChatUnknown,
		/// <summary>Embedding</summary>
		ChatEmbedding,
		/// <summary>Datos de respuesta a un mensaje</summary>
		ChatChoice
	}
	// Variables privadas
	private NodeType _nodeType;
	private string? _header, _footer, _fullText;
	private MvvmColor _background = MvvmColor.AliceBlue;
	private MvvmColor _border = MvvmColor.AliceBlue;
	private MvvmColor _foregroundHeader = MvvmColor.Gray;
	private bool _hasFullText, _hasHeader, _hasFooter;
	private double _fontSizeText, _fontSizeHeader;

	public NodeMessageViewModel(TreeMessagesViewModel trvTree, ControlHierarchicalViewModel? parent, string text, NodeType nodeType, bool lazyLoad, object? tag) 
				: base(parent, text, string.Empty, string.Empty, tag, lazyLoad, false, MvvmColor.Black)
	{
		// Inicializa las propiedades
		ViewModel = trvTree;
		TreeNodeType = nodeType;
		// Corta el texto si es demasiado largo
		if (!string.IsNullOrWhiteSpace(Text))
		{
			if (Text.Length > 500)
			{
				FullText = Text;
				Text = Text.Left(500) + " ...";
				HasFullText = true;
			}
		}
		// Inicializa las propiedades
		FontSizeText = ViewModel.MainViewModel.FontSizeText;
		FontSizeHeader = ViewModel.MainViewModel.FontSizeHeader;
		// Inicializa el manejador de eventos para cambiar el tamaño de las fuentes
		ViewModel.MainViewModel.ComboFontSizes.PropertyChanged += (sender, args) => {
																						if (!string.IsNullOrWhiteSpace(args.PropertyName) &&
																								args.PropertyName.Equals(nameof(ViewModel.MainViewModel.ComboFontSizes.SelectedItem),
																														 StringComparison.CurrentCultureIgnoreCase))
																						{
																							if (ViewModel.MainViewModel.ComboFontSizes.SelectedTag is double fontSize)
																								FontSizeText = fontSize;
																						}
																					};
		// Inicializa los colores
		InitColors();
	}

	/// <summary>
	///		Inicialza los colores
	/// </summary>
	private void InitColors()
	{
		switch (TreeNodeType)
		{
			case NodeType.File:
					Background = MvvmColor.Green;
					Border = MvvmColor.Green;
					Foreground = MvvmColor.White;
					IsBold = true;
				break;
			case NodeType.Root:
					Background = MvvmColor.NavajoWhite;
					Border = MvvmColor.NavajoWhite;
					Foreground = MvvmColor.Black;
					IsBold = true;
				break;
			case NodeType.Call:
					Background = MvvmColor.MediumSeaGreen;
					Border = MvvmColor.MediumSeaGreen;
					ForegroundHeader = MvvmColor.White;
				break;
			case NodeType.ChatEmbedding:
					Background = MvvmColor.PowderBlue;
					Border = MvvmColor.PowderBlue;
				break;
			case NodeType.ChatUser:
					Background = MvvmColor.MediumSeaGreen;
					Border = MvvmColor.MediumSeaGreen;
				break;
			case NodeType.ChatBot:
			case NodeType.ChatChoice:
			case NodeType.ChatBotAssistant:
					Background = MvvmColor.OliveDrab;
					Border = MvvmColor.OliveDrab;
					ForegroundHeader = MvvmColor.White;
				break;
			case NodeType.ChatTool:
					Background = MvvmColor.LemonChiffon;
					Border = MvvmColor.LemonChiffon;					
				break;
			case NodeType.ChatUnknown:
					Background = MvvmColor.Tomato;
					Border = MvvmColor.Tomato;
				break;
		}
	}

	/// <summary>
	///		Carga los datos hijo
	/// </summary>
	public override void LoadChildrenData()
	{
		if (Tag is ChatFileModel chatFileNode)
		{
			ChatFileModel chatFile = ViewModel.MainViewModel.IaHttpParserManager.Parse(chatFileNode.FileName);

				// Carga los mensajes del archivo
				LoadCalls(this, chatFile.Messages);
		}
	}

	/// <summary>
	///		Carga las llamadas a métodos del archivo
	/// </summary>
	private void LoadCalls(NodeMessageViewModel root, List<MessageModel> messages)
	{
		foreach (MessageModel message in messages)
		{
			NodeMessageViewModel node = CreateNode(root, message.GetPage(), NodeType.Call, message);

				// Asigna las propiedades
				node.IsBold = true;
				node.Header = message.Method.ToString();
				// Carga las secciones
				LoadSections(node, message);
		}
	}

	/// <summary>
	///		Carga las secciones
	/// </summary>
	private void LoadSections(NodeMessageViewModel root, MessageModel message)
	{
		NodeMessageViewModel node = CreateNode(root, "Request", NodeType.Root, message);

			// Carga los datos de la solicitud
			LoadRequestNodes(node, message.Request);
			// Crea el nodo de respuesta
			node = CreateNode(root, "Response", NodeType.Root, message);
			// Carga los datos de la respuesta
			LoadRequestNodes(node, message.Response);
	}

	/// <summary>
	///		Carga los nodos de la respuesta
	/// </summary>
	private void LoadRequestNodes(NodeMessageViewModel root, MessageSectionModel section)
	{
		switch (section.Parsed)
		{
			case ChatCompletionModel chatCompletion:
					LoadChatCompletionNodes(root, section.Message, chatCompletion);
				break;
			case ChatCompletionResponseModel chatResponse:
					LoadChatResponseNodes(root, section.Message, chatResponse);
				break;
			case ChatCompletionSkippedReason chatSkipped:
					LoadChatSkippedNodes(root, section.Message, chatSkipped);
				break;
			case EmbeddingModel embedding:
					LoadEmbeddingNode(root, section.Message, embedding);
				break;
			case EmbeddingResponseModel embeddingResponse:
					LoadEmbeddingResponseNode(root, section.Message, embeddingResponse);
				break;
			case AssistantModel assistant:
					LoadAssistantNode(root, section.Message, assistant);
				break;
			default:
					LoadBodyNode(root, section.Message, section);	
				break;
		}
	}

	/// <summary>
	///		Carga los nodos de chat
	/// </summary>
	private void LoadChatCompletionNodes(NodeMessageViewModel root, MessageModel message, ChatCompletionModel chatCompletion)
	{
		// Añade las funciones
		foreach (ChatCompletionToolModel tool in chatCompletion.Tools)
		{
			NodeMessageViewModel node = CreateNode(root, tool.Type, NodeType.ChatTool, message);

				// Asigna los datos de la función
				if (tool.Function is not null)
				{
					node.Header = tool.Function.Name;
					node.Text = tool.Function.Description;
				}
		}
		// Añade los mensajes
		foreach (ChatCompletionMessageModel chatMessage in chatCompletion.Messages)
		{
			NodeMessageViewModel node = CreateNode(root, chatMessage.Content, NodeType.ChatUser, message);

				// Cambia el texto si está vacío
				if (string.IsNullOrWhiteSpace(chatMessage.Content))
					node.Text = "No message content";
				// Cambia propiedades del nodo si estamos en un mensaje del bot
				if (!string.IsNullOrWhiteSpace(chatMessage.Role) && !chatMessage.Role.Equals("user", StringComparison.CurrentCultureIgnoreCase))
				{
					if (chatMessage.Role.Equals("assistant", StringComparison.CurrentCultureIgnoreCase))
						node.TreeNodeType = NodeType.ChatBotAssistant;
					else
						node.TreeNodeType = NodeType.ChatBot;
					node.Header = chatMessage.Name;
					node.Footer = chatMessage.Role;
				}
				// Carga las llamadas a herramientas
				if (chatMessage.Tool_Calls is not null)
					LoadChatMessageToolCall(node, message, chatMessage.Tool_Calls);
		}
	}

	/// <summary>
	///		Carga los nodos de respuesta del chat
	/// </summary>
	private void LoadChatResponseNodes(NodeMessageViewModel root, MessageModel message, ChatCompletionResponseModel chatResponse)
	{
		foreach (ChatCompletionResponseChoiceModel choice in chatResponse.Choices)
		{
			// Razón de finalización
			if (!string.IsNullOrWhiteSpace(choice.Finish_Reason))
			{
				NodeMessageViewModel nodeReason = CreateNode(root, choice.Finish_Reason, NodeType.ChatChoice, message);

					// Asigna las propiedades
					nodeReason.Header = "Finish reason";
			}
			// Datos del mensaje
			if (choice.Message is not null)
			{
				string content = string.IsNullOrWhiteSpace(choice.Message.Content) ? "No output message" : choice.Message.Content;
				NodeMessageViewModel node = CreateNode(root, content, NodeType.ChatBot, message);

					// Asigna las propiedades
					node.Header = choice.Message.Role;
					// Crea los nodos de llamadas a funciones
					LoadChatMessageToolCall(node, message, choice.Message.Tool_Calls);
			}
		}
	}

	/// <summary>
	///		Carga las llamadas a herramientas en un mensaje
	/// </summary>
	private void LoadChatMessageToolCall(NodeMessageViewModel root, MessageModel message, List<ChatCompletionResponseChoiceMessageToolCallModel> tool_Calls)
	{
		foreach (ChatCompletionResponseChoiceMessageToolCallModel call in tool_Calls)
			if (call.Function is not null)
			{
				NodeMessageViewModel nodeTool = CreateNode(root, call.Function.Name, NodeType.ChatTool, message);

					// Asigna las propiedades
					nodeTool.Header = call.Type;
					nodeTool.Footer = call.Function.Arguments;
			}
			else
			{
				NodeMessageViewModel nodeTool = CreateNode(root, call.Type, NodeType.ChatTool, message);

					// Asigna las propiedades
					nodeTool.Footer = call.Id;
			}
	}

	/// <summary>
	///		Añade un nodo que indica que la respuesta se ha omitido (por ejemplo, en streaming)
	/// </summary>
	private void LoadChatSkippedNodes(NodeMessageViewModel root, MessageModel message, ChatCompletionSkippedReason chatSkipped)
	{
		CreateNode(root, chatSkipped.Skipped, NodeType.ChatUnknown, message);
	}

	/// <summary>
	///		Carga el nodo de embedding
	/// </summary>
	private void LoadEmbeddingNode(NodeMessageViewModel root, MessageModel message, EmbeddingModel embedding)
	{
		NodeMessageViewModel node = CreateNode(root, embedding.GetInput(), NodeType.ChatEmbedding, message);

			// Asigna las propiedades
			node.Header = embedding.Model;
			node.Footer = embedding.Encoding_Format;
	}

	/// <summary>
	///		Carga el nodo de respuesta del embedding
	/// </summary>
	private void LoadEmbeddingResponseNode(NodeMessageViewModel root, MessageModel message, EmbeddingResponseModel embedding)
	{
		NodeMessageViewModel node = CreateNode(root, embedding.Model, NodeType.ChatEmbedding, message);

			// Asigna las propiedades de uso
			if (embedding.Usage is not null)
				node.Header = $"Prompt tokens: {embedding.Usage.Prompt_Tokens:#,##0}. Total tokens: {embedding.Usage.Total_Tokens:#,##0}";
	}

	/// <summary>
	///		Carga el nodo de respuesta del asistente
	/// </summary>
	private void LoadAssistantNode(NodeMessageViewModel root, MessageModel message, AssistantModel assistant)
	{
		NodeMessageViewModel node = CreateNode(root, assistant.Description, NodeType.ChatBotAssistant, message);

			// Asigna las propiedades
			node.Header = assistant.Name;
			node.Footer = assistant.Model;
			// Añade los datos adicionales
			CreateNode(node, assistant.Instructions, NodeType.ChatBotAssistant, message);
			foreach (AssistantToolModel tool in assistant.Tools)
				CreateNode(node, tool.Type, NodeType.ChatTool, message);
	}

	/// <summary>
	///		Crea un nodo con el cuerpo de la sección
	/// </summary>
	private void LoadBodyNode(NodeMessageViewModel root, MessageModel message, MessageSectionModel section)
	{
		string body = section.Body;

			// Ajusta el cuerpo
			if (string.IsNullOrWhiteSpace(body))
			{
				if (section.IsRequest)
					body = "Empty request body";
				else
					body = "No server response";
			}
			// Crea el nodo
			CreateNode(root, body, NodeType.ChatUnknown, message);
	}

	/// <summary>
	///		Crea un nodo
	/// </summary>
	private NodeMessageViewModel CreateNode(NodeMessageViewModel? parent, string text, NodeType type, object? tag)
	{
		NodeMessageViewModel node = new(ViewModel, parent, text, type, false, tag);

			// Añade el nodo
			if (parent is null)
				Children.Add(node);
			else
				parent.Children.Add(node);
			// Devuelve el nodo creado
			return node;
	}

	/// <summary>
	///		ViewModel del árbol
	/// </summary>
	public TreeMessagesViewModel ViewModel { get; }

	/// <summary>
	///		Tipo de nodo
	/// </summary>
	public NodeType TreeNodeType
	{
		get { return _nodeType; }
		set 
		{ 
			if (CheckProperty(ref _nodeType, value))
				InitColors();
		}
	}

	/// <summary>
	///		Cabecera
	/// </summary>
	public string? Header
	{
		get { return _header; }
		set 
		{ 
			if (CheckProperty(ref _header, value))
				HasHeader = !string.IsNullOrWhiteSpace(_header);
		}
	}

	/// <summary>
	///		Indica si tiene cabecera
	/// </summary>
	public bool HasHeader
	{
		get { return _hasHeader; }
		set { CheckProperty(ref _hasHeader, value); }
	}

	/// <summary>
	///		Texto completo
	/// </summary>
	public string? FullText
	{
		get { return _fullText; }
		set { CheckProperty(ref _fullText, value); }
	}

	/// <summary>
	///		Indica si tiene texto completo
	/// </summary>
	public bool HasFullText
	{
		get { return _hasFullText; }
		set { CheckProperty(ref _hasFullText, value); }
	}

	/// <summary>
	///		Pie
	/// </summary>
	public string? Footer
	{
		get { return _footer; }
		set 
		{ 
			if (CheckProperty(ref _footer, value))
				HasFooter = !string.IsNullOrWhiteSpace(_footer);
		}
	}

	/// <summary>
	///		Indica si tiene un pie
	/// </summary>
	public bool HasFooter
	{
		get { return _hasFooter; }
		set { CheckProperty(ref _hasFooter, value); }
	}

	/// <summary>
	///		Color de fondo
	/// </summary>
	public MvvmColor Background
	{
		get { return _background; }
		set { CheckProperty(ref _background, value); }
	}

	/// <summary>
	///		Color del borde
	/// </summary>
	public MvvmColor Border
	{
		get { return _border; }
		set { CheckProperty(ref _border, value); }
	}

	/// <summary>
	///		Color de texto de cabecera / pie
	/// </summary>
	public MvvmColor ForegroundHeader
	{
		get { return _foregroundHeader; }
		set { CheckProperty(ref _foregroundHeader, value); }
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
}
