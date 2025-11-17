using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public class AutoSave
{
    private const string ManualSaveKey = "autosave@manualSave";

    private static bool _isChangedHierarchy = false;

    static AutoSave()
    {
        IsManualSave = true;
        EditorApplication.playModeStateChanged += (state) => {
            if (IsAutoSave && !EditorApplication.isPlaying && EditorApplication.isPlayingOrWillChangePlaymode)
            {

                IsManualSave = false;

                if (IsSavePrefab)
                    AssetDatabase.SaveAssets();
                if (IsSaveScene)
                {
                    //Debug.Log ("save scene " + System.DateTime.Now);
                    EditorSceneManager.SaveOpenScenes();
                }
                IsManualSave = true;
            }
            _isChangedHierarchy = false;
        };

        var nextTime = EditorApplication.timeSinceStartup + Interval;
        EditorApplication.update += () =>
        {
            if (!_isChangedHierarchy || !(nextTime < EditorApplication.timeSinceStartup)) return;
            nextTime = EditorApplication.timeSinceStartup + Interval;

            IsManualSave = false;

            if (IsSaveSceneTimer && IsAutoSave && !EditorApplication.isPlaying)
            {
                if (IsSavePrefab)
                    AssetDatabase.SaveAssets();
                if (IsSaveScene)
                {
                    //Debug.Log ("save scene " + System.DateTime.Now);
                    EditorSceneManager.SaveOpenScenes();
                }
            }
            _isChangedHierarchy = false;
            IsManualSave = true;
        };

        EditorApplication.hierarchyChanged += () => {
            if (!EditorApplication.isPlaying)
                _isChangedHierarchy = true;
        };
    }

    public static bool IsManualSave
    {
        get => EditorPrefs.GetBool(ManualSaveKey);
        set => EditorPrefs.SetBool(ManualSaveKey, value);
    }

    private const string AutoSaveKey = "auto save";

    private static bool IsAutoSave
    {
        get
        {
            var value = EditorUserSettings.GetConfigValue(AutoSaveKey);
            return !string.IsNullOrEmpty(value) && value.Equals("True");
        }
        set => EditorUserSettings.SetConfigValue(AutoSaveKey, value.ToString());
    }

    private const string AutoSavePrefab = "auto save prefab";

    private static bool IsSavePrefab
    {
        get
        {
            var value = EditorUserSettings.GetConfigValue(AutoSavePrefab);
            return !string.IsNullOrEmpty(value) && value.Equals("True");
        }
        set => EditorUserSettings.SetConfigValue(AutoSavePrefab, value.ToString());
    }

    private const string AutoSaveScene = "auto save scene";

    private static bool IsSaveScene
    {
        get
        {
            var value = EditorUserSettings.GetConfigValue(AutoSaveScene);
            return !string.IsNullOrEmpty(value) && value.Equals("True");
        }
        set => EditorUserSettings.SetConfigValue(AutoSaveScene, value.ToString());
    }

    private static readonly string autoSaveSceneTimer = "auto save scene timer";

    private static bool IsSaveSceneTimer
    {
        get
        {
            var value = EditorUserSettings.GetConfigValue(autoSaveSceneTimer);
            return !string.IsNullOrEmpty(value) && value.Equals("True");
        }
        set => EditorUserSettings.SetConfigValue(autoSaveSceneTimer, value.ToString());
    }

    private const string AutoSaveInterval = "save scene interval";

    private static int Interval
    {
        get
        {

            var value = EditorUserSettings.GetConfigValue(AutoSaveInterval) ?? "60";
            return int.Parse(value);
        }
        set
        {
            if (value < 60)
                value = 60;
            EditorUserSettings.SetConfigValue(AutoSaveInterval, value.ToString());
        }
    }

    [SettingsProvider]
    static SettingsProvider ExampleOnGUI()
    {
        var provider = new SettingsProvider("Preferences/Auto Save", SettingsScope.User)
        {
            label = "Auto Save", // メニュー名やタイトルで利用される
            guiHandler = (searchText) => {
                bool isAutoSave = EditorGUILayout.BeginToggleGroup("auto save", IsAutoSave);

                IsAutoSave = isAutoSave;
                EditorGUILayout.Space();

                IsSavePrefab = EditorGUILayout.ToggleLeft("save prefab", IsSavePrefab);
                IsSaveScene = EditorGUILayout.ToggleLeft("save scene", IsSaveScene);
                IsSaveSceneTimer = EditorGUILayout.BeginToggleGroup("save scene interval", IsSaveSceneTimer);
                Interval = EditorGUILayout.IntField("interval(sec)", Interval);
                EditorGUILayout.EndToggleGroup();
                EditorGUILayout.EndToggleGroup();
            },
            keywords = new HashSet<string>(new[] { "AutoSave" }) // 検索でヒットさせたいワード
        };
        return provider;
    }

    [MenuItem("File/Backup/Backup")]
    public static void Backup()
    {
        for (var i = 0; i < SceneManager.sceneCount; ++i)
        {
            var sceneName = SceneManager.GetSceneAt(i).path;
            var expoertPath = "Backup/" + sceneName;

            Directory.CreateDirectory(Path.GetDirectoryName(expoertPath));

            if (string.IsNullOrEmpty(sceneName))
                return;

            var data = File.ReadAllBytes(sceneName);
            File.WriteAllBytes(expoertPath, data);
        }
    }

    [MenuItem("File/Backup/Rollback")]
    public static void RollBack()
    {
        for (var i = 0; i < SceneManager.sceneCount; ++i)
        {
            var sceneName = SceneManager.GetSceneAt(i).path;
            var expoertPath = "Backup/" + sceneName;

            var data = File.ReadAllBytes(expoertPath);
            File.WriteAllBytes(sceneName, data);
            AssetDatabase.Refresh(ImportAssetOptions.Default);
        }
    }
}