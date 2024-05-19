using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zest.ViewModels.ViewModels
{
    public class CommunityViewModel : CommunityBaseViewModel
    {
        public int Id { get; set; }
        public string Creator { get; set; }
        public bool IsSubscribed { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
