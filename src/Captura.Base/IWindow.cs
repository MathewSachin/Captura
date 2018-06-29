﻿using System;
using System.Drawing;

namespace Captura.Models
{
    public interface IWindow
    {
        bool IsAlive { get; }

        bool IsVisible { get; }

        IntPtr Handle { get; }

        string Title { get; }

        Rectangle Rectangle { get; }
    }
}