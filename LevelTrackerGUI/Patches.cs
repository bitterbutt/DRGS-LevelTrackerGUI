using System;
using HarmonyLib;

namespace LevelTracker
{
	[Harmony]
	public static class Patches
	{
		[HarmonyPostfix]
		[HarmonyPatch(typeof(Player), "OnExitDropPod")]
		public static void OnExitDropPod()
		{
			BlockTracker.Load();
		}

		[HarmonyPostfix]
		[HarmonyPatch(typeof(Player), "OnEnterDropPod")]
		public static void OnEnterDropPod()
		{
			BlockTracker.Unload();
		}

		[HarmonyPostfix]
		[HarmonyPatch(typeof(Player), "OnRockAndStoneInput")]
		public static void OnRockAndStone()
		{
			// Toggle GUI instead of listing to console
			// BlockTracker.ListBlocks();
			LevelTrackerGUI.Plugin.ShowGUI = !LevelTrackerGUI.Plugin.ShowGUI;
			LevelTrackerGUI.Plugin.Log.LogInfo($"GUI toggled: {LevelTrackerGUI.Plugin.ShowGUI}");
		}
	}
}
