using Bau.Libraries.BauMvvm.ViewModels;

namespace IAHttpCallsParser.ViewModels.Headers;

/// <summary>
///		ViewModel para una cabecera
/// </summary>
public class HeaderViewModel : BaseObservableObject
{
	private string _key = default!;
	private string? _value;

	public HeaderViewModel(string key, string? value)
	{
		Key = key;
		Value = value;
	}

	/// <summary>
	///		Clave de la cabecera
	/// </summary>
	public string Key
	{
		get { return _key; }
		set { CheckProperty(ref _key, value); }
	}

	/// <summary>
	///		Valor de la cabecera
	/// </summary>
	public string? Value
	{
		get { return _value; }
		set { CheckProperty(ref _value, value); }
	}
}