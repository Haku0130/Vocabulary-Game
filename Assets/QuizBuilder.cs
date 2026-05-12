using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class QuizBuilder : MonoBehaviour
{
    public int relations = 1;
    public VocabManager vocabManager;
    public TextMeshProUGUI poolCount;
    // 本次測驗的暫存池
    private List<VocabData> sessionPool = new List<VocabData>();

    // 【清空題目池功能】
    // 重置當前暫存的題目清單，準備重新選題。
    public void ClearPool() => sessionPool.Clear();

    // 【難度篩選功能】
    // 從單字庫中找出指定難度等級的單字，並加入測驗池。
    public void AddDifficulty(int lv)
    {
        var words = vocabManager.GetFullDict().Values.Where(v => v.difficulty == lv);
        sessionPool.AddRange(words);
        FinalizePool();
    }

    // 【星號標記篩選功能】
    // 篩選出所有使用者特別標記（打星號）的重點單字加入測驗池。
    public void Added()
    {
        var words = vocabManager.GetFullDict().Where(v => v.Value.isMarked).Select(v => v.Value);
        sessionPool.AddRange(words);
        FinalizePool();
    }

    // 【熟練度篩選功能】
    // 根據特定的熟悉度數值篩選單字，針對特定學習階段進行測驗。
    public void AddByFamiliarity(int fami)
    {
        var words = vocabManager.GetFullDict().Values.Where(v => v.familiarity == fami);
        sessionPool.AddRange(words);
        FinalizePool();
    }

    // 【邏輯條件變更功能】
    // 切換篩選時的運算關係（如大於、等於或小於），影響題目選取的範圍。
    public void ChangeRelations(int r)
    {
        relations = r;
    }

    // 【錯誤紀錄篩選功能】
    // 根據單字曾答錯的次數與當前邏輯條件，將容易出錯或特定錯誤頻率的單字挑選出來。
    public void AddMistakes(int threshold)
    {
        if (relations == 0)
        {
            var words = vocabManager.GetFullDict().Values.Where(v => v.mistakeCount > threshold);
            sessionPool.AddRange(words);
            FinalizePool();
        }
        else if (relations == 1)
        {
            var words = vocabManager.GetFullDict().Values.Where(v => v.mistakeCount == threshold);
            sessionPool.AddRange(words);
            FinalizePool();
        }
        else
        {
            var words = vocabManager.GetFullDict().Values.Where(v => v.mistakeCount < threshold);
            sessionPool.AddRange(words);
            FinalizePool();
        }
    }

    // 【題目池彙整功能】
    // 移除重複選取的單字並進行隨機排序，最後更新介面上的題目總數顯示。
    private void FinalizePool()
    {
        sessionPool = sessionPool.Distinct().OrderBy(x => Random.value).ToList();
        poolCount.text = "題庫裡已有:" + sessionPool.Count.ToString() + "題";
        Debug.Log($"目前題庫總數：{sessionPool.Count}");
    }

    // 【匯出題目功能】
    // 將最終篩選完成並打亂順序的題目清單回傳給測驗系統。
    public List<VocabData> GetFinalPool() => sessionPool;
}