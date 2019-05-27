﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Mobile.Models
{
    public enum MenuItemType
    {
        Browse,
        About,
        CurrentDay
    }
    public class HomeMenuItem
    {
        public MenuItemType Id { get; set; }

        public string Title { get; set; }
    }
}
