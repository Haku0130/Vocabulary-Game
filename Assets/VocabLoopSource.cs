using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // LoopScrollRect 的命名空間

public class VocabLoopSource : MonoBehaviour, LoopScrollPrefabSource, LoopScrollDataSource
{
    public GameObject prefab;
    public VocabUIController vocabUIController;
    public VocabUIController uiController; // 引用原本的控制腳本

    private LoopVerticalScrollRect loopScroll;
    private Stack<GameObject> pool = new Stack<GameObject>();

    void Awake()
    {
        loopScroll = GetComponent<LoopVerticalScrollRect>();
        loopScroll.prefabSource = this;
        loopScroll.dataSource = this;
    }

    // 【物件池獲取功能】
    // 實作介面以管理 UI 物件的回收與再利用。優先從堆疊中提取已失效的物件，若無可用物件則生成新實例。
    public GameObject GetObject(int index)
    {
        if (pool.Count > 0)
        {
            GameObject obj = pool.Pop();
            obj.SetActive(true);
            return obj;
        }

        Debug.Log("實例化新單字物件");
        return Instantiate(prefab);
    }

    // 【物件回收功能】
    // 當 UI 物件移出可視範圍時，將其設為非活動狀態並推回物件池，減少記憶體頻繁配置的開銷。
    public void ReturnObject(Transform trans)
    {
        trans.gameObject.SetActive(false);
        pool.Push(trans.gameObject);
    }

    // 【數據綁定與填充功能】
    // 根據當前滾動索引從單字清單中提取數據，動態更新 UI 文字內容並重新綁定點擊互動事件。
    public void ProvideData(Transform transform, int index)
    {
        var list = uiController.GetCurrentDataList();
        if (index < 0 || index >= list.Count) return;

        var data = list[index];

        // 更新文字：若單字被標記為重點，則在視覺上加上星號提示
        var tmpText = transform.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        if (tmpText != null)
        {
            tmpText.text = data.isMarked ? "* " + data.word : data.word;
        }

        // 事件綁定：重置按鈕點擊事件，確保其開啟對應單字的詳細資訊面板
        var btn = transform.GetComponent<UnityEngine.UI.Button>();
        if (btn != null)
        {
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => uiController.OpenDetail(data));
        }
    }
}