using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;


public class SceneSwitcherWindow : EditorWindow {
    private const string LABEL_CONTRIBUTE_NAME = "Made by KeyboardSmasher, Inspired by Freya Holmér";

    private const float BTN_SINWAVE_SPEED = 1f;
    private const float BTN_SINWAVE_FREQUENCY = .75f;
    private const float BTN_SINWAVE_AMPLITUDE = .75f;

    private const float BTN_COLOR_FREQUENCY = 1.5f;
    private const float BTN_COLOR_LERP_SPEED = 1.5f;

    private readonly static Color[] BTN_COLOR_ARRAY = new Color[] {
        new Color(255,51,51,255)/255f,
        new Color(255,153,51,255)/255f,
        new Color(255,255,51,255)/255f,
        new Color(153,255,51,255)/255f,
        new Color(51,255,51,255)/255f,
        new Color(51,255,153,255)/255f,
        new Color(51,255,255,255)/255f,
        new Color(51,51,255,255)/255f,
        new Color(153,51,255,255)/255f,
        new Color(255,51,255,255)/255f,
        new Color(255,51,153,255)/255f,
    };

    public static SceneSwitcherWindow CurrentActiveSceneSwitcher { get; private set; }

    [MenuItem("Tools/Open Scene")]
    public static void OpenWindown() => GetWindow<SceneSwitcherWindow>("Open Scene Window");
    public OpenSceneMode openSceneMode;

    GUIStyle m_labelContributeStyle;
    GUIStyle m_buttonSceneStyle;
    GUIStyle m_scrollViewStyle;

    GUIContent m_labelContributeContent;

    SerializedObject _so;
    List<string> _listScenePath;

    Vector2 m_scrollPosition;
    float m_timeColorLerp;

    void OnEnable() {
        _so = new SerializedObject(this);

        minSize = new Vector2(330f, 300f);

        openSceneMode = OpenSceneMode.Single;

        LoadStyle();
        LoadBuildScenePath();

        EditorBuildSettings.sceneListChanged += OnBuildSceneListChanged;

        CurrentActiveSceneSwitcher = this;
    }

    void OnDisable() {
        CurrentActiveSceneSwitcher = null;
        EditorBuildSettings.sceneListChanged -= OnBuildSceneListChanged;
    }

    void LoadStyle() {
        m_buttonSceneStyle = new GUIStyle() {
            alignment = TextAnchor.MiddleLeft,
            richText = true
        };
        m_scrollViewStyle = new GUIStyle() {
            alignment = TextAnchor.MiddleCenter,
        };
        m_labelContributeStyle = new GUIStyle() {
            alignment = TextAnchor.MiddleCenter,
        };
        m_labelContributeContent = new GUIContent("<size=12><b><color=#AAAAAA> " + LABEL_CONTRIBUTE_NAME + "</color></b></size>");
    }

    private void OnGUI() {
        _so.Update();
        GUILayout.BeginVertical();
        GUILayout.Space(20);
        DrawSceneButtons();
        GUILayout.FlexibleSpace();
        DrawContributes();
        GUILayout.Space(20);
        GUILayout.EndVertical();
        LoadBuildScenePath();
        Repaint();
    }

    public void LoadBuildScenePath() {
        //Load Scene
        IEnumerable<EditorBuildSettingsScene> buildScenes = EditorBuildSettings.scenes.Where(scene => scene.enabled);

        IEnumerable<string> paths = buildScenes.Select(scene => scene.path);

        _listScenePath = paths.ToList();

        Repaint();
    }

    private void DrawSceneButtons() {
        Rect rectScrollView = GUILayoutUtility.GetRect(new GUIContent(), m_scrollViewStyle, GUILayout.Height(Mathf.Max(position.height - 70, 60)));
        Rect rectButton = GUILayoutUtility.GetRect(new GUIContent(), m_buttonSceneStyle, GUILayout.Height(0));

        m_scrollPosition = GUI.BeginScrollView(rectScrollView, m_scrollPosition, new Rect(rectScrollView.x, rectScrollView.y, rectScrollView.width, 22 * _listScenePath.Count));

        rectButton.width *= 2.5f / 3f;
        rectButton.height = 20;
        rectButton.y = rectScrollView.y;
        float rootX = (position.width - rectButton.width) * .5f;

        m_timeColorLerp = Time.realtimeSinceStartup * BTN_COLOR_LERP_SPEED;

        for (int i = 0; i < _listScenePath.Count; i++) {
            float sinc = (Mathf.Sin(Time.realtimeSinceStartup * BTN_SINWAVE_SPEED + i * BTN_SINWAVE_FREQUENCY) * BTN_SINWAVE_AMPLITUDE + 1) / 2f * rootX * 2f;

            float lerpArray = m_timeColorLerp + BTN_COLOR_FREQUENCY * i;
            float lengthFloat = (float)BTN_COLOR_ARRAY.Length;
            if (lerpArray > lengthFloat)
                lerpArray = lerpArray - lengthFloat * (int)(lerpArray / lengthFloat);

            int startIndex = Mathf.FloorToInt(lerpArray);
            int endIndex = Mathf.CeilToInt(lerpArray);
            float lerpColor = Mathf.InverseLerp(startIndex, endIndex, lerpArray);
            //Fix end Index
            endIndex = endIndex % BTN_COLOR_ARRAY.Length;
            GUI.color = Color.Lerp(BTN_COLOR_ARRAY[startIndex], BTN_COLOR_ARRAY[endIndex], lerpColor);

            rectButton.x = sinc;
            if (GUI.Button(rectButton, "Open " + GetAssetName(_listScenePath[i]))) {
                OpenScene(_listScenePath[i]);
            }
            rectButton.y += 22;
        }
        GUI.color = Color.white;
        GUI.EndScrollView();
    }
    private void DrawContributes() {
        GUILayout.Label(m_labelContributeContent, m_labelContributeStyle);
    }
    private void OnBuildSceneListChanged() {
        LoadBuildScenePath();
    }
    private void OpenScene(string path) {
        if (EditorSceneManager.EnsureUntitledSceneHasBeenSaved("You haven't saved this scene, Do you want to save?")) {
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            EditorSceneManager.OpenScene(path, openSceneMode);
        }
    }

    static string GetAssetName(string asset) {
        return Path.GetFileNameWithoutExtension(asset);
    }
}