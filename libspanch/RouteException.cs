using System;
using System.Runtime.Serialization;

namespace libspanch
{
	[Serializable]
	internal class RouteException : Exception
	{
		public RouteException()
		{
		}

		public RouteException(string message) : base(message)
		{
		}

		public RouteException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected RouteException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}