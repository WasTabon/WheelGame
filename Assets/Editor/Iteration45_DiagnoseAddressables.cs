using UnityEngine;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using System.Collections.Generic;

public class Iteration45_DiagnoseAddressables : EditorWindow
{
    [MenuItem("WheelGame/Iteration 4.5 - Diagnose Addressables")]
    public static void ShowWindow()
    {
        GetWindow<Iteration45_DiagnoseAddressables>("Diagnose Addressables");
    }

    private Vector2 scrollPos;
    private List<string> log = new List<string>();

    private void OnGUI()
    {
        GUILayout.Label("Addressables Diagnostics", EditorStyles.boldLabel);
        GUILayout.Space(10);

        if (GUILayout.Button("Run Full Diagnosis", GUILayout.Height(40)))
        {
            log.Clear();
            RunDiagnosis();
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
            else
                GUI.contentColor = Color.white;

            GUILayout.Label(line, EditorStyles.wordWrappedLabel);
        }
        GUI.contentColor = Color.white;
        GUILayout.EndScrollView();
    }

    private void RunDiagnosis()
    {
        Log("=== ADDRESSABLES DIAGNOSTICS ===");
        Log("");

        CheckSettings();
        CheckGroups();
        CheckProfiles();
        CheckCatalog();
        CheckLabels();
        CheckAddressableAssets();
        CheckBuildPath();

        Log("");
        Log("=== DIAGNOSIS COMPLETE ===");
    }

    private void CheckSettings()
    {
        Log("--- 1. SETTINGS ---");

        AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;

        if (settings == null)
        {
            Log("[ERROR] AddressableAssetSettings is NULL!");
            Log("[ERROR] Go to Window > Asset Management > Addressables > Groups and click 'Create Addressables Settings'");
            return;
        }

        Log("[OK] AddressableAssetSettings found: " + AssetDatabase.GetAssetPath(settings));

        Log("  ActiveProfileId: " + settings.activeProfileId);

        if (string.IsNullOrEmpty(settings.activeProfileId))
        {
            Log("[ERROR] No active profile set!");
        }
        else
        {
            string profileName = settings.profileSettings.GetProfileName(settings.activeProfileId);
            Log("  Active Profile Name: " + profileName);

            if (string.IsNullOrEmpty(profileName))
            {
                Log("[ERROR] Active profile ID exists but profile name is empty/invalid!");
            }
            else
            {
                Log("[OK] Active profile: " + profileName);
            }
        }

        Log("");
    }

    private void CheckGroups()
    {
        Log("--- 2. GROUPS ---");

        AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
        if (settings == null) { Log("[ERROR] No settings, skipping"); return; }

        if (settings.groups == null || settings.groups.Count == 0)
        {
            Log("[ERROR] No Addressable groups exist!");
            Log("[ERROR] Create a group: Addressables Groups window > Create > Group > Packed Assets");
            return;
        }

        Log("  Total groups: " + settings.groups.Count);

        foreach (var group in settings.groups)
        {
            if (group == null)
            {
                Log("[WARN] Found a null group reference (possibly deleted)");
                continue;
            }

            Log("  Group: '" + group.Name + "' | Entries: " + group.entries.Count);

            var schema = group.GetSchema<BundledAssetGroupSchema>();
            if (schema != null)
            {
                Log("    BuildPath: " + schema.BuildPath.GetValue(settings));
                Log("    LoadPath: " + schema.LoadPath.GetValue(settings));
            }
            else
            {
                Log("    [WARN] No BundledAssetGroupSchema on this group");
            }

            foreach (var entry in group.entries)
            {
                Log("    Asset: '" + entry.address + "' -> " + entry.AssetPath);

                if (entry.labels != null && entry.labels.Count > 0)
                {
                    Log("    Labels: " + string.Join(", ", entry.labels));
                }
                else
                {
                    Log("    [WARN] No labels on this asset");
                }
            }
        }

        Log("");
    }

    private void CheckProfiles()
    {
        Log("--- 3. PROFILES ---");

        AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
        if (settings == null) { Log("[ERROR] No settings, skipping"); return; }

        var profileSettings = settings.profileSettings;
        var profileNames = profileSettings.GetAllProfileNames();

        Log("  Profiles count: " + profileNames.Count);

        foreach (string name in profileNames)
        {
            string id = profileSettings.GetProfileId(name);
            Log("  Profile: '" + name + "' (id: " + id + ")");

            string remoteBuild = profileSettings.GetValueByName(id, "RemoteBuildPath");
            string remoteLoad = profileSettings.GetValueByName(id, "RemoteLoadPath");
            string localBuild = profileSettings.GetValueByName(id, "LocalBuildPath");
            string localLoad = profileSettings.GetValueByName(id, "LocalLoadPath");

            Log("    LocalBuildPath: " + localBuild);
            Log("    LocalLoadPath: " + localLoad);
            Log("    RemoteBuildPath: " + remoteBuild);
            Log("    RemoteLoadPath: " + remoteLoad);

            if (string.IsNullOrEmpty(remoteLoad) || remoteLoad.Contains("XXXX"))
            {
                Log("[WARN] RemoteLoadPath looks unconfigured! Should be your Cloudflare R2 URL");
            }
            else if (remoteLoad.StartsWith("http"))
            {
                Log("[OK] RemoteLoadPath looks like a valid URL");
            }
        }

        Log("");
    }

    private void CheckCatalog()
    {
        Log("--- 4. REMOTE CATALOG ---");

        AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
        if (settings == null) { Log("[ERROR] No settings, skipping"); return; }

        Log("  BuildRemoteCatalog: " + settings.BuildRemoteCatalog);

        if (!settings.BuildRemoteCatalog)
        {
            Log("[WARN] BuildRemoteCatalog is OFF. For remote loading, this should be ON.");
        }
        else
        {
            Log("[OK] BuildRemoteCatalog is ON");
        }

        Log("");
    }

    private void CheckLabels()
    {
        Log("--- 5. LABELS ---");

        AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
        if (settings == null) { Log("[ERROR] No settings, skipping"); return; }

        var labels = settings.GetLabels();
        Log("  Defined labels: " + labels.Count);

        foreach (string label in labels)
        {
            Log("    - " + label);
        }

        bool hasMusicLabel = labels.Contains("music");
        if (!hasMusicLabel)
        {
            Log("[ERROR] Label 'music' does not exist!");
            Log("[ERROR] In Addressables Groups window, select your music asset, click Labels dropdown, add 'music'");
        }
        else
        {
            Log("[OK] Label 'music' exists");
        }

        bool hasAssetsWithMusicLabel = false;
        foreach (var group in settings.groups)
        {
            if (group == null) continue;
            foreach (var entry in group.entries)
            {
                if (entry.labels.Contains("music"))
                {
                    hasAssetsWithMusicLabel = true;
                    Log("[OK] Asset with 'music' label found: " + entry.address + " in group " + group.Name);
                }
            }
        }

        if (!hasAssetsWithMusicLabel)
        {
            Log("[ERROR] No assets have the 'music' label assigned!");
            Log("[ERROR] Select your AudioClip in Addressables Groups, add label 'music' to it");
        }

        Log("");
    }

    private void CheckAddressableAssets()
    {
        Log("--- 6. ADDRESSABLE ASSETS ---");

        AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
        if (settings == null) { Log("[ERROR] No settings, skipping"); return; }

        int totalEntries = 0;
        bool foundGameMusic = false;

        foreach (var group in settings.groups)
        {
            if (group == null) continue;
            totalEntries += group.entries.Count;

            foreach (var entry in group.entries)
            {
                if (entry.address == "GameMusic")
                {
                    foundGameMusic = true;
                    Log("[OK] Found 'GameMusic' addressable: " + entry.AssetPath + " in group: " + group.Name);
                }
            }
        }

        Log("  Total addressable entries: " + totalEntries);

        if (totalEntries == 0)
        {
            Log("[ERROR] No assets are marked as Addressable!");
            Log("[ERROR] Select your .wav file, check 'Addressable' in Inspector, set address to 'GameMusic'");
        }

        if (!foundGameMusic)
        {
            Log("[ERROR] No asset with address 'GameMusic' found!");
            Log("[ERROR] Your music .wav file must have Addressable address set to 'GameMusic'");
        }

        Log("");
    }

    private void CheckBuildPath()
    {
        Log("--- 7. BUILD STATUS ---");

        string serverDataPath = "ServerData";
        if (System.IO.Directory.Exists(serverDataPath))
        {
            string[] files = System.IO.Directory.GetFiles(serverDataPath, "*", System.IO.SearchOption.AllDirectories);
            Log("  ServerData folder exists, files: " + files.Length);
            foreach (string f in files)
            {
                Log("    " + f);
            }

            if (files.Length == 0)
            {
                Log("[WARN] ServerData folder is empty. Run Build > New Build > Default Build Script in Addressables Groups");
            }
        }
        else
        {
            Log("[WARN] ServerData folder doesn't exist. Addressables haven't been built yet.");
            Log("[WARN] This is OK if you're still setting up, but you'll need to build before deploying.");
        }

        Log("");
    }

    private void Log(string msg)
    {
        log.Add(msg);
        if (msg.StartsWith("[ERROR]"))
            Debug.LogError("[Addressables Diag] " + msg);
        else if (msg.StartsWith("[WARN]"))
            Debug.LogWarning("[Addressables Diag] " + msg);
        else
            Debug.Log("[Addressables Diag] " + msg);
    }
}
