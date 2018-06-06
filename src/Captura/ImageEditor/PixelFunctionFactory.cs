namespace Captura
{
    public static class PixelFunctionFactory
    {
        static void Negative(ref byte Red, ref byte Green, ref byte Blue)
        {
            Red = (byte)(255 - Red);
            Green = (byte)(255 - Green);
            Blue = (byte)(255 - Blue);
        }

        static void Green(ref byte Red, ref byte Green, ref byte Blue)
        {
            Red = Blue = 0;
        }

        static void Red(ref byte Red, ref byte Green, ref byte Blue)
        {
            Green = Blue = 0;
        }

        static void Blue(ref byte Red, ref byte Green, ref byte Blue)
        {
            Red = Green = 0;
        }

        static void Grayscale(ref byte Red, ref byte Green, ref byte Blue)
        {
            var pixel = 0.299 * Red + 0.587 * Green + 0.114 * Blue;

            if (pixel > 255)
                pixel = 255;

            Red = Green = Blue = (byte)pixel;
        }

        static void Sepia(ref byte Red, ref byte Green, ref byte Blue)
        {
            var newRed = 0.393 * Red + 0.769 * Green + 0.189 * Blue;
            var newGreen = 0.349 * Red + 0.686 * Green + 0.168 * Blue;
            var newBlue = 0.272 * Red + 0.534 * Green + 0.131 * Blue;

            // Red
            Red = (byte)(newRed > 255 ? 255 : newRed);

            // Green
            Green = (byte)(newGreen > 255 ? 255 : newGreen);

            // Blue
            Blue = (byte)(newBlue > 255 ? 255 : newBlue);
        }

        public static ModifyPixel GetEffectFunction(ImageEffect Effect)
        {
            switch (Effect)
            {
                case ImageEffect.Negative:
                    return Negative;

                case ImageEffect.Blue:
                    return Blue;

                case ImageEffect.Green:
                    return Green;

                case ImageEffect.Red:
                    return Red;

                case ImageEffect.Sepia:
                    return Sepia;

                case ImageEffect.Grayscale:
                    return Grayscale;

                default:
                    return null;
            }
        }
    }
}