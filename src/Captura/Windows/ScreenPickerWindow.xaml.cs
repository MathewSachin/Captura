using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Captura.Video;
using Captura.ViewModels;
using Reactive.Bindings;

namespace Captura
{
    public partial class ScreenPickerWindow
    {
        const double Scale = 0.15;

        public ObservableCollection<ScreenPickerViewModel> ScreenPickerViewModels { get; } = new ObservableCollection<ScreenPickerViewModel>();

        public ICommand SelectScreenCommand { get; }

        ScreenPickerWindow()
        {
            SelectScreenCommand = new ReactiveCommand<IScreen>()
                .WithSubscribe(M =>
                {
                    SelectedScreen = M;

                    Close();
                });

            InitializeComponent();

            ScreenContainer.Width = SystemParameters.VirtualScreenWidth * Scale;
            ScreenContainer.Height = SystemParameters.VirtualScreenHeight * Scale;

            var platformServices = ServiceProvider.Get<IPlatformServices>();

            var screens = platformServices.EnumerateScreens().ToArray();

            foreach (var screen in screens)
            {
                ScreenPickerViewModels.Add(new ScreenPickerViewModel(screen, Scale));
            }
        }

        public IScreen SelectedScreen { get; private set; }

        void CloseClick(object Sender, RoutedEventArgs E)
        {
            SelectedScreen = null;

            Close();
        }

        public static IScreen PickScreen()
        {
            var picker = new ScreenPickerWindow
            {
                Owner = MainWindow.Instance
            };

            picker.ShowDialog();

            return picker.SelectedScreen;
        }
    }
}
