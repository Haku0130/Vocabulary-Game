using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VocabUIController : MonoBehaviour
{
    public VocabManager vocabManager;
    public GameObject vocLibrary;
    public GameObject detailpanel;
    public GameObject WordSheet;
    public GameObject wordItemPrefab;
    public Transform contentParent;
    public TMP_InputField searchBar;
    public List<VocabData> filteredList = new List<VocabData>();
    public LoopVerticalScrollRect loopScroll;

    private int currentLevel = 1;

    // 【單字庫開關功能】
    // 控制單字庫主介面的顯示與隱藏。
    public void ShowVoc()
    {
        vocLibrary.SetActive(true);
    }

    public void CloseVoc()
    {
        vocLibrary.SetActive(false);
    }

    // 【難度層級選擇功能】
    // 根據使用者選取的難度分級（Level）切換顯示內容，並觸發列表刷新。
    public void SelectDifficulty(int level)
    {
        WordSheet.SetActive(true);
        currentLevel = level;
        RefreshList();
    }

    // 【詳細資訊關閉功能】
    // 隱藏單字詳細內容面板。
    public void CloseDetail()
    {
        detailpanel.SetActive(false);
    }

    // 【搜尋觸發功能】
    // 監聽輸入框的字串變動，即時將關鍵字傳遞給列表刷新邏輯以進行過濾。
    public void OnSearchChanged()
    {
        RefreshList(searchBar.text);
        Debug.Log(searchBar.text);
    }

    // 【資料快取接口】
    // 提供給無限滾動組件（LoopScrollSource）獲取當前過濾後資料集的管道。
    public List<VocabData> GetCurrentDataList() => filteredList;

    // 【列表動態刷新功能】
    // 核心邏輯：結合難度分級與搜尋關鍵字進行資料篩選，並重置無限滾動清單的顯示狀態與數據總量。
    public void RefreshList(string filter = "")
    {
        // 1. 數據檢索：利用 LINQ 進行複合條件篩選（難度匹配且符合開頭字母）
        filteredList = vocabManager.masterLibrary.Values
            .Where(v => v.difficulty == currentLevel)
            .Where(v => string.IsNullOrEmpty(filter) || v.word.StartsWith(filter.ToLower()))
            .ToList();

        // 2. UI 狀態重置：停止清單滾動慣性，確保介面更新時不會產生偏移
        loopScroll.StopMovement();

        // 3. 虛擬化清單同步：更新無限滾動組件的數據總量
        loopScroll.totalCount = filteredList.Count;

        // 4. 視圖重構：從首項開始重新填充儲存格單元
        loopScroll.RefillCells(0);
        Debug.Log("單字列表已刷新，目前符合條件總數：" + filteredList.Count);
    }

    // 【詳細面板開啟功能】
    // 顯示指定單字的詳細資訊面板，並將單字實體傳遞給詳情組件進行數據綁定。
    public void OpenDetail(VocabData data)
    {
        detailpanel.SetActive(true);
        DetailPanel.Instance.Show(data);
    }
}