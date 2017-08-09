using Captura.Models;
using Captura.Models.VideoItems;
using Captura.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Captura
{
    public class TimelineViewModel : ViewModelBase
    {
        #region Properties
        /// <summary>
        /// 현재 작업 번호
        /// </summary>
        public int WorkNumber { get; private set; } = 1;

        /// <summary>
        /// 캡춰 경로
        /// </summary>
        public string OutPath { get; private set; } = string.Empty;

        public ObservableCollection<MediaItem> MediaCollection { get; set; } = new ObservableCollection<MediaItem>();
        #endregion

        #region Commands
        #endregion

        //public TimelineViewModel()
        //    : this(1, "")
        //{

        //}

        public TimelineViewModel(int workNumber, string outPath)
        {
            this.WorkNumber = workNumber;
            this.OutPath = outPath;

            

            //if(System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                int fileIndex = 0;
                foreach(var filePath in Directory.GetFiles(outPath))
                {
                    var fileInfo = new FileInfo(filePath);

                    switch (fileInfo.Extension.ToLower())
                    {
                        case ".png":
                        case ".gif":
                            fileIndex++;
                            this.MediaCollection.Add(new MediaItem(this) { FileName = fileInfo.Name, Id = fileIndex, MediaType = MediaType.Image, Order = fileIndex, Interval = 5 });
                            break;
                        case ".mp4":
                            fileIndex++;
                            this.MediaCollection.Add(new MediaItem(this) { FileName = fileInfo.Name, Id = fileIndex, MediaType = MediaType.Video, Order = fileIndex, Interval = 5 });
                            break;
                    }
                }
            }

            this.PlayMedia = new DelegateCommand((obj) => {
                var mediaElement = obj as MediaElement;
                if(mediaElement != null)
                {

                }
            });

            this.MediaTargetSizes = new ObservableCollection<string>(Enum.GetNames(typeof(RegionSize)));
        }

        public DelegateCommand PlayMedia { get; set; }

        public DelegateCommand StopMedia { get; set; }

        public ObservableCollection<string> MediaTargetSizes { get; set; }

        public string MediaTargetSizeSelectedItem { get; set; } = "YOUTUBE_940_530";
    }
}