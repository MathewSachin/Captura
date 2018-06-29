﻿using System.Drawing;
using Captura;

namespace Screna
{
    public abstract class EditorBase : IBitmapEditor
    {
        public Graphics Graphics { get; }

        protected EditorBase(Graphics Graphics)
        {
            this.Graphics = Graphics;
        }

        public abstract void Dispose();
    }
}