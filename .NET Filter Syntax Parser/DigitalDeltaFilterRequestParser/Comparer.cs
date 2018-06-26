namespace DigitalDeltaFilterRequestParser
{
	/// <summary>
	/// Supported operators
	/// </summary>
	public enum Comparer
	{
		Unknown,
		Eq,
		Ne,
		Lt,
		Le,
		Ge,
		Gt,
		In,
		Notin,
		Like,
		Startswith,
		Endswith,
		Inbbox,
		Inpolygon,
		NotInbbox,
		NotInpolygon,
		Wkt,
		All
	}
}