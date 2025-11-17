using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Collections;
using System.IO;

//================================================================================
/// <summary>
/// 拡張エディターの汎用処理
/// </summary>
public class CommonEditorTools : EditorWindow
{
    //================================================================================
    /* レイアウト関連 */
    #region -- レイアウト関連

    //================================================================================
    /// <summary>
    /// 区間のオンオフ切り替え
    /// </summary>
    static public bool DrawHeader(string text, string key, bool forceOn) {
        bool state = EditorPrefs.GetBool(key, true);

        GUILayout.Space(3f);
        if (!forceOn && !state) GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f);
        GUILayout.BeginHorizontal();
        GUILayout.Space(3f);

        GUI.changed = false;

        if (!GUILayout.Toggle(true, "<b><size=11>" + text + "</size></b>", "dragtab", GUILayout.MinWidth(20f))) state = !state;
        if (GUI.changed) EditorPrefs.SetBool(key, state);

        GUILayout.Space(2f);
        GUILayout.EndHorizontal();
        GUI.backgroundColor = Color.white;
        if (!forceOn && !state) GUILayout.Space(3f);
        return state;
    }

    static public bool DrawHeader(string text) { return DrawHeader(text, text, false); }

    static public bool DrawHeader(string text, string key) { return DrawHeader(text, key, false); }

    static public bool DrawHeader(string text, bool forceOn) { return DrawHeader(text, text, forceOn); }

    //================================================================================
    /// <summary>
    /// 凹み状囲いの開始
    /// </summary>
    static public void BeginContents(bool flagVariableHeight) {
        GUILayout.BeginHorizontal();
        GUILayout.Space(4f);
        if (flagVariableHeight) {
            EditorGUILayout.BeginHorizontal("AS TextArea");
        } else {
            EditorGUILayout.BeginHorizontal("AS TextArea", GUILayout.MinHeight(10f));
        }
        GUILayout.BeginVertical();
        GUILayout.Space(2f);
    }

    //================================================================================
    /// <summary>
    /// 凹み状囲いの終了
    /// </summary>
    static public void EndContents() {
        GUILayout.Space(3f);
        GUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(3f);
        GUILayout.EndHorizontal();
        GUILayout.Space(3f);
    }

    //================================================================================
    /// <summary>
    /// Listを列挙する関数
    /// </summary>
    /// <param name="title"></param>
    /// <param name="list"></param>
    static public void CheckContent(string title, List<GameObject> list) {
        /* リストが空じゃなければ */
        if (list.Count != 0) {
            if (DrawHeader(title)) {
                BeginContents(true);
                EditorGUILayout.BeginVertical(GUI.skin.box);

                foreach (GameObject obj in list) {
                    EditorGUILayout.BeginHorizontal();

                    GameObject objField = EditorGUILayout.ObjectField("", obj, typeof(GameObject), true) as GameObject;

                    if (GUILayout.Button("Select")) {
                        Selection.activeObject = objField;
                    }

                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();
                EndContents();
            }
        }
    }

    #endregion

    //================================================================================
    /* ゲームオブジェクト関連 */
    #region -- ゲームオブジェクト関連

    //================================================================================
    /// <summary>
    /// 選択オブジェクトをHierarchyの上から順にリスト化
    /// </summary>
    /// <returns>Hierarchyの上から順にソートしたリスト</returns>
    static public List<GameObject> HierarchyObjSort() {
        List<GameObject> objLists = new List<GameObject>();

        foreach (var obj in Selection.objects) {
            objLists.Add(obj as GameObject);
        }

        /* Hierarchyの上からの順にソート */
        objLists.Sort((a, b) => a.transform.GetSiblingIndex() - b.transform.GetSiblingIndex());

        return objLists;
    }

    //================================================================================
    /// <summary>
    /// Hierarchyのアクティブオブジェクトを取得してリスト化
    /// </summary>
    /// <returns>Hierarchyのアクティブオブジェクト</returns>
    static public List<GameObject> HierarchyActiveObjGet() {
        List<GameObject> gameObjList = new List<GameObject>();

        /* typeで指定した型の全てのオブジェクトを配列で取得し,その要素数分繰り返す */
        foreach (GameObject obj in FindObjectsOfType(typeof(GameObject))) {
            /* シーン上に存在するオブジェクトならば処理 */
            if (obj.activeInHierarchy) {
                gameObjList.Add(obj);
            }
        }

        return gameObjList;
    }

    //================================================================================
    /// <summary>
    /// Hierarchyの全オブジェクトを取得してリスト化
    /// </summary>
    /// <returns>Hierarchyの全オブジェクト</returns>
    static public List<GameObject> HierarchyAllObjGet() {
        List<GameObject> gameObjList = new List<GameObject>();

        /* Typeで指定した型の全てのオブジェクトを配列で取得し,その要素数分繰り返す */
        foreach (GameObject obj in UnityEngine.Resources.FindObjectsOfTypeAll(typeof(GameObject))) {
            /* アセットからパスを取得.シーン上に存在するオブジェクトの場合,シーンファイル（.unity）のパスを取得 */
            string path = AssetDatabase.GetAssetOrScenePath(obj);
            /* シーン上に存在するオブジェクトかどうか文字列で判定 */
            bool isScene = path.Contains(".unity");
            /* シーン上に存在するオブジェクトならば処理 */
            if (isScene) {
                gameObjList.Add(obj);
            }
        }

        return gameObjList;
    }

    //================================================================================
    /// <summary>
    /// 選択オブジェのサイズを取得
    /// </summary>
    static public Vector3 GetObjSize() {
        Vector3 size = new Vector3();
        float minX = float.NaN;
        float maxX = float.NaN;
        float minY = float.NaN;
        float maxY = float.NaN;
        float minZ = float.NaN;
        float maxZ = float.NaN;

        foreach (GameObject gameObj in Selection.objects) {
            /* Rendererを含むオブジェクトに対して */
            Renderer[] targets = gameObj.GetComponentsInChildren<Renderer>();
            foreach (Renderer render in targets) {
                float lowX = render.transform.position.x - (render.bounds.size.x / 2.0f);
                float lowY = render.transform.position.y - (render.bounds.size.y / 2.0f);
                float lowZ = render.transform.position.z - (render.bounds.size.z / 2.0f);

                float highX = render.transform.position.x + (render.bounds.size.x / 2.0f);
                float highY = render.transform.position.y + (render.bounds.size.y / 2.0f);
                float highZ = render.transform.position.z + (render.bounds.size.z / 2.0f);

                /* 代入されたことがなかったら */
                if (float.IsNaN(minX) && float.IsNaN(maxX) &&
                    float.IsNaN(minY) && float.IsNaN(maxY) &&
                    float.IsNaN(minZ) && float.IsNaN(maxZ)) {
                    minX = lowX;
                    maxX = highX;
                    minY = lowY;
                    maxY = highY;
                    minZ = lowZ;
                    maxZ = highZ;
                }

                /* 入れ替え判定 */
                if (minX > lowX) {
                    minX = lowX;
                }
                if (maxX < highX) {
                    maxX = highX;
                }
                if (minY > lowY) {
                    minY = lowY;
                }
                if (maxY < highY) {
                    maxY = highY;
                }
                if (minZ > lowZ) {
                    minZ = lowZ;
                }
                if (maxZ < highZ) {
                    maxZ = highZ;
                }
            }
        }

        /* サイズの計算 */
        float sizeX = maxX - minX;
        float sizeY = maxY - minY;
        float sizeZ = maxZ - minZ;

        size = new Vector3(sizeX, sizeY, sizeZ);

        return size;
    }

    #endregion

    //================================================================================
    /* I.O関連 */
    #region -- I.O関連

    //================================================================================
    /// <summary>
    /// フォルダの作成
    /// </summary>
    /// <param name="path"></param>
    public static void CreateDirectory(string path) {
        /* フォルダが存在しなければ */
        if (!Directory.Exists(path)) {
            Directory.CreateDirectory(path);
        }
    }

    //================================================================================
    /// <summary>
    /// フォルダの削除
    /// </summary>
    /// <param name="path"></param>
    public static void DeleteDirectory(string path) {
        /* フォルダが存在しなければ */
        if (!Directory.Exists(path)) {
            return;
        }

        /* ディレクトリ以外の全ファイルを削除 */
        string[] filePaths = Directory.GetFiles(path);
        foreach (string filePath in filePaths) {
            File.SetAttributes(filePath, FileAttributes.Normal);
            File.Delete(filePath);
        }

        /* ディレクトリの中のディレクトリも再帰的に削除 */
        string[] directoryPaths = Directory.GetDirectories(path);
        foreach (string directoryPath in directoryPaths) {
            DeleteDirectory(directoryPath);
        }

        /* 中が空になったらディレクトリ自身も削除 */
        Directory.Delete(path, false);
    }

    //================================================================================
    /// <summary>
    /// フォルダとその中身を上書きコピー
    /// </summary>
    public static void CopyAndReplaceDirectory(string sourcePath, string copyPath) {
        /* フォルダが存在しなければ */
        if (!Directory.Exists(sourcePath)) {
            return;
        }

        /* 既にディレクトリがある場合は削除し、新たにディレクトリ作成 */
        DeleteDirectory(copyPath);
        Directory.CreateDirectory(copyPath);

        /* ファイルをコピー */
        foreach (var file in Directory.GetFiles(sourcePath)) {
            File.Copy(file, Path.Combine(copyPath, Path.GetFileName(file)));
        }

        /* ディレクトリの中のディレクトリも再帰的にコピー */
        foreach (var dir in Directory.GetDirectories(sourcePath)) {
            CopyAndReplaceDirectory(dir, Path.Combine(copyPath, Path.GetFileName(dir)));
        }
    }

    #endregion
}