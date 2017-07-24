using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captura.Models.VideoItems
{
    /// <summary>
    /// 영역 캡춰시 사이즈
    /// </summary>
    public enum RegionSize
    {
        /// <summary>
        /// 4:3
        /// </summary>
        XVGA_640_480,
        /// <summary>
        /// 16:9
        /// </summary>
        WVGA_854_480,
        /// <summary>
        /// 4:3
        /// </summary>
        XVGA_800_600,
        /// <summary>
        /// Youtube screen size
        /// </summary>
        YOUTUBE_940_530,
        /// <summary>
        /// 4:3
        /// </summary>
        XGA_1024_768,
        /// <summary>
        /// 16:9
        /// </summary>
        HD720_1280_720,
        /// <summary>
        /// 5:4 표준
        /// </summary>
        SXGA_1280_1024,
        /// <summary>
        /// 16:9 표준
        /// </summary>
        HD1080_1920_1080,
        /// <summary>
        /// 16:10 표준
        /// </summary>
        WUXGA_1920_1200,
        Custom
    }
}
