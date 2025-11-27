using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using UnityEngine;
using System.Linq;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using LevelTracker;
using BepInEx.Configuration;

namespace LevelTrackerGUI;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BasePlugin
{
    internal static new ManualLogSource Log;
    public static bool ShowGUI { get; set; } = true;
    internal static Rect windowRect = new Rect(20f, 0f, 190f, 140f);
    internal static ConfigEntry<bool> CfgOpenAtStart;
    internal static ConfigEntry<int> CfgWindowX;
    internal static ConfigEntry<int> CfgWindowY;
    private static int lastSavedX;
    private static int lastSavedY;
    private readonly Harmony harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);

    public override void Load()
    {
        Log = base.Log;
        Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");

        harmony.PatchAll();

        ClassInjector.RegisterTypeInIl2Cpp<GUIComponent>();
        AddComponent<GUIComponent>();

        // Config bindings
        CfgOpenAtStart = Config.Bind("General", "OpenAtStart", true, "Show the Level Tracker window automatically at game start.");
        CfgWindowX = Config.Bind("Window", "PosX", (int)windowRect.x, "Window X position.");
        CfgWindowY = Config.Bind("Window", "PosY", -1, "Window Y position. -1 centers vertically on first run.");

        ShowGUI = CfgOpenAtStart.Value;
        windowRect.x = CfgWindowX.Value;
        lastSavedX = CfgWindowX.Value;
        lastSavedY = CfgWindowY.Value;
    }

    internal static void SaveWindowPositionIfChanged()
    {
        int ix = (int)windowRect.x;
        int iy = (int)windowRect.y;
        if (ix != lastSavedX || iy != lastSavedY)
        {
            CfgWindowX.Value = ix;
            CfgWindowY.Value = iy;
            lastSavedX = ix;
            lastSavedY = iy;
            try { CfgWindowX.ConfigFile.Save(); } catch { }
        }
    }
}

public class GUIComponent : MonoBehaviour
{
    public GUIComponent(System.IntPtr ptr) : base(ptr) { }

    private void Start()
    {
        if (Plugin.CfgWindowY.Value < 0)
        {
            Plugin.windowRect.y = (Screen.height - Plugin.windowRect.height) / 2f;
        }
        else
        {
            Plugin.windowRect.y = Plugin.CfgWindowY.Value;
        }
    }

    private void Update()
    {
        // Toggling is handled via Harmony patch: Patches.OnRockAndStoneInput
    }

    private void OnGUI()
    {
        if (!Plugin.ShowGUI) return;

        Plugin.windowRect = GUI.Window(12345, Plugin.windowRect, (UnityEngine.GUI.WindowFunction)DrawWindow, "Level Tracker");
    }

    private void DrawWindow(int windowID)
    {
        // Layout constants
        const float topPadding = 14f;    // room for window title bar
        const float headerHeight = 22f;  // header line
        const float rowHeight = 20f;     // each data row
        const float bottomPadding = 8f;  // minimal bottom room
        const float contentX = 8f;       // left padding

        var sourceList = LevelTracker.BlockTracker.List;

        // Header (store text so we can size to it when list empty)
        const string headerText = "Rock & Stone: Toggle";
        GUI.Label(new Rect(contentX, topPadding, Plugin.windowRect.width - contentX * 2f, headerHeight), headerText);

        if (sourceList == null || sourceList.Count == 0)
        {
            GUI.Label(new Rect(contentX, topPadding + headerHeight + 4f, 140f, rowHeight), "No ore data");
            const float charWidthHeader = 8f;
            const float sidePaddingXHeader = 28f;
            float headerMinWidth = sidePaddingXHeader + headerText.Length * charWidthHeader;
            Plugin.windowRect.width = Mathf.Max(Plugin.windowRect.width, headerMinWidth);
            // Shrink window to minimal height when empty
            Plugin.windowRect.height = topPadding + headerHeight + rowHeight + bottomPadding + 6f;
        }
        else
        {
            var ordered = sourceList
                .OrderByDescending(x => x.Type)
                .Where(x => x.TotalBlocks > 0)
                .ToList();

            // Determine padding width for names
            int nameWidth = ordered.Count > 0 ? ordered.Max(b => b.Name.Length) : 0;

            // Compute window width based on content
            int maxBlocks = ordered.Count > 0 ? ordered.Max(b => b.TotalBlocks) : 0;
            int maxTotal = ordered.Count > 0 ? ordered.Max(b => b.TotalCurrency) : 0;
            int maxBlocksDigits = Mathf.Max(2, maxBlocks.ToString().Length);
            int maxTotalDigits = Mathf.Max(2, maxTotal.ToString().Length);
            int suffixChars = 2 /*": "*/ + maxBlocksDigits + 3 /*" | "*/ + maxTotalDigits;
            const float charWidth = 8f;
            const float sidePaddingX = 28f;
            float desiredWidth = sidePaddingX + (nameWidth + suffixChars) * charWidth;
            // Ensure width is at least enough for header text
            float headerMinWidth = sidePaddingX + headerText.Length * charWidth;
            desiredWidth = Mathf.Max(desiredWidth, headerMinWidth);
            Plugin.windowRect.width = Mathf.Clamp(desiredWidth, headerMinWidth, 260f);

            // Dynamic height
            int rowsToShow = ordered.Count;
            float desiredHeight = topPadding + headerHeight + rowsToShow * rowHeight + bottomPadding;
            // Only expand if larger than current (prevents flashing if Harmony patches run early)
            if (desiredHeight > Plugin.windowRect.height)
                Plugin.windowRect.height = desiredHeight;

            float innerWidth = Plugin.windowRect.width - 2f * contentX;
            const float colGap = 6f;
            const float glyphWidth = 8f;

            for (int i = 0; i < ordered.Count; i++)
            {
                var bd = ordered[i];
                string qtyText = $"{bd.TotalBlocks:00} | {bd.TotalCurrency:00}";
                float qtyWidthPx = qtyText.Length * glyphWidth;
                float y = topPadding + headerHeight + i * rowHeight;
                float qtyX = contentX + innerWidth - qtyWidthPx - 4f;
                float nameWidthPx = Mathf.Max(20f, qtyX - contentX - colGap);

                var oldColor = GUI.contentColor;
                GUI.contentColor = (int)bd.Type <= 3 ? Color.white : new Color(1f, 0.92f, 0.016f);
                GUI.Label(new Rect(contentX, y, nameWidthPx, rowHeight), bd.Name);
                GUI.Label(new Rect(qtyX, y, qtyWidthPx + 2f, rowHeight), qtyText);
                GUI.contentColor = oldColor;
            }
        }

        GUI.DragWindow(new Rect(0, 0, 10000, 20));
        Plugin.SaveWindowPositionIfChanged();
    }
}
