using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(HeartFork_Manager))]
public class HeartFork_ManagerEditor : Editor
{
    // オブジェクトを複製する数を指定するための変数
    int numberOfCopies = 10;

    public override void OnInspectorGUI()
    {
        // デフォルトのインスペクタを描画
        DrawDefaultInspector();

        // スクリプトのターゲットを取得
        HeartFork_Manager script = (HeartFork_Manager)target;

        // 複製する数を指定するスライダー
        numberOfCopies = EditorGUILayout.IntSlider("複製する数", numberOfCopies, 1, 100);

        if (GUILayout.Button("オブジェクトを複製して割り当て"))
        {
            // 親オブジェクトの Transform を取得
            Transform parentTransform = script._pool;

            if (parentTransform == null)
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
            for (int i = parentTransform.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(parentTransform.GetChild(i).gameObject);
            }

            // 複製したオブジェクトを格納する配列を初期化
            HeartFork_PickupSub[] generatedObjects = new HeartFork_PickupSub[numberOfCopies];

            // 指定された数だけプレハブを複製
            for (int i = 0; i < numberOfCopies; i++)
            {
                // プレハブを複製
                GameObject clone = Instantiate(script._prefab, parentTransform);
                clone.name = script._prefab.name + "_Copy_" + (i + 1); // 名前を変更
                generatedObjects[i] = clone.GetComponent<HeartFork_PickupSub>();
            }

            // UdonSharpスクリプトの _objs 配列に複製されたオブジェクトを割り当て
            script._objs = generatedObjects;

            // スクリプトの変更をマークして保存可能にする
            EditorUtility.SetDirty(script);

            Debug.Log(numberOfCopies + "個のオブジェクトを複製して配列に割り当てました。");
        }
    }
}