﻿namespace Captura.Models
{
    public interface IMainWindow
    {
        bool IsVisible { get; set; }

        bool IsMinimized { get; set; }

        void EditImage(string FileName);

        void CropImage(string FileName);

        void TrimMedia(string FileName);
    }
}
