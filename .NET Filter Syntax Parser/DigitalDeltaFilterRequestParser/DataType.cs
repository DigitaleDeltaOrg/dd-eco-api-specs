namespace DigitalDeltaFilterRequestParser
{
	/// <summary>
	/// Supported data types.
	/// </summary>
	public enum DataType
	{
		StringType         = 1,
		NumericType        = 2,
		DateType           = 3,
		ArrayOfStringType  = 4,
		ArrayOfNumericType = 5,
		BboxType           = 6,
		PolygonType        = 7,
		WktType						 = 8
	}
}