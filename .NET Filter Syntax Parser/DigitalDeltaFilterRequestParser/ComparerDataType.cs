namespace DigitalDeltaFilterRequestParser
{
	/// <summary>
	/// Defines an operation/data type combination.
	/// </summary>
	public class ComparerDataType
	{
		public Comparer Comparer { set; get; }
		public DataType DataType { set; get; }

		public ComparerDataType(Comparer comparer, DataType dataType)
		{
			Comparer = comparer;
			DataType = dataType;
		}

	}
}