using Harmony12;
using System;
using System.IO;
using System.Reflection;
using static UnityModManagerNet.UnityModManager;

namespace SilentsMapImporterExtension {
	static class Main {
		public static ModEntry modEntry;
		static bool patched = false;

		static void Load(ModEntry modEntry) {
			Main.modEntry = modEntry;

			modEntry.OnToggle = OnToggle;
		}

		static bool OnToggle(ModEntry modEntry, bool value) {
			if (!value) return true;
			else {
				if (!patched) {					
					if (AccessTools.TypeByName("HondunesMapImporter") != null) {
						var harmony = HarmonyInstance.Create("com.Silentbaws.MapImporterExtension");
						
						var original = AccessTools.Method(AccessTools.TypeByName("HondunesMapImporter"), "LoadAssetBundle");
						var prefix = typeof(HonduneImporterPatches).GetMethod("LoadAssetBundlePrefix");

						harmony.Patch(original, new HarmonyMethod(prefix), null);
						patched = true;
					} else {
						return false;
					}
				}
				return true;
			}
		}
	}
}

class HonduneImporterPatches {
	public static bool LoadAssetBundlePrefix(int selection) {
		Logger.Log("Loading asset bundle");

		string dllFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\SkaterXL\\Maps\\Dlls";
		string mapFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\SkaterXL\\Maps";

		string[] assetFiles = Directory.GetFiles(mapFolder);

		if (Directory.Exists(dllFolder)) {
			string fileName = Path.GetFileName(assetFiles[selection]);
			string path = dllFolder + "\\" + fileName + ".dll";
			if (File.Exists(path)) {
				Assembly.LoadFile(path);
			}
		}

		return true;
	}
}
