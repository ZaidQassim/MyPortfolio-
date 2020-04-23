﻿using System;

namespace Core.Entities
{
    public class PortfolioItem : EntityBase
    {
        public string ProjectName { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string UrlView { get; set; }
        public string UrlGitHub { get; set; }
        public DateTime DateAdded { get; set; }
    }
}
