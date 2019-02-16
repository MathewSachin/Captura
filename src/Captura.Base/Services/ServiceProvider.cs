using Captura.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Ninject;

namespace Captura
{
    public static class ServiceProvider
    {
        static string _settingsDir;

        public static string SettingsDir
        {
            get
            {
                if (_settingsDir == null)
                    _settingsDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Captura");

                if (!Directory.Exists(_settingsDir))
                    Directory.CreateDirectory(_settingsDir);

                return _settingsDir;
            }
            set
            {
                _settingsDir = value;

                if (!Directory.Exists(_settingsDir))
                    Directory.CreateDirectory(_settingsDir);
            }
        }

        static IKernel Kernel { get; } = new StandardKernel();

        static readonly List<IModule> LoadedModules = new List<IModule>();

        public static void LoadModule(IModule Module)
        {
            Kernel.Load(new Binder(Module));

            LoadedModules.Add(Module);
        }

        /// <summary>
        /// To be called on App Exit
        /// </summary>
        public static void Dispose()
        {
            LoadedModules.ForEach(M => M.Dispose());

            // Singleton objects will be disposed by Kernel
            Kernel.Dispose();
        }

        public static T Get<T>() => Kernel.Get<T>();
        
        public static void LaunchFile(ProcessStartInfo StartInfo)
        {
            try { Process.Start(StartInfo.FileName); }
            catch (Win32Exception e) when (e.NativeErrorCode == 2)
            {
                MessageProvider.ShowError($"Could not find file: {StartInfo.FileName}");
            }
            catch (Exception e)
            {
                MessageProvider.ShowException(e, $"Could not open file: {StartInfo.FileName}");
            }
        }

        public static IMessageProvider MessageProvider => Get<IMessageProvider>();

        public static Version AppVersion => Assembly.GetEntryAssembly()?.GetName().Version;

        public static string AppDir
        {
            get
            {
                var location = Assembly.GetEntryAssembly()?.Location;

                return location == null ? "" : Path.GetDirectoryName(location);
            }
        }

        public static string LibDir => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public static bool FileExists(string FileName)
        {
            return new[] { LibDir, AppDir }
                       .Where(M => M != null)
                       .Any(M => File.Exists(Path.Combine(M, FileName))) || File.Exists(FileName);
        }

        /// <summary>
        /// Binds both as Inteface as Class
        /// </summary>
        public static void BindAsInterfaceAndClass<TInterface, TClass>(this IBinder Binder) where TClass : TInterface
        {
            Binder.BindSingleton<TClass>();

            // ReSharper disable once ConvertClosureToMethodGroup
            Binder.Bind<TInterface>(() => ServiceProvider.Get<TClass>());
        }
    }
}
