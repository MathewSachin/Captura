﻿using Captura.Models;
using Ninject.Modules;

namespace Captura
{
    public class FakesModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IMessageProvider>().To<FakeMessageProvider>().InSingletonScope();
            Bind<IRegionProvider>().ToConstant(FakeRegionProvider.Instance);
            Bind<ISystemTray>().To<FakeSystemTray>().InSingletonScope();
            Bind<IMainWindow>().To<FakeWindowProvider>().InSingletonScope();
            Bind<IPreviewWindow>().To<FakePreviewWindow>().InSingletonScope();
            Bind<IVideoSourcePicker>().To<FakeVideoSourcePicker>().InSingletonScope();
        }
    }
}