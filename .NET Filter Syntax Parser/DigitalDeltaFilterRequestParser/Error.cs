namespace DigitalDeltaFilterRequestParser
{
	/// <summary>
	/// An error definition consists of an error and its context (statement)
	/// </summary>
	public class Error
	{
		public ErrorType ErrorType { get; }
		public string Context { get; }

		public Error(ErrorType errorType, string context)
		{
			ErrorType = errorType;
			Context = context;
		}
	}
}