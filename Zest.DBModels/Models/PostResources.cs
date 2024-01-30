using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zest.DBModels.Models
{
	public partial class PostResources
	{
        public int Id { get; set; }
        public int PostId { get; set; }       
        public string ?Type { get; set; }
        public string ?Path { get; set; }
        public string? Name { get; set; }
        public DateTime CreatedOn { get; set; }
		public virtual Post Post { get; set; }
	}
}
