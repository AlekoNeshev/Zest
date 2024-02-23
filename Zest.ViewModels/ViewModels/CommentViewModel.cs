using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zest.ViewModels.ViewModels
{
    public class CommentViewModel
    {
        public int Id { get; set; }
        public string Publisher { get; set; }
        public string Text { get; set; }
        public int Likes { get; set; }
        public int Dislikes { get; set; }
        public bool IsOwner { get; set; }
        public List<CommentViewModel> Replies { get; set; } = new List<CommentViewModel>();
        public DateTime PostedOn { get; set; }
    }
}
