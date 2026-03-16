using UnityEngine;
using UnityEditor;
using TMPro;

public class Iteration10_PolishSetup : EditorWindow
{
    [MenuItem("WheelGame/Iteration 10 - Setup Final Polish (GameScene)")]
    public static void ShowWindow()
    {
        GetWindow<Iteration10_PolishSetup>("Iteration 10 - Final Polish");
    }

    private void OnGUI()
    {
        GUILayout.Label("Iteration 10 - Final Polish", EditorStyles.boldLabel);
        GUILayout.Space(10);
        GUILayout.Label("Run on GameScene.\n" +
                         "Adds:\n" +
                         "- FXManager (procedural particle systems)\n" +
                         "- UIJuice (low life pulse, timer colors)\n" +
                         "- Wires references to GameSceneUI elements", EditorStyles.wordWrappedLabel);
        GUILayout.Space(15);

        if (GUILayout.Button("Setup Final Polish (Iteration 10)", GUILayout.Height(40)))
        {
            Setup();
        }
    }

    private static void Setup()
    {
        SetupFXManager();
        SetupUIJuice();

        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
            UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
        Debug.Log("[Iteration 10] Final Polish setup complete!");
    }

    private static void SetupFXManager()
    {
        if (Object.FindObjectOfType<FXManager>() != null)
        {
            Debug.Log("[Iteration 10] FXManager already exists");
            return;
        }

        GameObject fxObj = new GameObject("FXManager");
        fxObj.AddComponent<FXManager>();
        Debug.Log("[Iteration 10] Created FXManager (particle systems created at runtime)");
    }

    private static void SetupUIJuice()
    {
        GameSceneUI gsUI = Object.FindObjectOfType<GameSceneUI>();
        Debug.Assert(gsUI != null, "[Iteration 10] GameSceneUI not found!");

        UIJuice juice = Object.FindObjectOfType<UIJuice>();
        if (juice == null)
        {
            juice = gsUI.gameObject.AddComponent<UIJuice>();
            Debug.Log("[Iteration 10] Added UIJuice to GameCanvas");
        }

        juice.livesText = gsUI.livesText;
        juice.livesContainer = gsUI.livesContainer;
        juice.timerText = gsUI.timerText;

        EditorUtility.SetDirty(juice);
        Debug.Log("[Iteration 10] UIJuice wired to lives and timer");
    }
}
