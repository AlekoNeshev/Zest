﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zest.ViewModels.ViewModels
{
    public class PostViewModel : PostBaseViewModel
    {
        public int Id { get; set; }
        public string Publisher { get; set; }
        public string CommunityName { get; set; }
        public int CommunityId { get; set; }
        public bool IsOwner { get; set; }
        public int Likes { get; set; }
        public int Dislikes { get; set; }
        public string ?ResourceType { get; set; } 
        public LikeViewModel? Like { get; set; }
        public DateTime PostedOn { get; set; }
    }
}
