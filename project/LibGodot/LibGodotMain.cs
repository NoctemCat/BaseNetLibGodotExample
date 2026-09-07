/*
MIT No Attribution

Copyright (c) 2026 NoctemCat

Permission is hereby granted, free of charge, to any person obtaining a copy of this
software and associated documentation files (the "Software"), to deal in the Software
without restriction, including without limitation the rights to use, copy, modify,
merge, publish, distribute, sublicense, and/or sell copies of the Software, and to
permit persons to whom the Software is furnished to do so.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A
PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
#if LIBGODOT_ENABLED
#nullable enable

using System;
using System.Collections.Generic;

namespace LibGodot.Bridge
{
    internal static partial class Initializer
    {
        internal unsafe static nint GetPluginInitialize()
        {
#if TOOLS
            // Use builtin dotnet loading for editor.
            return (nint)(void*)null;
#else
            // Use default loader for the export build.
            return (nint)(delegate* unmanaged<IntPtr, IntPtr, IntPtr, int, Godot.NativeInterop.godot_bool>)&global::GodotPlugins.Game.Main.InitializeFromGameProject;
#endif
        }

#if !LIBGODOT_DISABLE_MAIN
        static int Main()
        {
            // Console.WriteLine("LibGodot static main begin");
            List<string> args = [.. Environment.GetCommandLineArgs()];
            args.Insert(1, "--disable-crash-handler");
            // Console.WriteLine($"Environment.CurrentDirectory: {Environment.CurrentDirectory}");
            var instance = LibGodot.CreateGodotInstance(args);
            if (instance is null)
            {
                Console.Error.WriteLine("Error creating Godot instance");
                return 1;
            }

            LibGodot.SetPluginsInitialize(GetPluginInitialize());

            // Console.WriteLine("LibGodot before start");

            instance.Start();

            // Console.WriteLine("LibGodot before first iteration");
            while (!instance.Iteration()) { }

            // Console.WriteLine("LibGodot before destroy");
            instance.Dispose();

            return 0;
        }
#endif
    }
}
#endif
