#pragma warning disable
using System;
using System.IO;
using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using static UrlDir;

namespace AlphaPatch
{
    public class AlphaPatch
    {

        public static void Main()
        {
            var harmony = new Harmony("info.thepuzzlemaker.alphapatch");
            
            Console.WriteLine("=== AlphaPatch Initialized ===");
            Console.WriteLine("Patching UrlDir constructor...");
            var urlDirCtor = AccessTools.Constructor(typeof(UrlDir), new Type[] { typeof(ConfigDirectory[]), typeof(ConfigFileType[]) });
            MethodInfo urlDirCtorPrefix = AccessTools.Method(typeof(UrlDirPatch), nameof(UrlDirPatch.Patch01));
            harmony.Patch(urlDirCtor, new HarmonyMethod(urlDirCtorPrefix));
            
            Console.WriteLine("Patching UrlDir.Create...");
            var urlDirCreate = AccessTools.Method(typeof(UrlDir), "Create", new Type[] { typeof(UrlDir), typeof(DirectoryInfo) });
            MethodInfo urlDirCreatePrefix = AccessTools.Method(typeof(UrlDirPatch), nameof(UrlDirPatch.Patch02));
            harmony.Patch(urlDirCreate, new HarmonyMethod(urlDirCreatePrefix));
            
            Console.WriteLine("Patching UrlDir.CreateDirectory...");
            var urlDirCreateDirectory = AccessTools.Method(typeof(UrlDir), "CreateDirectory", new Type[] { typeof(UrlIdentifier), typeof(int) });
            MethodInfo urlDirCreateDirectoryPrefix = AccessTools.Method(typeof(UrlDirPatch), nameof(UrlDirPatch.Patch03));
            harmony.Patch(urlDirCreateDirectory, new HarmonyMethod(urlDirCreateDirectoryPrefix));
            
            Console.WriteLine("Patching UrlDir.GetConfig...");
            var urlDirGetConfig = AccessTools.Method(typeof(UrlDir), "GetConfig", new Type[] { typeof(UrlIdentifier), typeof(int) });
            MethodInfo urlDirGetConfigPrefix = AccessTools.Method(typeof(UrlDirPatch), nameof(UrlDirPatch.Patch04));
            harmony.Patch(urlDirGetConfig, new HarmonyMethod(urlDirGetConfigPrefix));
        }
    }

    class UrlDirPatch
    {
        static AccessTools.FieldRef<UrlDir, List<UrlFile>> _files = AccessTools.FieldRefAccess<UrlDir, List<UrlFile>>("_files");
        static AccessTools.FieldRef<UrlDir, List<UrlDir>> _children = AccessTools.FieldRefAccess<UrlDir, List<UrlDir>>("_children");
        static AccessTools.FieldRef<UrlDir, string> _path = AccessTools.FieldRefAccess<UrlDir, string>("_path");
        static AccessTools.FieldRef<UrlDir, UrlDir> _root = AccessTools.FieldRefAccess<UrlDir, UrlDir>("_root");
        static AccessTools.FieldRef<UrlDir, UrlDir> _parent = AccessTools.FieldRefAccess<UrlDir, UrlDir>("_parent");
        static AccessTools.FieldRef<UrlDir, string> _name = AccessTools.FieldRefAccess<UrlDir, string>("_name");

        public static bool Patch01(UrlDir __instance, ref ConfigDirectory[] dirConfig, ConfigFileType[] fileConfig)
        {
            _files(__instance) = new List<UrlFile>();
            _children(__instance) = new List<UrlDir>();
            _parent(__instance) = null;
            _root(__instance) = __instance;
            _name(__instance) = "root";
            List<ConfigDirectory> dirConfig1 = new List<ConfigDirectory>(dirConfig);
            dirConfig1.Sort(delegate (ConfigDirectory a, ConfigDirectory b) {
                return a.directory.CompareTo(b.directory);
            });
            int i = 0;
            int num = dirConfig1.Count;
            while (i < num)
            {
                var inst = (UrlDir)Activator.CreateInstance(typeof(UrlDir), BindingFlags.Instance | BindingFlags.NonPublic, null, new object[] { __instance, dirConfig1[i] }, null, null);
                _children(__instance).Add(inst);
                i++;
            }
            foreach (UrlFile urlFile in __instance.AllFiles)
            {
                urlFile.ConfigureFile(fileConfig);
            }
            _children(__instance).Sort(delegate (UrlDir a, UrlDir b)
            {
                return a.name.CompareTo(b.name);
            });
            return false; // Skip the original method.
        }

        public static bool Patch02(UrlDir __instance, UrlDir parent, DirectoryInfo info)
        {
            _path(__instance) = info.FullName;
            _parent(__instance) = parent;
            _root(__instance) = parent.root;
            _files(__instance) = new List<UrlFile>();
            _children(__instance) = new List<UrlDir>();
            FileInfo[] files0 = info.GetFiles();
            List<FileInfo> files = new List<FileInfo>(files0);
            files.Sort(delegate (FileInfo a, FileInfo b) {
                return a.Name.CompareTo(b.Name);
            });
            int i = 0;
            int num = files.Count;
            while (i < num)
            {
                _files(__instance).Add(new UrlFile(__instance, files[i]));
                i++;
            }
            DirectoryInfo[] directories0 = info.GetDirectories();
            List<DirectoryInfo> directories = new List<DirectoryInfo>(directories0);
            directories.Sort(delegate (DirectoryInfo a, DirectoryInfo b) {
                return a.Name.CompareTo(b.Name);
            });
            int j = 0;
            int num2 = directories.Count;
            while (j < num2)
            {
                DirectoryInfo directoryInfo = directories[j];
                if (directoryInfo.Name != ".svn" && directoryInfo.Name != "PluginData" && directoryInfo.Name != "zDeprecated")
                {
                    var inst = (UrlDir)Activator.CreateInstance(typeof(UrlDir), BindingFlags.Instance | BindingFlags.NonPublic, null, new object[] { __instance, directoryInfo }, null, null);
                    _children(__instance).Add(inst);
                }
                j++;
            }
            _children(__instance).Sort(delegate (UrlDir a, UrlDir b)
            {
                return a.name.CompareTo(b.name);
            });
            return false; // Skip the original method
        }

        public static bool Patch03(UrlDir __instance, UrlIdentifier id, int index, ref UrlDir __result)
        {
            if (index == id.urlDepth)
            {
                int i = 0;
                int count = _children(__instance).Count;
                while (i < count)
                {
                    if (id[index] == _children(__instance)[i].name)
                    {
                        __result = _children(__instance)[i];
                        return false; // Skip the original method.
                    }
                    i++;
                }
                DirectoryInfo directoryInfo = Directory.CreateDirectory(Path.Combine(_path(__instance), id[index]));
                var urlDir = (UrlDir)Activator.CreateInstance(typeof(UrlDir), BindingFlags.Instance | BindingFlags.NonPublic, null, new object[] { __instance, directoryInfo }, null, null);
                _children(__instance).Add(urlDir);
                _children(__instance).Sort(delegate (UrlDir a, UrlDir b) {
                    return _name(a).CompareTo(_name(b));
                });
                __result = urlDir;
                return false; // Skip the original method.
            }
            int j = 0;
            int count2 = _children(__instance).Count;
            while (j < count2)
            {
                if (id[index] == _children(__instance)[j].name)
                {
                    var dir = _children(__instance)[j];
                    var createDirectory = typeof(UrlDir).GetMethod("CreateDirectory", BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[] { typeof(UrlIdentifier), typeof(int) }, null);
                    __result = (UrlDir)createDirectory.Invoke(dir, new object[] { id, index + 1 });
                    return false; // Skip the original method.
                }
                j++;
            }
            __result = null;
            return false; // Skip the original method.
        }

        public static bool Patch04(UrlDir __instance, UrlIdentifier id, int index, ref UrlConfig __result)
        {
            if (index == id.urlDepth)
            {
                int i = 0;
                int count = __instance.files.Count;
                while (i < count)
                {
                    UrlFile urlFile = __instance.files[i];
                    if (urlFile.fileType == FileType.Config && urlFile.ContainsConfig(id[index]))
                    {
                        __result = urlFile.GetConfig(id[index]);
                        return false; // Skip the original method.
                    }
                    i++;
                }
            }
            else
            {
                int j = 0;
                int count2 = __instance.children.Count;
                while (j < count2)
                {
                    var getConfig = typeof(UrlDir).GetMethod("GetConfig", BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[] { typeof(UrlIdentifier), typeof(int) }, null);
                    UrlDir urlDir = __instance.children[j];
                    if (urlDir.name != string.Empty && id[index] == urlDir.name)
                    {

                        __result = (UrlConfig)getConfig.Invoke(urlDir, new object[] { id, index + 1 });
                        return false; // Skip the original method.
                    }
                    else
                    {
                        UrlConfig config = (UrlConfig)getConfig.Invoke(urlDir, new object[] { id, index });
                        if (config != null)
                        {
                            __result = config;
                            return false; // Skip the original method.
                        }
                    }
                    j++;
                }

                int k = 0;
                int count3 = __instance.files.Count;
                while (k < count3)
                {
                    UrlFile urlFile2 = __instance.files[k];
                    if (urlFile2.fileType == FileType.Config && id[index] == urlFile2.name)
                    {
                        __result = urlFile2.GetConfig(id[index + 1]);
                        return false; // Skip the original method.
                    }
                    k++;
                }
            }
            __result = null;
            return false; // Skip the original method
        }
    }


}
