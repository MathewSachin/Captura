using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Captura.Tests
{
    [TestClass]
    public class TestAssemblyManager
    {
        [AssemblyInitialize]
        public static void Init(TestContext Context)
        {
            ServiceProvider.LoadModule(new MoqModule());
        }
    }
}
