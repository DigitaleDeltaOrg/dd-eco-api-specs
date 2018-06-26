using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Wkx;

namespace DigitalDeltaFilterRequestParser
{
	/// <summary>
	/// Main filter class. Parses a line into separate statements, and statements into fieldname, operator and value components.
	/// </summary>
	public class FilterParser
	{
		#region Public
		public string Filter { private set; get; }
		public List<Condition> Conditions { private set; get; }
		public bool IsValid { private set; get; }
		public char ComponentSeparator { get; }
		public char ComparerSeparator { get; }
		public string DateFormat { get; }
		public List<FieldDefinition> Fields { get; }

		/// <summary>
		/// Construct and configure the parser.
		/// </summary>
		/// <param name="fields">List of fields definitions allowed by the parser. </param>
		/// <param name="comparerSeparator">Separator character for statements. Defaults to ';'.</param>
		/// <param name="componentSeparator">Separator character for components. Defaults to ':'</param>
		/// <param name="dateFormat">Date format for parsing dates. Defaults to "yyyy-MM-dd".</param>
		/// <param name="stringSeparator">Separator for strings.</param>
		public FilterParser(List<FieldDefinition> fields, char comparerSeparator = ';', char componentSeparator = ':', string dateFormat = "yyyy-MM-dd", char stringSeparator = '"')
		{
			Fields = fields;
			IsValid = true;
			Conditions = new List<Condition>();
			ComponentSeparator = componentSeparator;
			ComparerSeparator = comparerSeparator;
			DateFormat = dateFormat;

			if (componentSeparator == comparerSeparator)
			{
				throw new Exception("Statement separator and field separator cannot be the same.");
			}

			if (string.IsNullOrWhiteSpace(DateFormat))
			{
				throw new Exception("Missing date format.");
			}

			if (!DateFormat.Contains("yyyy") && !DateFormat.Contains("MM") && !DateFormat.Contains("dd"))
			{
				throw new Exception("Invalid date format. It needs to contain at least the following parts: yyyy, MM and dd.");
			}
		}

		/// <summary>
		/// Parse the specified line.
		/// </summary>
		/// <param name="filter">Filter to be parsed.</param>
		/// <returns>True if the parses found no errors, false otherwise.</returns>
		public bool Parse(string filter)
		{
			IsValid = true;
			Filter = filter;
			Conditions = new List<Condition>();
			if (string.IsNullOrWhiteSpace(filter))
			{
				return IsValid;
			}

			var filterParts = SplitLine(filter, ComparerSeparator).ToList();
			filterParts.ForEach(ParseStatement);
			return IsValid;
		}
		#endregion

		#region Private

		/// <summary>
		/// Splits a test into parts, specified by the separator. Takes into account that the separator can be embedded in quotes.
		/// </summary>
		/// <param name="stringToSplit">The string to be split.</param>
		/// <param name="separator">The separator character.</param>
		/// <returns>List of strings.</returns>
		// <remarks>Improved and modernized version of: https://stackoverflow.com/a/31804981 </remarks>
		private static List<string> SplitLine(string stringToSplit, char separator)
		{

			var characters = stringToSplit.ToCharArray();
			var returnValueList = new List<string>();
			var tempString = string.Empty;
			var blockUntilEndQuote = false;
			var blockUntilEndQuote2 = false;
			var characterCount = 0;
			foreach (var character in characters)
			{
				characterCount = characterCount + 1;
				switch (character)
				{
					case '"' when !blockUntilEndQuote2:
						blockUntilEndQuote = blockUntilEndQuote == false;
						break;

					case '\'' when !blockUntilEndQuote:
						blockUntilEndQuote2 = blockUntilEndQuote2 == false;
						break;

				}

				if (character != separator)
				{
					tempString += character;
				}
				else
				{
					if (character == separator && (blockUntilEndQuote || blockUntilEndQuote2))
					{
						tempString += character;
					}
					else
					{
						returnValueList.Add(tempString);
						tempString = string.Empty;
					}
				}

				if (characterCount == characters.Length)
				{
					returnValueList.Add(tempString);
					tempString = string.Empty;
				}
			}

			return returnValueList;
		}

		/// <summary>
		/// Parses a statement into components.
		/// </summary>
		/// <param name="parseValue">Value to parse.</param>
		private void ParseStatement(string parseValue)
		{
			var statement = new Condition();
			// Statement parts are separated by colons.
			var parts = SplitLine(parseValue, ComponentSeparator);
			if (parts.Count != 3)
			{
				statement.Errors.Add(new Error(ErrorType.InvalidSyntax, parseValue));
				Conditions.Add(statement);
				IsValid = false;
				return;
			}

			// The parts are:
			// part[0]: parametername
			// part[1]: operator
			// part[2]: value
			var parameter = parts[0].ToLower().Trim();
			statement.FieldName = parameter;
			if (Fields.All(field => !string.Equals(field.FieldName, parameter, StringComparison.CurrentCultureIgnoreCase)))
			{
				statement.Comparers = Comparer.Unknown;
				statement.Errors.Add(new Error(ErrorType.UnknownField, parseValue));
				Conditions.Add(statement);
				IsValid = false;
				return;
			}

			var fieldDefinition = Fields.Single(field => string.Equals(field.FieldName, parameter, StringComparison.CurrentCultureIgnoreCase));
			// Extract the operator.
			var @operator = ParseOperator(parts[1].ToLower());
			statement.Comparers = @operator;
			if (@operator == Comparer.Unknown)
			{
				statement.Errors.Add(new Error(ErrorType.UnknownOperator, parseValue));
				Conditions.Add(statement);
				IsValid = false;
				return;
			}

			if (fieldDefinition.ComparerDataTypes.All(operatorDataType => operatorDataType.Comparer != @operator))
			{
				statement.Errors.Add(new Error(ErrorType.InvalidOperatorForField, parseValue));
				Conditions.Add(statement);
				IsValid = false;
				return;
			}

			var type = fieldDefinition.ComparerDataTypes.Single(operatorDataType => operatorDataType.Comparer == @operator).DataType;
			// Parse the value.
			var error = ParseValue(parts[2], type, statement);
			if (error != ErrorType.None)
			{
				statement.Errors.Add(new Error(error, parseValue));
				Conditions.Add(statement);
				IsValid = false;
				return;

			}

			Conditions.Add(statement);
		}

		private static bool HasListDuplicates<T>(List<T> list)
		{
			return list.GroupBy(item => item).Where(item => item.Count() > 1).Select(item => item.Key).ToList().Count > 0;
		}


		/// <summary>
		/// Parses a value, based on data type.
		/// </summary>
		/// <param name="value">Value to be parsed.</param>
		/// <param name="dataType">The Digital Delta Data Type for the statement.</param>
		/// <param name="conditions">The processed statement</param>
		/// <returns>Error type</returns>
		/// <remarks>Uses the Newtonsoft JsonConvertor to parse the value.</remarks>
		private ErrorType ParseValue(string value, DataType dataType, Condition conditions)											  
		{
			if (string.IsNullOrWhiteSpace(value))
			{
				return ErrorType.InvalidValue;
			}

			try
			{
				switch (dataType)
				{
					case DataType.StringType:
						conditions.StringValue = JsonConvert.DeserializeObject<string>(value);
						conditions.ParameterDataType = DataType.StringType;
						return ErrorType.None;

					case DataType.DateType:
						conditions.DateTimeValue = JsonConvert.DeserializeObject<DateTime>(value, new IsoDateTimeConverter { DateTimeFormat = DateFormat });
						conditions.ParameterDataType = DataType.DateType;
						return ErrorType.None;

					case DataType.NumericType:
						conditions.NumericValue = JsonConvert.DeserializeObject<decimal>(value);
						conditions.ParameterDataType = DataType.NumericType;
						return ErrorType.None;

					case DataType.ArrayOfStringType:
						conditions.ArrayOfStringValues = JsonConvert.DeserializeObject<List<string>>(value);
						conditions.ParameterDataType = DataType.ArrayOfStringType;
						if (conditions.ArrayOfStringValues.Count == 0)
						{
							return ErrorType.EmptyList;
						}
						if (HasListDuplicates(conditions.ArrayOfStringValues))
						{
							return ErrorType.DuplicatesInList;
						}
						return ErrorType.None;

					case DataType.ArrayOfNumericType:
						conditions.ArrayOfNumericValues = JsonConvert.DeserializeObject<List<decimal>>(value);
						conditions.ParameterDataType = DataType.ArrayOfNumericType;
						if (HasListDuplicates(conditions.ArrayOfNumericValues))
						{
							return ErrorType.DuplicatesInList;
						}
						return ErrorType.None;

					case DataType.BboxType:
						{
							conditions.ArrayOfNumericValues = null;
							conditions.ParameterDataType = DataType.BboxType;
							var result = JsonConvert.DeserializeObject<List<decimal>>(value);
							if (result.Count != 4)
							{
								return ErrorType.InvalidBbox;
							}

							if (result[0].IsBetween(-180, 180) && result[1].IsBetween(-90, 90) && result[2].IsBetween(result[0], 180) && result[3].IsBetween(result[1], 90))
							{
								conditions.ArrayOfNumericValues = result;
								return ErrorType.None;
							}
						}
						return ErrorType.InvalidBbox;

					case DataType.PolygonType:
						{
							conditions.ArrayOfNumericValues = null;
							conditions.ParameterDataType = DataType.PolygonType;
							var result = JsonConvert.DeserializeObject<List<decimal>>(value);
							if (result.Count % 2 != 0)
							{
								return ErrorType.InvalidPolygon;
							}
							for(var i = 0; i < result.Count; i += 2)
							{
								if (!result[i].IsBetween(-180, 180) || !result[i + 1].IsBetween(-90, 90))
								{
									return ErrorType.InvalidPolygon;
								}
							}
							conditions.ArrayOfNumericValues = result;
							return ErrorType.None;
						}

					case DataType.WktType:
						{
							conditions.StringValue = null;
							conditions.ParameterDataType = DataType.WktType;
							try
							{
								Geometry.Deserialize<WktSerializer>(value);
							}
							catch (Exception)
							{
								return ErrorType.InvalidValue;
							}
							conditions.StringValue = value;
							return ErrorType.None;
						}

					default:
						throw new ArgumentOutOfRangeException(nameof(dataType), dataType, null);
				}
			}
			catch { /* JsonConvert exception sink hole. */ }
			return ErrorType.InvalidValue;
		}

		private static Comparer ParseOperator(string @operator)
		{
			switch (@operator)
			{
				case "eq": // Equal to
					return Comparer.Eq;

				case "ne": // Not equal to
					return Comparer.Ne;

				case "lt": // Less than
					return Comparer.Lt;

				case "le": // less than or equal to
					return Comparer.Le;

				case "ge": // Greater than or equal to
					return Comparer.Ge;

				case "gt": // Greater than
					return Comparer.Gt;

				case "in": // Value in list
					return Comparer.In;

				case "notin": // Value NOT in list
					return Comparer.Notin;

				case "like": // String like
					return Comparer.Like;

				case "startswith": // String starts with
					return Comparer.Startswith;

				case "endswith": // String ends with
					return Comparer.Endswith;

				case "inbbox": // Coordinate lies in bounding box
					return Comparer.Inbbox;

				case "inpolygon": // Coordinate lies in polygon
					return Comparer.Inpolygon;

				case "notinbbox": // Coordinate lies in bounding box
					return Comparer.NotInbbox;

				case "notinpolygon": // Coordinate lies in polygon
					return Comparer.NotInpolygon;

				case "wkt": // Coordinate lies in wkt
					return Comparer.Wkt;

				case "all":
					return Comparer.All;

				default: // Unknown operator
					return Comparer.Unknown;

			}
		}
		#endregion
	}
}
