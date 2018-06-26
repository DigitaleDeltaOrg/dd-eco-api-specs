using System.Collections.Generic;

namespace DigitalDeltaFilterRequestParser
{
	/// <summary>
	// Defines a field name, and it's allowable operation/data type combinations.
	/// </summary>
	public class FieldDefinition
	{
		public string FieldName { get; }
		public List<ComparerDataType> ComparerDataTypes { get; }

		public FieldDefinition(string fieldName, List<ComparerDataType> comparerDataTypes)
		{
			FieldName = fieldName;
			ComparerDataTypes = comparerDataTypes;
		}

	}
}