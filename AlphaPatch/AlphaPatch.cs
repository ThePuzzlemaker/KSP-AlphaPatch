#pragma warning disable IDE0051
#pragma warning disable IDE0060
#pragma warning disable IDE0044
using System;
using System.IO;
using HarmonyLib;
using static UrlDir;
using System.Collections.Generic;
using Mono.Cecil;
using BepInEx.Logging;

namespace AlphaPatch
{
    public class AlphaPatch
    {
        public static IEnumerable<string> TargetDLLs { get; } = new[] { "Assembly-CSharp.dll" };

        public static void Finish()
        {
            var harmony = new Harmony("info.thepuzzlemaker.alphapatch");
            harmony.PatchAll();
            Logger.CreateLogSource(nameof(AlphaPatch)).LogInfo("=== AlphaPatch Initialized ===");
        }

        public static void Patch(AssemblyDefinition assembly) { }
    }

    [HarmonyPatch(typeof(UrlDir), MethodType.Constructor)]
    [HarmonyPatch(new Type[] { typeof(ConfigDirectory[]), typeof(ConfigFileType[]) })]
    class UrlDirPatch01
    {
        static AccessTools.FieldRef<UrlDir, List<UrlFile>> _files = AccessTools.FieldRefAccess<UrlDir, List<UrlFile>>("_files");
        static AccessTools.FieldRef<UrlDir, List<UrlDir>> _children = AccessTools.FieldRefAccess<UrlDir, List<UrlDir>>("_children");
        
        static bool Postfix(UrlDir __instance, ConfigDirectory[] dirConfig, ConfigFileType[] fileConfig)
        {
            _files(__instance).Sort(delegate (UrlFile a, UrlFile b)
            {
                return a.name.CompareTo(b.name);
            });
            _children(__instance).Sort(delegate (UrlDir a, UrlDir b)
            {
                return a.name.CompareTo(b.name);
            });  
	    return true;
        }
    }

    [HarmonyPatch(typeof(UrlDir), "Create")]
    [HarmonyPatch(new Type[] { typeof(UrlDir), typeof(DirectoryInfo) })]
    class UrlDirPatch02
    {
        static AccessTools.FieldRef<UrlDir, List<UrlFile>> _files = AccessTools.FieldRefAccess<UrlDir, List<UrlFile>>("_files");
        static AccessTools.FieldRef<UrlDir, List<UrlDir>> _children = AccessTools.FieldRefAccess<UrlDir, List<UrlDir>>("_children");

        static bool Postfix(UrlDir __instance, UrlDir parent, DirectoryInfo info)
        {
            _files(__instance).Sort(delegate (UrlFile a, UrlFile b)
            {
                return a.name.CompareTo(b.name);
            });
            _children(__instance).Sort(delegate (UrlDir a, UrlDir b)
            {
                return a.name.CompareTo(b.name);
            });
	    return true;
        }
    }

    [HarmonyPatch(typeof(UrlDir), "CreateDirectory")]
    [HarmonyPatch(new Type[] { typeof(UrlIdentifier), typeof(int) })]
    class UrlDirPatch03
    {
        static AccessTools.FieldRef<UrlDir, List<UrlFile>> _files = AccessTools.FieldRefAccess<UrlDir, List<UrlFile>>("_files");
        static AccessTools.FieldRef<UrlDir, List<UrlDir>> _children = AccessTools.FieldRefAccess<UrlDir, List<UrlDir>>("_children");

        static bool Postfix(UrlDir __instance, UrlIdentifier id, int index, ref UrlDir __result)
        {
            if (__result != null)
            {
                _files(__instance).Sort(delegate (UrlFile a, UrlFile b)
                {
                    return a.name.CompareTo(b.name);
                });
                _children(__instance).Sort(delegate (UrlDir a, UrlDir b)
                {
                    return a.name.CompareTo(b.name);
                });
            }
	    return true;
        }
    }
}
