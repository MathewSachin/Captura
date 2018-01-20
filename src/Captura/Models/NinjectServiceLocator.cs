using Captura.ViewModels;
using Ninject;

namespace Captura
{
    public class NinjectServiceLocator
    {
        public static IKernel Kernel { get; } = new StandardKernel(new MainModule(), new CoreModule());
        
        public MainViewModel MainViewModel => Kernel.Get<MainViewModel>();
    }
}