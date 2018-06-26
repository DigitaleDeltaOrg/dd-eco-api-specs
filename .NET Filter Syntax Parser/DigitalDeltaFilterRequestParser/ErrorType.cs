namespace DigitalDeltaFilterRequestParser
{
	/// <summary>
	/// Possible error types.
	/// </summary>
	public enum ErrorType
	{
		None                           = 0,
		UnknownField                   = 1,
		UnknownOperator                = 2,
		InvalidValue                   = 3,
		InvalidBbox                    = 4,
		InvalidSyntax                  = 5,
		InvalidOperatorForField        = 6,
		InvalidTypeForFieldAndOperator = 7,
		DuplicatesInList               = 8,
		EmptyList                      = 9,
		InvalidPolygon                 = 10,
		InvalidWkt										 = 11
	}
}