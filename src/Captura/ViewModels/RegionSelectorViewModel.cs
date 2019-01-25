using System;
using System.Drawing;
// ReSharper disable MemberCanBePrivate.Global

namespace Captura
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class RegionSelectorViewModel : NotifyPropertyChanged
    {
        int _left = 50,
            _top = 50,
            _width = 500,
            _height = 500;

        const int MinWidth = 300,
            MinHeight = 300,
            BorderSize = 3;

        public int Left
        {
            get => _left;
            set
            {
                _left = value;

                OnPropertyChanged();
                RaisePropertyChanged(nameof(LeftDip));
                RaiseUpdateRegionName();
            }
        }

        public double LeftDip
        {
            get => Left / Dpi.X - BorderSize;
            set => Left = (int)((value + BorderSize) * Dpi.X);
        }

        public int Top
        {
            get => _top;
            set
            {
                _top = value;

                OnPropertyChanged();
                RaisePropertyChanged(nameof(TopDip));
                RaiseUpdateRegionName();
            }
        }

        public double TopDip
        {
            get => Top / Dpi.Y - BorderSize;
            set => Top = (int)((value + BorderSize) * Dpi.Y);
        }

        public int Width
        {
            get => _width;
            set
            {
                _width = Math.Max(value, MinWidth);

                OnPropertyChanged();
                RaisePropertyChanged(nameof(WidthDip));
                RaiseUpdateRegionName();
            }
        }

        public double WidthDip
        {
            get => Width / Dpi.X + 2 * BorderSize;
            set => Width = (int)((value - 2 * BorderSize) * Dpi.X);
        }

        public int Height
        {
            get => _height;
            set
            {
                _height = Math.Max(value, MinHeight);

                OnPropertyChanged();
                RaisePropertyChanged(nameof(HeightDip));
                RaiseUpdateRegionName();
            }
        }

        public double HeightDip
        {
            get => Height / Dpi.Y + 2 * BorderSize;
            set => Height = (int)((value - 2 * BorderSize) * Dpi.Y);
        }

        public Rectangle SelectedRegion
        {
            get => new Rectangle(Left, Top, Width, Height);
            set
            {
                Left = value.Left;
                Top = value.Top;
                Width = value.Width;
                Height = value.Height;
            }
        }

        void RaiseUpdateRegionName()
        {
            var region = SelectedRegion;

            UpdateRegionName?.Invoke(region.ToString()
                .Replace("{", "")
                .Replace("}", "")
                .Replace(",", ", "));
        }

        public event Action<string> UpdateRegionName;

        public void ResizeFromTop(double VerticalChangeDip)
        {
            var verticalChange = (int) (VerticalChangeDip * Dpi.Y);

            var oldBottom = Top + Height;
            var top = Top + verticalChange;
            var oldLeft = Left;

            try
            {
                if (top <= 0)
                {
                    Top = 0;
                    Height = oldBottom;
                    return;
                }

                var height = Height - verticalChange;

                if (height > MinHeight)
                {
                    Top = top;
                    Height = height;
                }
                else
                {
                    Height = MinHeight;
                    Top = oldBottom - MinHeight;
                }
            }
            finally
            {
                // workaround to prevent horizontal movement
                Left = oldLeft;
            }
        }

        public void ResizeFromLeft(double HorizontalChangeDip)
        {
            var horizontalChange = (int) (HorizontalChangeDip * Dpi.X);

            var oldRight = Left + Width;
            var left = Left + horizontalChange;
            var oldTop = Top;

            try
            {
                if (left <= 0)
                {
                    Left = 0;
                    Width = oldRight;
                    return;
                }

                var width = Width - horizontalChange;

                if (width > MinWidth)
                {
                    Left = left;
                    Width = width;
                }
                else
                {
                    Width = MinWidth;
                    Left = oldRight - MinWidth;
                }
            }
            finally
            {
                // workaround to prevent vertical movement
                Top = oldTop;
            }
        }
    }
}