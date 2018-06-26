using System;
using System.Collections.Generic;

namespace DigitalDeltaFilterRequestParser
{
	/// <summary>
	/// Defines a statement: a part of the filter that includes field name, operator and field value.
	/// </summary>
	public class Condition
	{
		public string FieldName { set; get; }
		public Comparer Comparers { set; get; }
		public string StringValue { set; get; }
		public DateTime DateTimeValue { set; get; }
		public decimal NumericValue { set; get; }
		public List<string> ArrayOfStringValues { set; get; }
		public List<decimal> ArrayOfNumericValues { set; get; }
		public DataType ParameterDataType { set; get; }
		public List<(string, string)> KeyValuePairs { set; get; }
		public List<Error> Errors { set; get; }

		/// <summary>
		/// Constructor
		/// </summary>
		public Condition()
		{
			Errors = new List<Error>();
		}
	}
}