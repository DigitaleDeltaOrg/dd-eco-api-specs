using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using DigitalDeltaFilterRequestParser;

namespace DigitalDeltaFilterParserTests
{
	[TestFixture]
	class DigitalDeltaFilterParserTests
	{
		const string BBOX_FIELD = "bbox";
		const string STRING_FIELD = "string";
		const string NUMERIC_FIELD = "number";
		const string DATE_FIELD = "date";
		const string POLYGON_FIELD = "polygon";
		const string WKT_FIELD = "wkt";

		[Test]
		public void GenericTests()
		{
			// Setup parser
			var parser = new FilterParser(new List<FieldDefinition>
			{
				new FieldDefinition(BBOX_FIELD, BboxSetup()),
				new FieldDefinition(STRING_FIELD, StringSetup()),
				new FieldDefinition(NUMERIC_FIELD, NumericSetup()),
				new FieldDefinition(DATE_FIELD, DateSetup()),
				new FieldDefinition(POLYGON_FIELD, PolygonSetup()),
			});

			// Incorrect operator
			parser.Parse($@"{BBOX_FIELD}:unknownoperator:""mytestvalue""");
			Assert.That(!parser.IsValid);
			Assert.That(parser.Conditions[0].Errors[0].ErrorType == ErrorType.UnknownOperator);

			// Invalid statement
			parser.Parse(@"unknownfield:eq:""mytestvalue""");
			Assert.That(!parser.IsValid);
			Assert.That(parser.Conditions[0].Errors[0].ErrorType == ErrorType.UnknownField);

			// Invalid statement
			parser.Parse(@"myfield-unknownoperator:""mytestvalue""");
			Assert.That(!parser.IsValid);
			Assert.That(parser.Conditions[0].Errors[0].ErrorType == ErrorType.InvalidSyntax);

			// Correct multi-statement test.
			parser.Parse($@"{NUMERIC_FIELD}:lt:1.234;{STRING_FIELD}:like:""ABCD""");
			Assert.That(parser.IsValid);
			Assert.That(parser.Conditions.Count == 2);
			Assert.That(parser.Conditions.Any());
			Assert.That(parser.Conditions[0].Comparers == Comparer.Lt);
			Assert.That(parser.Conditions[0].NumericValue == (decimal)1.234);
			Assert.That(parser.Conditions[1].Comparers == Comparer.Like);
			Assert.That(parser.Conditions[1].StringValue == "ABCD");
		}

		[Test]
		public void BboxTests()
		{
			// Setup parser
			var parser = new FilterParser(new List<FieldDefinition>
			{
				new FieldDefinition(BBOX_FIELD, BboxSetup())
			});

			// Incorrect value type
			parser.Parse($@"{BBOX_FIELD}:eq:123");
			Assert.That(!parser.IsValid);
			Assert.That(parser.Conditions[0].Errors[0].ErrorType == ErrorType.InvalidOperatorForField);

			// Incorrect data type
			parser.Parse($@"{BBOX_FIELD}:inbbox:""mytestvalue""");
			Assert.That(!parser.IsValid);
			Assert.That(parser.Conditions[0].Errors[0].ErrorType == ErrorType.InvalidValue);

			// Correct area
			parser.Parse($@"{BBOX_FIELD}:inbbox:[1.234,2.345,3.456,4.567]");
			Assert.That(parser.IsValid);
			Assert.That(parser.Conditions.Count == 1);
			Assert.That(parser.Conditions.Any());
			Assert.That(parser.Conditions[0].Comparers == Comparer.Inbbox);
			Assert.That(parser.Conditions[0].FieldName == BBOX_FIELD);
			Assert.That(parser.Conditions[0].ArrayOfNumericValues.Count == 4);

			// Incorrect area (bounds)
			parser.Parse($@"{BBOX_FIELD}:inbbox:[1.234,-1234.345,3.456,4.567]");
			Assert.That(!parser.IsValid);
			Assert.That(parser.Conditions.Count == 1);
			Assert.That(parser.Conditions.Any());
			Assert.That(parser.Conditions[0].Comparers == Comparer.Inbbox);
			Assert.That(parser.Conditions[0].FieldName == BBOX_FIELD);
			Assert.That(parser.Conditions[0].Errors[0].ErrorType == ErrorType.InvalidBbox);

			// Correct area
			parser.Parse($@"{BBOX_FIELD}:notinbbox:[1.234,2.345,3.456,4.567]");
			Assert.That(parser.IsValid);
			Assert.That(parser.Conditions.Count == 1);
			Assert.That(parser.Conditions.Any());
			Assert.That(parser.Conditions[0].Comparers == Comparer.NotInbbox);
			Assert.That(parser.Conditions[0].FieldName == BBOX_FIELD);
			Assert.That(parser.Conditions[0].ArrayOfNumericValues.Count == 4);


		}

		[Test]
		public void PolygonTests()
		{
			// Setup parser
			var parser = new FilterParser(new List<FieldDefinition>
			{
				new FieldDefinition(POLYGON_FIELD, PolygonSetup())
			});

			// Incorrect value type
			parser.Parse($@"{POLYGON_FIELD.ToUpper()}:eq:123");
			Assert.That(!parser.IsValid);
			Assert.That(parser.Conditions[0].Errors[0].ErrorType == ErrorType.InvalidOperatorForField);

			// Incorrect data type
			parser.Parse($@"{POLYGON_FIELD}:Inpolygon:""mytestvalue""");
			Assert.That(!parser.IsValid);
			Assert.That(parser.Conditions[0].Errors[0].ErrorType == ErrorType.InvalidValue);

			// Correct area
			parser.Parse($@"{POLYGON_FIELD}:inpolygon:[1.234,2.345,3.456,4.567]");
			Assert.That(parser.IsValid);
			Assert.That(parser.Conditions.Count == 1);
			Assert.That(parser.Conditions.Any());
			Assert.That(parser.Conditions[0].Comparers == Comparer.Inpolygon);
			Assert.That(parser.Conditions[0].FieldName == POLYGON_FIELD);
			Assert.That(parser.Conditions[0].ArrayOfNumericValues.Count == 4);

			// Incorrect area (bounds)
			parser.Parse($@"{POLYGON_FIELD}:inpolygon:[1.234,-1234.345,3.456,4.567]");
			Assert.That(!parser.IsValid);
			Assert.That(parser.Conditions.Count == 1);
			Assert.That(parser.Conditions.Any());
			Assert.That(parser.Conditions[0].Comparers == Comparer.Inpolygon);
			Assert.That(parser.Conditions[0].FieldName == POLYGON_FIELD);
			Assert.That(parser.Conditions[0].Errors[0].ErrorType == ErrorType.InvalidPolygon);

			// Incorrect area (bounds)
			parser.Parse($@"{POLYGON_FIELD}:notinpolygon:[1.234,-1234.345,3.456,4.567]");
			Assert.That(!parser.IsValid);
			Assert.That(parser.Conditions.Count == 1);
			Assert.That(parser.Conditions.Any());
			Assert.That(parser.Conditions[0].Comparers == Comparer.NotInpolygon);
			Assert.That(parser.Conditions[0].FieldName == POLYGON_FIELD);
			Assert.That(parser.Conditions[0].Errors[0].ErrorType == ErrorType.InvalidPolygon);

		}

		[Test]
		public void WktTests()
		{
			// Setup parser
			var parser = new FilterParser(new List<FieldDefinition>
			{
				new FieldDefinition(WKT_FIELD, WktSetup())
			});

			// Incorrect value type
			parser.Parse($@"{WKT_FIELD}:eq:123");
			Assert.That(!parser.IsValid);
			Assert.That(parser.Conditions[0].Errors[0].ErrorType == ErrorType.InvalidOperatorForField);

			// Incorrect data type
			parser.Parse($@"{WKT_FIELD}:wkt:""mytestvalue""");
			Assert.That(!parser.IsValid);
			Assert.That(parser.Conditions[0].Errors[0].ErrorType == ErrorType.InvalidValue);

			// Correct area
			parser.Parse($@"{WKT_FIELD}:wkt:POINT(1 2)");
			Assert.That(parser.IsValid);
			Assert.That(parser.Conditions.Count == 1);
			Assert.That(parser.Conditions.Any());
			Assert.That(parser.Conditions[0].Comparers == Comparer.Wkt);
			Assert.That(parser.Conditions[0].FieldName == WKT_FIELD);
			Assert.That(parser.Conditions[0].StringValue != null);

			// Correct area
			parser.Parse($@"{WKT_FIELD}:wkt:MULTIPOLYGON (((30 20, 45 40, 10 40, 30 20)),((15 5, 40 10, 10 20, 5 10, 15 5)))");
			Assert.That(parser.IsValid);
			Assert.That(parser.Conditions.Count == 1);
			Assert.That(parser.Conditions.Any());
			Assert.That(parser.Conditions[0].Comparers == Comparer.Wkt);
			Assert.That(parser.Conditions[0].FieldName == WKT_FIELD);
			Assert.That(parser.Conditions[0].StringValue != null);

		}


		[Test]
		public void NumericTests()
		{
			// Setup parser
			var parser = new FilterParser(new List<FieldDefinition>
			{
				new FieldDefinition(NUMERIC_FIELD, NumericSetup())
			});

			// Correct numeric test.
			parser.Parse($@"{NUMERIC_FIELD}:lt:1.234");
			Assert.That(parser.IsValid);
			Assert.That(parser.Conditions.Count == 1);
			Assert.That(parser.Conditions.Any());
			Assert.That(parser.Conditions[0].Comparers == Comparer.Lt);
			Assert.That(parser.Conditions[0].FieldName == NUMERIC_FIELD);
			Assert.That(parser.Conditions[0].NumericValue == (decimal)1.234);

			// Incorrect numeric test.
			parser.Parse($@"{NUMERIC_FIELD}:ge:ABCD");
			Assert.That(!parser.IsValid);
			Assert.That(parser.Conditions.Count == 1);
			Assert.That(parser.Conditions.Any());
			Assert.That(parser.Conditions[0].Comparers == Comparer.Ge);
			Assert.That(parser.Conditions[0].FieldName == NUMERIC_FIELD);
			Assert.That(parser.Conditions[0].Errors[0].ErrorType == ErrorType.InvalidValue);

			// Correct numeric array test.
			parser.Parse($@"{NUMERIC_FIELD}:in:[1,2]");
			Assert.That(parser.IsValid);
			Assert.That(parser.Conditions.Count == 1);
			Assert.That(parser.Conditions.Any());
			Assert.That(parser.Conditions[0].Comparers == Comparer.In);
			Assert.That(parser.Conditions[0].ArrayOfNumericValues.Count == 2 && parser.Conditions[0].ArrayOfNumericValues[0] == 1 && parser.Conditions[0].ArrayOfNumericValues[1] == 2);
		}

		[Test]
		public void StringTests()
		{
			// Setup parser
			var parser = new FilterParser(new List<FieldDefinition>
			{
				new FieldDefinition(STRING_FIELD, StringSetup())
			});

			// Correct string array test with separator contained.
			parser.Parse($@"{STRING_FIELD}:in:[""ABCD""]");
			Assert.That(parser.IsValid);
			Assert.That(parser.Conditions.Count == 1);
			Assert.That(parser.Conditions.Any());
			Assert.That(parser.Conditions[0].Comparers == Comparer.In);
			Assert.That(parser.Conditions[0].ArrayOfStringValues.Count == 1 && parser.Conditions[0].ArrayOfStringValues.First() == "ABCD");

			// Correct string array test with two items. Includes single quote inside a double-quote string.
			parser.Parse($@"{STRING_FIELD}:in:[""AB'CD"",""BCDE""]");
			Assert.That(parser.IsValid);
			Assert.That(parser.Conditions.Count == 1);
			Assert.That(parser.Conditions.Any());
			Assert.That(parser.Conditions[0].Comparers == Comparer.In);
			Assert.That(parser.Conditions[0].ArrayOfStringValues.Count == 2 && parser.Conditions[0].ArrayOfStringValues[0] == @"AB'CD" && parser.Conditions[0].ArrayOfStringValues[1] == "BCDE");

			// Incorrect string array test.
			parser.Parse($@"{STRING_FIELD}:in:1234");
			Assert.That(!parser.IsValid);
			Assert.That(parser.Conditions.Count == 1);
			Assert.That(parser.Conditions.Any());
			Assert.That(parser.Conditions[0].Comparers == Comparer.In);
			Assert.That(parser.Conditions[0].Errors[0].ErrorType == ErrorType.InvalidValue);

			// Incorrect string array test.
			parser.Parse($@"{STRING_FIELD}:in:""234""");
			Assert.That(!parser.IsValid);
			Assert.That(parser.Conditions.Count == 1);
			Assert.That(parser.Conditions.Any());
			Assert.That(parser.Conditions[0].Comparers == Comparer.In);
			Assert.That(parser.Conditions[0].Errors[0].ErrorType == ErrorType.InvalidValue);

			// Correct string array test with separator contained.
			parser.Parse($@"{STRING_FIELD}:in:[""AB:CD"",""BCDE""]");
			Assert.That(parser.IsValid);
			Assert.That(parser.Conditions.Count == 1);
			Assert.That(parser.Conditions.Any());
			Assert.That(parser.Conditions[0].Comparers == Comparer.In);
			Assert.That(parser.Conditions[0].ArrayOfStringValues.Count == 2 && parser.Conditions[0].ArrayOfStringValues[0] == "AB:CD" && parser.Conditions[0].ArrayOfStringValues[1] == "BCDE");

			// Correct string array test with separator contained.
			parser.Parse($@"{STRING_FIELD}:in:[""AB:CD"",""BC;DE""]");
			Assert.That(parser.IsValid);
			Assert.That(parser.Conditions.Count == 1);
			Assert.That(parser.Conditions.Any());
			Assert.That(parser.Conditions[0].Comparers == Comparer.In);
			Assert.That(parser.Conditions[0].ArrayOfStringValues.Count == 2 && parser.Conditions[0].ArrayOfStringValues[0] == "AB:CD" && parser.Conditions[0].ArrayOfStringValues[1] == "BC;DE");
			Assert.That(parser.Conditions[0].ParameterDataType == DataType.ArrayOfStringType);

			// Duplicate test.
			parser.Parse($@"{STRING_FIELD}:in:[""AB:CD"",""AB:CD""]");
			Assert.That(!parser.IsValid);
			Assert.That(parser.Conditions.Count == 1);
			Assert.That(parser.Conditions.Any());
			Assert.That(parser.Conditions[0].Comparers == Comparer.In);
			Assert.That(parser.Conditions[0].ArrayOfStringValues.Count == 2 && parser.Conditions[0].ArrayOfStringValues[0] == "AB:CD" && parser.Conditions[0].ArrayOfStringValues[1] == "AB:CD");
			Assert.That(parser.Conditions[0].ParameterDataType == DataType.ArrayOfStringType);
			Assert.That(parser.Conditions[0].Errors[0].ErrorType == ErrorType.DuplicatesInList);

			// Empty list test.
			parser.Parse($@"{STRING_FIELD}:in:[]");
			Assert.That(!parser.IsValid);
			Assert.That(parser.Conditions.Count == 1);
			Assert.That(parser.Conditions.Any());
			Assert.That(parser.Conditions[0].Comparers == Comparer.In);
			Assert.That(parser.Conditions[0].ArrayOfStringValues.Count == 0);
			Assert.That(parser.Conditions[0].ParameterDataType == DataType.ArrayOfStringType);
			Assert.That(parser.Conditions[0].Errors[0].ErrorType == ErrorType.EmptyList);

			// All-operator test.
			parser.Parse($@"{STRING_FIELD}:all:[""AB:CD"",""CD:EF""]");
			Assert.That(parser.IsValid);
			Assert.That(parser.Conditions.Count == 1);
			Assert.That(parser.Conditions.Any());
			Assert.That(parser.Conditions[0].Comparers == Comparer.All);
			Assert.That(parser.Conditions[0].ArrayOfStringValues.Count == 2 && parser.Conditions[0].ArrayOfStringValues[0] == "AB:CD" && parser.Conditions[0].ArrayOfStringValues[1] == "CD:EF");
			Assert.That(parser.Conditions[0].ParameterDataType == DataType.ArrayOfStringType);


			// All-operator test. Duplicates in list.
			parser.Parse($@"{STRING_FIELD}:all:[""AB:CD"",""AB:CD""]");
			Assert.That(!parser.IsValid);
			Assert.That(parser.Conditions.Count == 1);
			Assert.That(parser.Conditions.Any());
			Assert.That(parser.Conditions[0].Comparers == Comparer.All);
			Assert.That(parser.Conditions[0].ArrayOfStringValues.Count == 2 && parser.Conditions[0].ArrayOfStringValues[0] == "AB:CD" && parser.Conditions[0].ArrayOfStringValues[1] == "AB:CD");
			Assert.That(parser.Conditions[0].ParameterDataType == DataType.ArrayOfStringType);
			Assert.That(parser.Conditions[0].Errors[0].ErrorType == ErrorType.DuplicatesInList);

		}

		[Test]
		public void DateTests()
		{
			// Setup parser
			var parser = new FilterParser(new List<FieldDefinition>
			{
				new FieldDefinition(DATE_FIELD, DateSetup())
			});

			// Correct date test.
			parser.Parse($@"{DATE_FIELD}:eq:""2016-12-31""");
			Assert.That(parser.IsValid);
			Assert.That(parser.Conditions[0].DateTimeValue.Date == new DateTime(2016, 12, 31));

			// Incorrect date test.
			parser.Parse($@"{DATE_FIELD}:eq:""201612-31""");
			Assert.That(!parser.IsValid);
			Assert.That(parser.Conditions.Count == 1);
			Assert.That(parser.Conditions.Any());
			Assert.That(parser.Conditions[0].Comparers == Comparer.Eq);
			Assert.That(parser.Conditions[0].Errors[0].ErrorType == ErrorType.InvalidValue);

			// Incorrect date test.
			parser.Parse($@"{DATE_FIELD}:eq:201612-31");
			Assert.That(!parser.IsValid);
			Assert.That(parser.Conditions.Count == 1);
			Assert.That(parser.Conditions.Any());
			Assert.That(parser.Conditions[0].Comparers == Comparer.Eq);
			Assert.That(parser.Conditions[0].Errors[0].ErrorType == ErrorType.InvalidValue);
		}

		#region Setup helpers. 
		private static List<ComparerDataType> StringSetup()
		{
			var stringType = new List<ComparerDataType>
			{
				new ComparerDataType(Comparer.Eq, DataType.StringType),
				new ComparerDataType(Comparer.Ne, DataType.StringType),
				new ComparerDataType(Comparer.Like, DataType.StringType),
				new ComparerDataType(Comparer.Startswith, DataType.StringType),
				new ComparerDataType(Comparer.Endswith, DataType.StringType),
				new ComparerDataType(Comparer.In, DataType.ArrayOfStringType),
				new ComparerDataType(Comparer.Notin, DataType.ArrayOfStringType),
				new ComparerDataType(Comparer.All, DataType.ArrayOfStringType)

			};
			return stringType;
		}

		private static List<ComparerDataType> WktSetup()
		{
			var stringType = new List<ComparerDataType>
			{
				new ComparerDataType(Comparer.Wkt, DataType.WktType)
			};
			return stringType;
		}

		private static List<ComparerDataType> NumericSetup()
		{
			var numericType = new List<ComparerDataType>
			{
				new ComparerDataType(Comparer.Eq, DataType.NumericType),
				new ComparerDataType(Comparer.Ne, DataType.NumericType),
				new ComparerDataType(Comparer.Lt, DataType.NumericType),
				new ComparerDataType(Comparer.Le, DataType.NumericType),
				new ComparerDataType(Comparer.Gt, DataType.NumericType),
				new ComparerDataType(Comparer.Ge, DataType.NumericType),
				new ComparerDataType(Comparer.In, DataType.ArrayOfNumericType),
				new ComparerDataType(Comparer.Notin, DataType.ArrayOfNumericType)
			};
			return numericType;
		}

		private static List<ComparerDataType> BboxSetup()
		{
			var numericType = new List<ComparerDataType>
			{
				new ComparerDataType(Comparer.Inbbox, DataType.BboxType),
				new ComparerDataType(Comparer.NotInbbox, DataType.BboxType),
			};
			return numericType;
		}

		private static List<ComparerDataType> PolygonSetup()
		{
			var numericType = new List<ComparerDataType>
			{
				new ComparerDataType(Comparer.Inpolygon, DataType.PolygonType),
				new ComparerDataType(Comparer.NotInpolygon, DataType.PolygonType),
			};
			return numericType;
		}

		private static List<ComparerDataType> DateSetup()
		{
			var dateType = new List<ComparerDataType>
			{
				new ComparerDataType(Comparer.Eq, DataType.DateType),
				new ComparerDataType(Comparer.Ne, DataType.DateType),
				new ComparerDataType(Comparer.Lt, DataType.DateType),
				new ComparerDataType(Comparer.Le, DataType.DateType),
				new ComparerDataType(Comparer.Gt, DataType.DateType),
				new ComparerDataType(Comparer.Ge, DataType.DateType)
			};
			return dateType;
		}
		#endregion

	}
}
