using UnityEngine;
using UnityEditor;

public class Iteration5_MusicReactiveSetup : EditorWindow
{
    [MenuItem("WheelGame/Iteration 5 - Setup Music Reactive")]
    public static void ShowWindow()
    {
        GetWindow<Iteration5_MusicReactiveSetup>("Iteration 5 - Music Reactive");
    }

    private void OnGUI()
    {
        GUILayout.Label("Iteration 5 - Music Reactive Setup", EditorStyles.boldLabel);
        GUILayout.Space(10);
        GUILayout.Label("This will add to the GameScene:\n" +
                         "- MusicReactor on AudioManager (spectrum analysis + beat detection)\n" +
                         "- WheelMusicSync on WheelRoot (rotation + pulse driven by music)\n" +
                         "- Wire all references", EditorStyles.wordWrappedLabel);
        GUILayout.Space(15);

        if (GUILayout.Button("Setup Music Reactive System (Iteration 5)", GUILayout.Height(40)))
        {
            SetupMusicReactive();
        }
    }

    private static void SetupMusicReactive()
    {
        SetupMusicReactor();
        SetupWheelMusicSync();

        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
            UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
        Debug.Log("[Iteration 5] Music Reactive system setup complete!");
    }

    private static void SetupMusicReactor()
    {
        AudioManager am = Object.FindObjectOfType<AudioManager>();

        MusicReactor reactor = Object.FindObjectOfType<MusicReactor>();
        if (reactor == null)
        {
            if (am != null)
            {
                reactor = am.gameObject.AddComponent<MusicReactor>();
                Debug.Log("[Iteration 5] Added MusicReactor to AudioManager");
            }
            else
            {
                GameObject reactorObj = new GameObject("MusicReactor");
                reactor = reactorObj.AddComponent<MusicReactor>();
                Debug.Log("[Iteration 5] Created MusicReactor (AudioManager not found on scene - it comes from Bootstrap via DontDestroyOnLoad)");
            }
        }

        if (am != null && am.musicSource != null)
        {
            reactor.audioSource = am.musicSource;
            Debug.Log("[Iteration 5] Wired MusicReactor audioSource to AudioManager.musicSource");
        }
        else
        {
            Debug.LogWarning("[Iteration 5] AudioManager or musicSource not found on scene. MusicReactor.audioSource will be wired at runtime.");
        }

        EditorUtility.SetDirty(reactor);
    }

    private static void SetupWheelMusicSync()
    {
        GameObject wheelRoot = GameObject.Find("WheelRoot");
        Debug.Assert(wheelRoot != null, "[Iteration 5] WheelRoot not found! Run Iteration 2 setup first.");

        WheelMusicSync sync = wheelRoot.GetComponent<WheelMusicSync>();
        if (sync == null)
        {
            sync = wheelRoot.AddComponent<WheelMusicSync>();
            Debug.Log("[Iteration 5] Added WheelMusicSync to WheelRoot");
        }

        WheelController wc = wheelRoot.GetComponent<WheelController>();
        Debug.Assert(wc != null, "[Iteration 5] WheelController not found on WheelRoot!");
        sync.wheelController = wc;

        MusicReactor reactor = Object.FindObjectOfType<MusicReactor>();
        if (reactor != null)
        {
            sync.musicReactor = reactor;
            Debug.Log("[Iteration 5] Wired WheelMusicSync references");
        }
        else
        {
            Debug.LogWarning("[Iteration 5] MusicReactor not found on scene - will be wired at runtime");
        }

        EditorUtility.SetDirty(sync);
    }
}
