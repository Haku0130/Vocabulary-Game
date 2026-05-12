using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using System.Text;

// 【單字實體資料結構】
// 定義單字的屬性欄位，包含拼字、翻譯、難度、標記狀態、熟練度及錯誤紀錄。
[System.Serializable]
public class VocabData
{
    public string word;
    public string combinedDescription;
    public int difficulty;
    public bool isMarked;
    public int familiarity;
    public int mistakeCount;

    public VocabData(string word, string desc, int level)
    {
        this.word = word;
        this.combinedDescription = desc;
        this.difficulty = level;
        this.isMarked = false;
        this.familiarity = 0;
    }
}

public class VocabManager : MonoBehaviour
{
    public static VocabManager Instance;
    public DataManager dataManager;

    // 使用 Dictionary 確保單字唯一性，並快取清單以優化效能
    public Dictionary<string, VocabData> masterLibrary = new Dictionary<string, VocabData>();
    private List<VocabData> cachedList = new List<VocabData>();

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        LoadAllLevels();
    }

    // 【單字庫初始化功能】
    // 遍歷 Resources 中不同等級的 CSV 檔案，將所有單字載入內存並觸發全局進度讀取。
    void LoadAllLevels()
    {
        masterLibrary.Clear();

        for (int i = 1; i <= 6; i++)
        {
            string fileName = $"Vocab/senior_lv{i}";
            TextAsset csvFile = Resources.Load<TextAsset>(fileName);

            if (csvFile != null)
            {
                ParseCSV(csvFile.text, i);
            }
            else
            {
                Debug.LogWarning($"找不到檔案: {fileName}");
            }
        }

        dataManager.GlobalLoad();
        Debug.Log($"<color=cyan>單字庫載入完成！</color> 最終不重複單字數: {masterLibrary.Count}");
    }

    // 【CSV 文本解析功能】
    // 運用正規表達式與字串處理技術，將原始文本拆解為單字、詞性與翻譯，並處理重複單字的語義整合。
    void ParseCSV(string content, int level)
    {
        string[] lines = content.Split(new[] { "\r\n", "\r", "\n" }, System.StringSplitOptions.RemoveEmptyEntries);

        foreach (string line in lines)
        {
            try
            {
                string cleanedLine = line.Replace("\"", "").Trim();
                if (string.IsNullOrEmpty(cleanedLine)) continue;

                string[] atSplit = cleanedLine.Split('@');
                if (atSplit.Length < 2) continue;

                string word = atSplit[0].Trim();
                string rest = atSplit[1];

                // 拆分詞性與翻譯
                string[] bracketSplit = rest.Split('(', ')');
                string rawPos = bracketSplit.Length > 1 ? bracketSplit[1].Trim() : "";
                string translation = bracketSplit.Length > 2 ? bracketSplit[2].Trim() : rest.Trim();

                // 使用 Regex 移除詞性中的雜訊數字
                string pos = Regex.Replace(rawPos, @"\d", "");
                string infoToAdd = string.IsNullOrEmpty(pos) ? translation : $"({pos}) {translation}";

                // 整合與儲存邏輯：若單字已存在則疊加描述，不存在則新增實體
                if (masterLibrary.ContainsKey(word))
                {
                    if (!masterLibrary[word].combinedDescription.Contains(infoToAdd))
                    {
                        masterLibrary[word].combinedDescription += " / " + infoToAdd;
                    }
                }
                else
                {
                    masterLibrary.Add(word, new VocabData(word, infoToAdd, level));
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"解析失敗: {line}，錯誤: {e.Message}");
            }
        }
        cachedList = new List<VocabData>(masterLibrary.Values);
    }

    // 【數據檢索接口】
    // 提供隨機抽題、全字典獲取及全清單快取獲取的功能，供外部系統進行數據處理。
    public VocabData GetRandomWord()
    {
        if (masterLibrary.Count == 0) return null;
        List<string> keys = new List<string>(masterLibrary.Keys);
        return masterLibrary[keys[Random.Range(0, keys.Count)]];
    }

    public Dictionary<string, VocabData> GetFullDict() => masterLibrary;
    public List<VocabData> GetFullList() => cachedList;
}