using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zest.ViewModels.ViewModels
{
	public class MessageViewModel
	{
        public string SenderUsername { get; set; }
        public string Text { get; set; }
		public DateTime CreatedOn { get; set; }
	}
}
