using UnityEngine;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using System.Collections.Generic;
using System.IO;

public class Iteration45_DeepDiagnose : EditorWindow
{
    [MenuItem("WheelGame/Iteration 4.5 - Deep Diagnose")]
    public static void ShowWindow()
    {
        GetWindow<Iteration45_DeepDiagnose>("Deep Diagnose");
    }

    private Vector2 scrollPos;
    private List<string> log = new List<string>();

    private void OnGUI()
    {
        GUILayout.Label("Addressables Deep Diagnostics", EditorStyles.boldLabel);
        GUILayout.Space(10);

        if (GUILayout.Button("Run Deep Diagnosis", GUILayout.Height(40)))
        {
            log.Clear();
            RunDeepDiagnosis();
            Repaint();
        }

        GUILayout.Space(5);

        if (GUILayout.Button("FIX: Set Play Mode to Simulate Groups", GUILayout.Height(30)))
        {
            FixPlayMode();
            Repaint();
        }

        GUILayout.Space(10);

        scrollPos = GUILayout.BeginScrollView(scrollPos);
        foreach (string line in log)
        {
            if (line.StartsWith("[ERROR]"))
                GUI.contentColor = Color.red;
            else if (line.StartsWith("[WARN]"))
                GUI.contentColor = Color.yellow;
            else if (line.StartsWith("[OK]"))
                GUI.contentColor = Color.green;
            else if (line.StartsWith("[FIX]"))
                GUI.contentColor = Color.cyan;
            else
                GUI.contentColor = Color.white;

            GUILayout.Label(line, EditorStyles.wordWrappedLabel);
        }
        GUI.contentColor = Color.white;
        GUILayout.EndScrollView();
    }

    private void RunDeepDiagnosis()
    {
        Log("=== DEEP DIAGNOSTICS ===");
        Log("");

        CheckPlayModeScript();
        CheckRuntimeData();
        CheckProfileValues();
        CheckGroupPaths();
        CheckCatalogSettings();
        CheckBuildOutput();

        Log("");
        Log("=== DEEP DIAGNOSIS COMPLETE ===");
    }

    private void CheckPlayModeScript()
    {
        Log("--- PLAY MODE SCRIPT ---");

        AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
        if (settings == null)
        {
            Log("[ERROR] No settings!");
            return;
        }

        var activePlayModeIndex = settings.ActivePlayModeDataBuilderIndex;
        Log("  ActivePlayModeDataBuilderIndex: " + activePlayModeIndex);

        if (activePlayModeIndex >= 0 && activePlayModeIndex < settings.DataBuilders.Count)
        {
            var builder = settings.DataBuilders[activePlayModeIndex];
            if (builder != null)
            {
                string builderName = builder.GetType().Name;
                Log("  Active Play Mode: " + builderName);

                if (builderName.Contains("UseAssetDatabase") || builderName.Contains("VirtualMode"))
                {
                    Log("[OK] Using 'Use Asset Database' — works without building bundles");
                }
                else if (builderName.Contains("SimulateGroups") || builderName.Contains("Simulate"))
                {
                    Log("[OK] Using 'Simulate Groups' — simulates remote loading locally");
                }
                else if (builderName.Contains("ExistingBuild") || builderName.Contains("Packed"))
                {
                    Log("[WARN] Using 'Use Existing Build' — REQUIRES built bundles and valid catalog!");
                    Log("[FIX] Try switching to 'Use Asset Database (fastest)' or 'Simulate Groups' for editor testing");
                    Log("[FIX] 'Use Existing Build' is only needed for testing the actual download flow");

                    CheckExistingBuildData();
                }
                else
                {
                    Log("  Builder type: " + builderName);
                }
            }
            else
            {
                Log("[ERROR] Active DataBuilder is null at index " + activePlayModeIndex);
            }
        }
        else
        {
            Log("[ERROR] ActivePlayModeDataBuilderIndex out of range: " + activePlayModeIndex + " (builders count: " + settings.DataBuilders.Count + ")");
        }

        Log("  All DataBuilders:");
        for (int i = 0; i < settings.DataBuilders.Count; i++)
        {
            var db = settings.DataBuilders[i];
            string marker = (i == activePlayModeIndex) ? " <-- ACTIVE" : "";
            if (db != null)
                Log("    [" + i + "] " + db.GetType().Name + marker);
            else
                Log("    [" + i + "] NULL" + marker);
        }

        Log("");
    }

    private void CheckExistingBuildData()
    {
        Log("  --- Checking existing build data ---");

        string runtimePath = "Library/com.unity.addressables/aa/" + UnityEditor.EditorUserBuildSettings.activeBuildTarget;

        if (Directory.Exists(runtimePath))
        {
            string[] files = Directory.GetFiles(runtimePath, "*", SearchOption.AllDirectories);
            Log("  Runtime data folder exists: " + runtimePath);
            Log("  Files: " + files.Length);
            foreach (string f in files)
            {
                Log("    " + f);
            }

            bool hasSettings = false;
            foreach (string f in files)
            {
                if (f.Contains("settings") && f.EndsWith(".json"))
                {
                    hasSettings = true;
                    Log("[OK] Found settings.json: " + f);
                }
            }

            if (!hasSettings)
            {
                Log("[ERROR] No settings.json in runtime data! Addressables can't initialize.");
                Log("[FIX] Switch Play Mode to 'Use Asset Database' or rebuild bundles");
            }
        }
        else
        {
            Log("[ERROR] Runtime data folder does not exist: " + runtimePath);
            Log("[ERROR] This means no local build data exists for 'Use Existing Build' mode!");
            Log("[FIX] Either switch Play Mode to 'Use Asset Database' or build bundles first");
        }
    }

    private void CheckRuntimeData()
    {
        Log("--- RUNTIME DATA ---");

        string[] possiblePaths = new string[]
        {
            "Library/com.unity.addressables",
            "Library/com.unity.addressables/aa",
            "Library/com.unity.addressables/aa/iOS",
            "Library/com.unity.addressables/aa/iOS/iOS",
            "Assets/AddressableAssetsData",
            "Assets/AddressableAssetsData/AssetGroups",
        };

        foreach (string p in possiblePaths)
        {
            if (Directory.Exists(p))
            {
                int count = Directory.GetFiles(p).Length;
                Log("  EXISTS: " + p + " (" + count + " files)");
            }
            else
            {
                Log("  MISSING: " + p);
            }
        }

        string settingsAsset = "Assets/AddressableAssetsData/AddressableAssetSettings.asset";
        if (File.Exists(settingsAsset))
            Log("[OK] AddressableAssetSettings.asset exists");
        else
            Log("[ERROR] AddressableAssetSettings.asset MISSING!");

        string defaultSettingsPath = "Assets/AddressableAssetsData/DefaultObject.asset";
        if (File.Exists(defaultSettingsPath))
            Log("[OK] DefaultObject.asset exists");
        else
            Log("[WARN] DefaultObject.asset missing — may cause init issues");

        Log("");
    }

    private void CheckProfileValues()
    {
        Log("--- PROFILE VALUES (RESOLVED) ---");

        AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
        if (settings == null) return;

        string profileId = settings.activeProfileId;
        var ps = settings.profileSettings;

        List<string> varNames = ps.GetVariableNames();
        Log("  Profile variables:");

        foreach (string varName in varNames)
        {
            string rawValue = ps.GetValueByName(profileId, varName);
            string evaluatedValue = "";

            try
            {
                evaluatedValue = ps.EvaluateString(profileId, rawValue);
            }
            catch (System.Exception e)
            {
                evaluatedValue = "[EVAL ERROR: " + e.Message + "]";
            }

            Log("    " + varName + ":");
            Log("      Raw: '" + rawValue + "'");
            Log("      Resolved: '" + evaluatedValue + "'");

            if (string.IsNullOrEmpty(rawValue))
            {
                Log("[ERROR] " + varName + " is EMPTY!");
            }
        }

        Log("");
    }

    private void CheckGroupPaths()
    {
        Log("--- GROUP PATHS (RESOLVED) ---");

        AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
        if (settings == null) return;

        foreach (var group in settings.groups)
        {
            if (group == null) continue;

            var schema = group.GetSchema<BundledAssetGroupSchema>();
            if (schema == null) continue;

            string buildPath = "";
            string loadPath = "";

            try { buildPath = schema.BuildPath.GetValue(settings); } catch { buildPath = "[ERROR evaluating]"; }
            try { loadPath = schema.LoadPath.GetValue(settings); } catch { loadPath = "[ERROR evaluating]"; }

            Log("  Group: " + group.Name);
            Log("    BuildPath resolved: " + buildPath);
            Log("    LoadPath resolved: " + loadPath);
            Log("    BuildPath ID: " + schema.BuildPath.Id);
            Log("    LoadPath ID: " + schema.LoadPath.Id);
        }

        Log("");
    }

    private void CheckCatalogSettings()
    {
        Log("--- CATALOG SETTINGS ---");

        AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
        if (settings == null) return;

        Log("  BuildRemoteCatalog: " + settings.BuildRemoteCatalog);

        if (settings.BuildRemoteCatalog)
        {
            string catalogBuildPath = "";
            string catalogLoadPath = "";

            try
            {
                catalogBuildPath = settings.RemoteCatalogBuildPath.GetValue(settings);
                catalogLoadPath = settings.RemoteCatalogLoadPath.GetValue(settings);
            }
            catch (System.Exception e)
            {
                Log("[ERROR] Failed to evaluate catalog paths: " + e.Message);
            }

            Log("  RemoteCatalogBuildPath resolved: " + catalogBuildPath);
            Log("  RemoteCatalogLoadPath resolved: " + catalogLoadPath);

            if (string.IsNullOrEmpty(catalogBuildPath))
                Log("[ERROR] RemoteCatalogBuildPath is empty!");

            if (string.IsNullOrEmpty(catalogLoadPath))
                Log("[ERROR] RemoteCatalogLoadPath is empty!");

            if (!string.IsNullOrEmpty(catalogLoadPath) && !catalogLoadPath.StartsWith("http"))
                Log("[WARN] RemoteCatalogLoadPath doesn't start with http — is this correct?");
        }

        Log("");
    }

    private void CheckBuildOutput()
    {
        Log("--- BUILD OUTPUT ---");

        string serverDataPath = "ServerData";
        if (Directory.Exists(serverDataPath))
        {
            string[] files = Directory.GetFiles(serverDataPath, "*", SearchOption.AllDirectories);
            Log("  ServerData files: " + files.Length);
            foreach (string f in files)
            {
                FileInfo fi = new FileInfo(f);
                Log("    " + f + " (" + (fi.Length / 1024) + " KB)");
            }
        }
        else
        {
            Log("[WARN] No ServerData folder");
        }

        Log("");
    }

    private void FixPlayMode()
    {
        log.Clear();
        AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
        if (settings == null)
        {
            Log("[ERROR] No settings found!");
            return;
        }

        for (int i = 0; i < settings.DataBuilders.Count; i++)
        {
            var db = settings.DataBuilders[i];
            if (db != null && db.GetType().Name.Contains("SimulateGroups"))
            {
                settings.ActivePlayModeDataBuilderIndex = i;
                EditorUtility.SetDirty(settings);
                AssetDatabase.SaveAssets();
                Log("[OK] Play Mode switched to 'Simulate Groups (advanced)' (index " + i + ")");
                Log("[OK] This simulates remote loading without needing actual bundles");
                Log("Now try running Bootstrap scene again!");
                return;
            }
        }

        for (int i = 0; i < settings.DataBuilders.Count; i++)
        {
            var db = settings.DataBuilders[i];
            if (db != null && (db.GetType().Name.Contains("UseAssetDatabase") || db.GetType().Name.Contains("VirtualMode")))
            {
                settings.ActivePlayModeDataBuilderIndex = i;
                EditorUtility.SetDirty(settings);
                AssetDatabase.SaveAssets();
                Log("[OK] Play Mode switched to 'Use Asset Database' (index " + i + ")");
                Log("Now try running Bootstrap scene again!");
                return;
            }
        }

        Log("[ERROR] Could not find a suitable Play Mode DataBuilder!");
    }

    private void Log(string msg)
    {
        log.Add(msg);
    }
}
