using Captura.Models;
using Captura.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        public TimelineViewModel(int workNumber, string outPath)
        {
            this.WorkNumber = workNumber;
            this.OutPath = outPath;

            //if(System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                for (int i = 1; i <= 10; i++)
                {
                    this.MediaCollection.Add(new MediaItem(this) { FileName = "CapturedFile" + i + ".png", Id = i, MediaType = MediaType.Image, Order = i });
                }
            }

            this.PlayMedia = new DelegateCommand((obj) => {
                var mediaElement = obj as MediaElement;
                if(mediaElement != null)
                {

                }
            });
        }

        public DelegateCommand PlayMedia { get; set; }

        public DelegateCommand StopMedia { get; set; }
    }
}