﻿using System;
using System.Runtime.InteropServices;
using Captura;
using SharpDX.Direct3D11;
using Device = SharpDX.Direct3D11.Device;
using MapFlags = SharpDX.Direct3D11.MapFlags;

namespace DesktopDuplication
{
    public class Texture2DFrame : IBitmapFrame
    {
        public Texture2D Texture { get; }
        public Texture2D PreviewTexture { get; }

        public Device Device { get; }

        public Texture2DFrame(Texture2D Texture, Device Device, Texture2D PreviewTexture)
        {
            this.Texture = Texture;
            this.Device = Device;
            this.PreviewTexture = PreviewTexture;

            var desc = Texture.Description;

            Width = desc.Width;
            Height = desc.Height;
        }

        public void Dispose() { }

        public int Width { get; }
        public int Height { get; }

        public void CopyTo(byte[] Buffer, int Length)
        {
            var mapSource = Device.ImmediateContext.MapSubresource(Texture, 0, MapMode.Read, MapFlags.None);

            try
            {
                Marshal.Copy(mapSource.DataPointer, Buffer, 0, Length);
            }
            finally
            {
                Device.ImmediateContext.UnmapSubresource(Texture, 0);
            }
        }

        public void CopyTo(IntPtr Buffer, int Length)
        {
            var mapSource = Device.ImmediateContext.MapSubresource(Texture, 0, MapMode.Read, MapFlags.None);

            try
            {
                Kernel32.CopyMemory(Buffer, mapSource.DataPointer, (uint)Length);
            }
            finally
            {
                Device.ImmediateContext.UnmapSubresource(Texture, 0);
            }
        }
    }
}