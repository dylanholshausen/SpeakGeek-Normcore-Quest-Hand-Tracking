using UnityEngine;
using UnityEditor;
using System;
using System.IO;

namespace Hdg
{
    public static class BuildHelpers
    {
        /// <summary>
        /// Helper function to write a link.xml into the HdgRemoteDebugRuntime directory to ensure the Remote Debug server is not stripped.
        /// Call this function before starting your build.
        /// </summary>
        public static void WriteRemoteDebugLinkXml(BuildTarget buildTarget)
        {
            var path = FindRemoteDebugPath();
            var dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
                return;

            var linkXml = Path.Combine(dir, "link.xml");

            try
            {
                var contents = @"<linker><assembly fullname=""XXX"" preserve=""all""/></linker>";
                var dllName = GetRemoteDebugDLLName(buildTarget);
                contents = contents.Replace("XXX", dllName);
                File.WriteAllText(linkXml, contents);
            }
            catch (Exception e)
            {
                Debug.LogError(string.Format("Remote Debug: Failed to write link.xml to {0}", linkXml));
                Debug.LogError(e.Message);
            }
        }

        /// <summary>
        /// Helper function to remove a link.xml from the HdgRemoteDebugRuntime directory.
        /// Call this function once your build has finished.
        /// </summary>
        public static void RemoveRemoteDebugLinkXml()
        {
            var path = FindRemoteDebugPath();
            var dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
                return;

            var linkXml = Path.Combine(dir, "link.xml");
            if (!File.Exists(linkXml))
                return;

            try
            {
                // Delete the link.xml and the meta file.
                File.Delete(linkXml);
                var meta = linkXml + ".meta";
                if (File.Exists(meta))
                    File.Delete(meta);
            }
            catch (Exception e)
            {
                Debug.LogError(string.Format("Remote Debug: Failed to remove link.xml from {0}", linkXml));
                Debug.LogError(e.Message);
            }
        }

        /// <summary>
        /// Helper function to disable the Remote Debug plugin before building.
        /// Call this function before starting your build.
        /// </summary>
        /// <param name="buildTarget"></param>
        public static void DisableRemoteDebug(BuildTarget buildTarget)
        {
            var importers = PluginImporter.GetAllImporters();
            var dllName = GetRemoteDebugDLLName(buildTarget);
            foreach (var importer in importers)
            {
                if (importer.assetPath.IndexOf(dllName, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    Debug.Log("Remote Debug: Disabling Remote Debug plugin..");
                    importer.SetCompatibleWithPlatform(buildTarget, false);
                }
            }
        }

        /// <summary>
        /// Helper function to re-enable the Remote Debug plugin after building.
        /// Call this function once your build has finished.
        /// </summary>
        /// <param name="buildTarget"></param>
        public static void EnableRemoteDebug(BuildTarget buildTarget)
        {
            var importers = PluginImporter.GetAllImporters();
            var dllName = GetRemoteDebugDLLName(buildTarget);
            foreach (var importer in importers)
            {
                if (importer.assetPath.IndexOf(dllName, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    Debug.Log("Remote Debug: Enabling Remote Debug plugin..");
                    importer.SetCompatibleWithPlatform(buildTarget, true);
                }
            }
        }

        /// <summary>
        /// Returns the path to Remote Debug.
        /// </summary>
        /// <returns>Relative plugin path</returns>
        private static string FindRemoteDebugPath()
        {
            var importers = PluginImporter.GetAllImporters();
            foreach (var importer in importers)
            {
                if (importer.assetPath.IndexOf("HdgRemoteDebugRuntime", StringComparison.OrdinalIgnoreCase) >= 0)
                    return importer.assetPath;
            }

            return "";
        }

        /// <summary>
        /// Returns the appropriate Remote Debug DLL name depending on the given build target.
        /// </summary>
        /// <param name="buildTarget">Build target</param>
        /// <returns>DLL name</returns>
        private static string GetRemoteDebugDLLName(BuildTarget buildTarget)
        {
            switch (buildTarget)
            {
                case BuildTarget.WSAPlayer:
                {
                    // UWP builds use the UWP runtime.
                    return "HdgRemoteDebugRuntimeUWP";
                }

                default:
                {
                    return "HdgRemoteDebugRuntime";
                }
            }
        }
    }
}
