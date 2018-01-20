using Captura.ViewModels;
using Ninject;

namespace Captura
{
    public class NinjectServiceLocator
    {
        readonly IKernel _kernel;

        public NinjectServiceLocator()
        {
            _kernel = new StandardKernel(new MainModule(), new CoreModule());
        }

        public MainViewModel MainViewModel => _kernel.Get<MainViewModel>();
    }
}