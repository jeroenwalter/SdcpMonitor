using System.Reflection;
using System.Runtime.Versioning;
using System.Windows;
using SdcpMonitor;

[assembly: AssemblyTitle(App.ApplicationName)]
[assembly: AssemblyDescription("Monitor/control 3D printers that support the SDCP protocol")]
[assembly: AssemblyCopyright("Copyright (c) 2024 Jeroen Walter")]
[assembly: AssemblyFileVersion(App.FileVersion)]
[assembly: AssemblyVersion(App.FileVersion)]
[assembly: AssemblyProduct(App.ApplicationName)]
[assembly: AssemblyInformationalVersion(App.ApplicationVersion)]

[assembly: ThemeInfo(
    ResourceDictionaryLocation.None,            //where theme specific resource dictionaries are located
                                                //(used if a resource is not found in the page,
                                                // or application resource dictionaries)
    ResourceDictionaryLocation.SourceAssembly   //where the generic resource dictionary is located
                                                //(used if a resource is not found in the page,
                                                // app, or any theme specific resource dictionaries)
)]
