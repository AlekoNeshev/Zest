using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zest.Services.ActionResult
{
	public class CustomFileStreamResult
	{
		

		public CustomFileStreamResult(Stream stream, string contentType)
		{
			this.stream = stream;
			this.contentType = contentType;
		}

		private Stream stream;
		private string contentType;

		public string ContentType
		{
			get { return contentType; }
			set { contentType = value; }
		}

		public Stream Stream
		{
			get { return stream; }
			set { stream = value; }
		}

	}

}
