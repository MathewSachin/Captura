using System;
using System.Drawing;
using System.Windows.Input;
using Captura.Models;
using Reactive.Bindings;

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

        const int MinWidth = 10,
            MinHeight = 10,
            KeyMoveDelta = 1;

        public const int BorderSize = 3;

        readonly IVideoSourcePicker _videoSourcePicker;

        public RegionSelectorViewModel(IVideoSourcePicker VideoSourcePicker)
        {
            _videoSourcePicker = VideoSourcePicker;

            MoveLeftCommand = new ReactiveCommand()
                .WithSubscribe(() => Left -= KeyMoveDelta);
            MoveRightCommand = new ReactiveCommand()
                .WithSubscribe(() => Left += KeyMoveDelta);
            MoveUpCommand = new ReactiveCommand()
                .WithSubscribe(() => Top -= KeyMoveDelta);
            MoveDownCommand = new ReactiveCommand()
                .WithSubscribe(() => Top += KeyMoveDelta);

            IncreaseWidthCommand = new ReactiveCommand()
                .WithSubscribe(() => Width += KeyMoveDelta);
            DecreaseWidthCommand = new ReactiveCommand()
                .WithSubscribe(() => Width -= KeyMoveDelta);
            IncreaseHeightCommand = new ReactiveCommand()
                .WithSubscribe(() => Height += KeyMoveDelta);
            DecreaseHeightCommand = new ReactiveCommand()
                .WithSubscribe(() => Height -= KeyMoveDelta);
        }

        public void SnapToRegion()
        {
            var region = _videoSourcePicker.PickRegion();

            if (region is Rectangle rect)
            {
                SelectedRegion = rect;
            }
        }

        public int Left
        {
            get => _left;
            set
            {
                Set(ref _left, value);
                RaisePropertyChanged(nameof(LeftDip));
            }
        }

        public double LeftDip
        {
            get => Left / Dpi.X - BorderSize;
            set => Left = (int)Math.Round((value + BorderSize) * Dpi.X);
        }

        public int Top
        {
            get => _top;
            set
            {
                Set(ref _top, value);
                RaisePropertyChanged(nameof(TopDip));
            }
        }

        public double TopDip
        {
            get => Top / Dpi.Y - BorderSize;
            set => Top = (int)Math.Round((value + BorderSize) * Dpi.Y);
        }

        public int Width
        {
            get => _width;
            set
            {
                Set(ref _width, Math.Max(value, MinWidth));
                RaisePropertyChanged(nameof(WidthDip));
            }
        }

        public double WidthDip
        {
            get => Width / Dpi.X + 2 * BorderSize;
            set => Width = (int)Math.Round((value - 2 * BorderSize) * Dpi.X);
        }

        public int Height
        {
            get => _height;
            set
            {
                Set(ref _height, Math.Max(value, MinHeight));
                RaisePropertyChanged(nameof(HeightDip));
            }
        }

        public double HeightDip
        {
            get => Height / Dpi.Y + 2 * BorderSize;
            set => Height = (int)Math.Round((value - 2 * BorderSize) * Dpi.Y);
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

        public void ResizeFromTop(double VerticalChangeDip)
        {
            var verticalChange = (int) (VerticalChangeDip * Dpi.Y);

            var oldBottom = Top + Height;
            var top = Top + verticalChange;
            
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

        public void ResizeFromLeft(double HorizontalChangeDip)
        {
            var horizontalChange = (int) (HorizontalChangeDip * Dpi.X);

            var oldRight = Left + Width;
            var left = Left + horizontalChange;

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

        public ICommand MoveLeftCommand { get; }
        public ICommand MoveRightCommand { get; }
        public ICommand MoveUpCommand { get; }
        public ICommand MoveDownCommand { get; }

        public ICommand IncreaseWidthCommand { get; }
        public ICommand DecreaseWidthCommand { get; }
        public ICommand IncreaseHeightCommand { get; }
        public ICommand DecreaseHeightCommand { get; }
    }
}