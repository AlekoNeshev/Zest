using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zest.ViewModels.ViewModels
{
	public class LikeViewModel
	{
		public int Id { get; set; }

		public bool Value { get; set; }

		public string AccountId { get; set; }

		public int? PostId { get; set; }

		public int? CommentId { get; set; }
	}
}
