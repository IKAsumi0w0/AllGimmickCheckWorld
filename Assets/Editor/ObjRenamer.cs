using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

//================================================================================
/// <summary>
/// 選択オブジェクト名のリネームツール
/// </summary>
//================================================================================
public class ObjRenamer : EditorWindow
{
    //================================================================================
    Vector2 mBeforeScrollPos = Vector2.zero;
    Vector2 mAfterScrollPos = Vector2.zero;

    List<string> mAfterObjNames = new List<string>();

    enum EnumSelected
    {
        Add,
        Replace,
        SerialNum,
        Delete
    }
    EnumSelected eSelected = EnumSelected.Add;

    // 追加 ------------------------------
    int mAddPosition = 0;

    string mAddText = "";

    enum EnumAddPopup
    {
        Prefix,
        Suffix
    }
    EnumAddPopup eAddPopup = EnumAddPopup.Prefix;

    // 置換 ------------------------------
    string mReplaceBeforeText = "";
    string mReplaceAfterText = "";

    // 連番 ------------------------------
    int mSerialNumStart = 0;
    int mSerialNumDigit = 1;

    bool mFlagOverride = false;

    string mOverrideText = "";

    enum EnumSerialNumPopup
    {
        Prefix,
        Suffix
    }
    EnumSerialNumPopup eSerialNumPopup = EnumSerialNumPopup.Suffix;

    // 削除 ------------------------------
    int mDeleteNumStart = 0;
    int mDeleteNumDigit = 1;

    enum EnumDeletePopup
    {
        Prefix,
        Suffix,
        Select
    }
    EnumDeletePopup eDeletePopup = EnumDeletePopup.Prefix;

    //================================================================================
    /// <summary>
    /// メニューのWindowに追加
    /// </summary>
    [MenuItem("Custom/Prefabs/ObjRenamer")]
    public static void OpenWindow()
    {
        EditorWindow.GetWindow<ObjRenamer>("ObjRenamer");
    }

    //================================================================================
    /// <summary>
    /// Hierarchyで選択時
    /// </summary>
    void OnSelectionChange()
    {
        //再描画
        Repaint();
    }

    //================================================================================
    /// <summary>
    /// メイン描画処理
    /// </summary>
    void OnGUI()
    {
        eSelected = (EnumSelected)GUILayout.Toolbar((int)eSelected, new string[] { "追加", "置換", "連番", "削除" }, EditorStyles.toolbarButton);

        CommonEditorTools.BeginContents(true);
        switch (eSelected) {
            case EnumSelected.Add:
                AddOfLayout();
                break;
            case EnumSelected.Replace:
                ReplaceOfLayout();
                break;
            case EnumSelected.SerialNum:
                SerialNumOfLayout();
                break;
            case EnumSelected.Delete:
                DeleteOfLayout();
                break;
            default:
                break;
        }

        CheckOfLayout();
        CommonEditorTools.EndContents();
    }

    //================================================================================
    /// <summary>
    /// 各リネーム処理の反映処理
    /// </summary>
    /// <param name="objNames">各リネーム処理後のオブジェクト名のリスト</param>
    void RunRename(List<string> objNames)
    {
        List<GameObject> selectObjs = new List<GameObject>();
        int count = 0;

        foreach (var obj in Selection.objects) {
            selectObjs.Add(obj as GameObject);
        }

        // Hierarchyの上からの順にソート
        selectObjs.Sort((a, b) => a.transform.GetSiblingIndex() - b.transform.GetSiblingIndex());

        foreach (var obj in selectObjs) {
            GameObject gameObj = obj as GameObject;
            Undo.RecordObject(gameObj, "Undo Rename");
            gameObj.name = objNames[count];
            count++;
        }
    }

    //================================================================================
    /// <summary>
    /// 追加のレイアウト設定
    /// </summary>
    void AddOfLayout()
    {
        if (CommonEditorTools.DrawHeader("Set Param")) {
            CommonEditorTools.BeginContents(false);
            EditorGUILayout.BeginVertical(GUI.skin.box);
            eAddPopup = (EnumAddPopup)EditorGUILayout.EnumPopup("前後(Prefix/Suffix)", (System.Enum)eAddPopup);
            mAddPosition = EditorGUILayout.IntField("何桁目に", mAddPosition);
            mAddText = EditorGUILayout.TextField("追加文字", mAddText);
            EditorGUILayout.EndVertical();
            CommonEditorTools.EndContents();
        }
        if (GUILayout.Button("追加する")) {
            RunRename(AddName());
        }
    }

    //================================================================================
    /// <summary>
    /// 追加の処理
    /// </summary>
    /// <returns>処理後のオブジェクト名のリスト</returns>
    List<string> AddName()
    {
        List<string> objNames = new List<string>();

        foreach (var obj in CommonEditorTools.HierarchyObjSort()) {
            GameObject gameObj = obj as GameObject;
            if (eAddPopup == EnumAddPopup.Prefix) {
                objNames.Add(gameObj.name.Insert(mAddPosition, mAddText));
            } else {
                objNames.Add(gameObj.name.Insert(gameObj.name.Length - mAddPosition, mAddText));
            }
        }

        return objNames;
    }

    //================================================================================
    /// <summary>
    /// 置換のレイアウト設定
    /// </summary>
    void ReplaceOfLayout()
    {
        if (CommonEditorTools.DrawHeader("Set Param")) {
            CommonEditorTools.BeginContents(false);
            EditorGUILayout.BeginVertical(GUI.skin.box);
            mReplaceBeforeText = EditorGUILayout.TextField("この文字を", mReplaceBeforeText);
            mReplaceAfterText = EditorGUILayout.TextField("この文字に", mReplaceAfterText);
            EditorGUILayout.EndVertical();
            CommonEditorTools.EndContents();
        }
        if (GUILayout.Button("置換する")) {
            RunRename(ReplaceName());
        }
    }

    //================================================================================
    /// <summary>
    /// 置換の処理
    /// </summary>
    /// <returns>処理後のオブジェクト名のリスト</returns>
    List<string> ReplaceName()
    {
        List<string> objNames = new List<string>();

        foreach (var obj in CommonEditorTools.HierarchyObjSort()) {
            GameObject gameObj = obj as GameObject;
            // 名前に置換前の文字が含まれていない or 置換前の文字列が""だったら
            if (!gameObj.name.Contains(mReplaceBeforeText) || mReplaceBeforeText == "") {
                objNames.Add(gameObj.name);
            } else {
                objNames.Add((gameObj.name.Replace(mReplaceBeforeText, mReplaceAfterText)));
            }
        }

        return objNames;
    }

    //================================================================================
    /// <summary>
    /// 連番のレイアウト設定
    /// </summary>
    void SerialNumOfLayout()
    {
        if (CommonEditorTools.DrawHeader("Set Param")) {
            CommonEditorTools.BeginContents(false);
            EditorGUILayout.BeginVertical(GUI.skin.box);
            mFlagOverride = EditorGUILayout.Toggle("上書き", mFlagOverride);
            if (mFlagOverride) {
                mOverrideText = EditorGUILayout.TextField("置き換え文字", mOverrideText);
            }
            eSerialNumPopup = (EnumSerialNumPopup)EditorGUILayout.EnumPopup("前後(Prefix/Suffix)", (System.Enum)eSerialNumPopup);
            mSerialNumStart = EditorGUILayout.IntField("開始数", mSerialNumStart);
            mSerialNumDigit = EditorGUILayout.IntField("桁数", mSerialNumDigit);
            EditorGUILayout.EndVertical();
            CommonEditorTools.EndContents();
        }

        if (GUILayout.Button("連番付け")) {
            RunRename(AddSerialNum());
        }
    }

    //================================================================================
    /// <summary>
    /// 連番の処理
    /// </summary>
    /// <returns>処理後のオブジェクト名のリスト</returns>
    List<string> AddSerialNum()
    {
        List<string> objNames = new List<string>();
        int serialNumCount = mSerialNumStart;

        foreach (var obj in CommonEditorTools.HierarchyObjSort()) {
            GameObject gameObj = obj as GameObject;
            string serialNumName = gameObj.name;

            if (mFlagOverride) {
                serialNumName = mOverrideText;
            }

            if (eSerialNumPopup == EnumSerialNumPopup.Prefix) {
                objNames.Add(serialNumCount.ToString("D" + mSerialNumDigit) + serialNumName);
            } else {
                objNames.Add(serialNumName + serialNumCount.ToString("D" + mSerialNumDigit));
            }

            serialNumCount++;
        }

        return objNames;
    }

    //================================================================================
    /// <summary>
    /// 削除のレイアウト設定
    /// </summary>
    void DeleteOfLayout()
    {
        if (CommonEditorTools.DrawHeader("Set Param")) {
            CommonEditorTools.BeginContents(false);
            EditorGUILayout.BeginVertical(GUI.skin.box);
            eDeletePopup = (EnumDeletePopup)EditorGUILayout.EnumPopup("削除方法", (System.Enum)eDeletePopup);
            if (eDeletePopup == EnumDeletePopup.Select) {
                mDeleteNumStart = EditorGUILayout.IntField("何桁目から", mDeleteNumStart);
                mDeleteNumDigit = EditorGUILayout.IntField("桁数", mDeleteNumDigit);
            } else {
                mDeleteNumDigit = EditorGUILayout.IntField("桁数", mDeleteNumDigit);
            }
            EditorGUILayout.EndVertical();
            CommonEditorTools.EndContents();
        }
        if (GUILayout.Button("削除する")) {
            RunRename(DeleteName());
        }
    }

    //================================================================================
    /// <summary>
    /// 名前の文字列を削除する処理
    /// </summary>
    /// <returns>処理後のオブジェクト名のリスト</returns>
	List<string> DeleteName()
    {
        List<string> objNames = new List<string>();

        foreach (var obj in CommonEditorTools.HierarchyObjSort()) {
            GameObject gameObj = obj as GameObject;

            if (mDeleteNumDigit <= 0 || mDeleteNumDigit > obj.name.Length) {
                objNames.Add(gameObj.name);
            } else {
                switch (eDeletePopup) {
                    case EnumDeletePopup.Prefix:
                        objNames.Add(gameObj.name.Remove(0, mDeleteNumDigit));
                        break;
                    case EnumDeletePopup.Suffix:
                        objNames.Add(gameObj.name.Remove(gameObj.name.Length - mDeleteNumDigit));
                        break;
                    case EnumDeletePopup.Select:
                        objNames.Add(gameObj.name.Remove(mDeleteNumStart, mDeleteNumDigit));
                        break;
                    default:
                        objNames.Add(gameObj.name);
                        break;
                }
            }
        }

        return objNames;
    }

    //================================================================================
    /// <summary>
    /// 変更確認のレイアウト
    /// </summary>
    void CheckOfLayout()
    {
        if (CommonEditorTools.DrawHeader("Check")) {
            // 何も選択されていなかったら
            if (!Selection.activeGameObject) {
                CommonEditorTools.BeginContents(false);
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.HelpBox("オブジェクトを選択してください", MessageType.Warning);
            } else {
                CommonEditorTools.BeginContents(true);
                EditorGUILayout.BeginHorizontal();

                //== 変更前 ==
                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField("変更前");
                mBeforeScrollPos = EditorGUILayout.BeginScrollView(mBeforeScrollPos, GUI.skin.box);
                foreach (var obj in CommonEditorTools.HierarchyObjSort()) {
                    GameObject gameObj = obj as GameObject;
                    EditorGUILayout.LabelField(gameObj.name);
                }
                EditorGUILayout.EndScrollView();
                EditorGUILayout.EndVertical();

                //== 変更後 ==
                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField("変更後");
                mAfterScrollPos = EditorGUILayout.BeginScrollView(mAfterScrollPos, GUI.skin.box);
                switch (eSelected) {
                    case EnumSelected.Add:
                        mAfterObjNames = AddName();
                        break;
                    case EnumSelected.Replace:
                        mAfterObjNames = ReplaceName();
                        break;
                    case EnumSelected.SerialNum:
                        mAfterObjNames = AddSerialNum();
                        break;
                    case EnumSelected.Delete:
                        mAfterObjNames = DeleteName();
                        break;
                    default:
                        break;
                }

                foreach (var objName in mAfterObjNames) {
                    EditorGUILayout.LabelField(objName);
                }

                EditorGUILayout.EndScrollView();
                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.EndHorizontal();
            CommonEditorTools.EndContents();
        }
    }

}