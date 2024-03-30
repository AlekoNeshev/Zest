using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zest.ViewModels.ViewModels
{
    public class AccountViewModel : BaseAccountViewModel
    {     
        public string Email { get; set; }
        public bool IsAdmin { get; set; }
    }
}
