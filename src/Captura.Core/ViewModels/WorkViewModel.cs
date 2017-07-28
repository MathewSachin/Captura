using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captura.ViewModels
{
    public class WorkViewModel : ViewModelBase
    {
        #region Properties
        public MainViewModel MainViewModel { get; private set; }
        #endregion

        #region Commands
        /// <summary>
        /// New Work Command
        /// </summary>
        public DelegateCommand NewWorkCommand { get; set; }

        /// <summary>
        /// Edit Work Command
        /// </summary>
        public DelegateCommand EditWorkCommand { get; set; }
        #endregion

        public WorkViewModel(MainViewModel mainViewModel)
        {
            this.MainViewModel = mainViewModel;
        }
    }
}
