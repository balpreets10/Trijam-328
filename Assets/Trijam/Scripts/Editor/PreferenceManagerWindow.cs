using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class PreferenceManagerWindow : EditorWindow
{
    private Vector2 scrollPosition;
    private PreferenceService preferenceService;
    private Dictionary<string, bool> foldoutStates = new Dictionary<string, bool>();

    [MenuItem("Tools/Preference Manager")]
    public static void ShowWindow()
    {
        GetWindow<PreferenceManagerWindow>("Preference Manager");
    }

    private void OnEnable()
    {
        preferenceService = new PreferenceService();
    }

    private void OnGUI()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        DrawPreferenceSection("Game Settings", () =>
        {
            DrawPreference("Current Level", preferenceService.CurrentLevel, (value) => preferenceService.CurrentLevel = value);
            DrawPreference("High Score", preferenceService.HighScore, (value) => preferenceService.HighScore = value);
        });

        DrawPreferenceSection("Audio Settings", () =>
        {
            DrawSliderPreference("Sound Volume", preferenceService.SoundVolume, (value) => preferenceService.SoundVolume = value);
            DrawSliderPreference("Music Volume", preferenceService.MusicVolume, (value) => preferenceService.MusicVolume = value);
        });
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space(10);

        if (GUILayout.Button("Reset All Preferences", GUILayout.Height(30)))
        {
            if (EditorUtility.DisplayDialog("Reset Preferences",
                "Are you sure you want to reset all preferences to their default values?",
                "Yes", "No"))
            {
                preferenceService.ResetAll();
            }
        }

        EditorGUILayout.EndScrollView();
    }

    private void DrawPreferenceSection(string title, System.Action drawContent)
    {
        if (!foldoutStates.ContainsKey(title))
            foldoutStates[title] = true;

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        foldoutStates[title] = EditorGUILayout.Foldout(foldoutStates[title], title, true);
        if (foldoutStates[title])
        {
            EditorGUI.indentLevel++;
            drawContent();
            EditorGUI.indentLevel--;
        }
        EditorGUILayout.EndVertical();
    }

    private void DrawPreference(string label, int value, System.Action<int> setter)
    {
        EditorGUI.BeginChangeCheck();
        int newValue = EditorGUILayout.IntField(label, value);
        if (EditorGUI.EndChangeCheck())
        {
            setter(newValue);
        }
    }

    private void DrawSliderPreference(string label, float value, System.Action<float> setter)
    {
        EditorGUI.BeginChangeCheck();
        float newValue = EditorGUILayout.Slider(label, value, 0f, 1f);
        if (EditorGUI.EndChangeCheck())
        {
            setter(newValue);
        }
    }
}