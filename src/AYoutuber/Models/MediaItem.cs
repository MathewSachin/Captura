using Captura.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captura.Models
{
    public class MediaItem : ViewModelBase
    {
        #region Properties
        public TimelineViewModel ViewModel { get; private set; }

        private int id;

        public int Id
        {
            get { return id; }
            set
            {
                id = value;
            }
        }

        private string fileName;

        public string FileName
        {
            get { return fileName; }
            set { fileName = value; }
        }

        private MediaType mediaType;

        public MediaType MediaType
        {
            get { return mediaType; }
            set
            {
                mediaType = value;
            }
        }

        private int order;

        public int Order
        {
            get { return order; }
            set
            {
                order = value;
                this.OnPropertyChanged();
            }
        }

        private int interval;

        public int Interval
        {
            get { return interval; }
            set
            {
                if(interval != value)
                {
                    interval = value;
                    this.OnPropertyChanged();
                }                
            }
        }


        public string MediaSource
        {
            get
            {
                return System.IO.Path.Combine(this.ViewModel.OutPath, FileName);
            }
        }

        private string notice = string.Empty;

        public string Notice
        {
            get { return notice; }
            set { notice = value; }
        }

        #endregion

        #region Construction
        public MediaItem(TimelineViewModel timelineViewModel)
        {
            this.ViewModel = timelineViewModel;
        }
        #endregion
    }
}
