﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidDays.Models;

namespace VoidDays.ViewModels.Interfaces
{
    public interface IMainContainerViewModel
    {
        void Initialize();
        User User { get; set; }
    }
}
