using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WholeCake_PickupMain))]
public class WholeCake_PickupMainEditor : Editor
{
    // オブジェクトを複製する数を指定するための変数
    int numberOfCopies = 50;
    WholeCake_PickupMain script;

    public override void OnInspectorGUI()
    {
        // デフォルトのインスペクタを描画
        DrawDefaultInspector();

        // スクリプトのターゲットを取得
        script = (WholeCake_PickupMain)target;

        if (GUILayout.Button("オブジェクトを複製して割り当て"))
        {
            FuncClone(script._trans0);
            FuncClone(script._trans1);

            // スクリプトの変更をマークして保存可能にする
            EditorUtility.SetDirty(script);

            Debug.Log(numberOfCopies + "個のオブジェクトを複製して配列に割り当てました。");
        }
    }

    public void FuncClone(Transform trans)
    {
        if (trans == null)
        {
            Debug.LogError("親オブジェクトが指定されていません。");
            return;
        }

        // プレハブが指定されているかチェック
        if (script._prefab == null)
        {
            Debug.LogError("Prefabが指定されていません。");
            return;
        }

        // すべての子オブジェクトを削除
        for (int i = trans.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(trans.GetChild(i).gameObject);
        }

        // 指定された数だけプレハブを複製
        for (int i = 0; i < numberOfCopies; i++)
        {
            // プレハブを複製
            GameObject clone = Instantiate(script._prefab, trans);
            CakeCandleGimmick cakeCandleGimmick = clone.GetComponent< CakeCandleGimmick>();
            cakeCandleGimmick._wcpm = script;
            clone.name = script._prefab.name + "_Copy_" + (i + 1); // 名前を変更
        }
    }
}