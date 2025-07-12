using IAHttpCallsParser.Application;
using IAHttpCallsParser.Application.Models;

ChatFileModel file = new IAHttpCallsParserManager().Parse(@"C:\Projects\Repos\Poc\IAHttpCallsParser\Data\Sample.http");

	// Muestra los datos
	foreach (MessageModel message in file.Messages)
	{
		Console.WriteLine($"Message {file.Messages.IndexOf(message).ToString()}");
		Console.WriteLine($"{message.Method.ToString()}: {message.Url}");
		Console.WriteLine();
		WriteRequest(message.Request, "Request");
		WriteRequest(message.Response, "Response");
		Console.WriteLine(new string('=', 30));
		Console.WriteLine(new string('=', 30));
		Console.WriteLine();
	}

// Escribe los datos de una sección del mensaje
void WriteRequest(MessageSectionModel section, string title)
{
	Console.WriteLine(title);
	Console.WriteLine("Headers");
	foreach ((string key, string? value) in section.Headers)
		Console.WriteLine($"\t{key}: {value}");
	Console.WriteLine(new string('-', 30));
	Console.WriteLine("Body");
	Console.WriteLine(section.Body);
	Console.WriteLine(new string('-', 30));
	Console.WriteLine();
}