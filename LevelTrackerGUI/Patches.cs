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
		[HarmonyPatch(typeof(Player), "Die")]
		[HarmonyPatch(typeof(Player), "OnStageEnd")]
		[HarmonyPatch(typeof(Player), "OnSurrender")]
		public static void OnUnloadTrigger()
		{
			BlockTracker.Unload();
		}

		[HarmonyPostfix]
		[HarmonyPatch(typeof(Player), "OnRockAndStoneInput")]
		public static void OnRockAndStone()
		{
			LevelTrackerGUI.Plugin.ShowGUI = !LevelTrackerGUI.Plugin.ShowGUI;
		}
	}
}