using UnityEngine;
using System.IO;
using UnityEngine.InputSystem.LowLevel;

public class DataManager : MonoBehaviour
{
    public PetStats petStats;
    public FurnitureManager furnitureManager;
    public VocabManager vocabManager;

    // 定義存檔檔案在電腦或手機裡的具體位置
    private string SavePath => Path.Combine(Application.persistentDataPath, "global_save.json");

    // 【存檔功能】
    // 把寵物狀態、家具擺設、單字學習進度全部打包，
    // 轉成文字格式 (JSON) 後寫入檔案。
    public void GlobalSave()
    {
        GameSaveData masterData = new GameSaveData();

        petStats.PackData(masterData);

        furnitureManager.SaveFurnitures(masterData);


        foreach (var v in vocabManager.masterLibrary.Values)
        {
            masterData.vocabProgress.Add(v);
        }

        string json = JsonUtility.ToJson(masterData, true);
        File.WriteAllText(SavePath, json);
        Debug.Log("所有遊戲進度已存入全域存檔");
    }

    // 【讀檔功能】
    // 檢查有沒有存檔，有的話就把檔案讀進來，
    // 並依照裡面的內容還原寵物數值、更新單字熟練度、把家具擺回正確位置。
    public void GlobalLoad()
    {
        if (File.Exists(SavePath))
        {
            string json = File.ReadAllText(SavePath);
            GameSaveData masterData = JsonUtility.FromJson<GameSaveData>(json);

            // 1. 恢復寵物數值
            petStats.UnpackData(masterData);

            // 2. 恢復單字資料 (覆蓋原本從 CSV 讀入的初始值)
            foreach (var v in masterData.vocabProgress)
            {
                if (vocabManager.masterLibrary.ContainsKey(v.word))
                {
                    vocabManager.masterLibrary[v.word].mistakeCount = v.mistakeCount;
                    vocabManager.masterLibrary[v.word].familiarity = v.familiarity;
                    vocabManager.masterLibrary[v.word].isMarked = v.isMarked;
                }
            }

            // 3. 恢復家具資料
            furnitureManager.LoadFurnitures(masterData);
        }
    }

    // 【重置功能】(測試用)
    // 直接把存檔檔案刪除，讓遊戲回到最初始的狀態。
    [ContextMenu("Reset Game Data")]
    public void ResetData()
    {
        if (File.Exists(SavePath))
        {
            File.Delete(SavePath);
            Debug.Log("<color=yellow>存檔已刪除！請重啟遊戲以套用初始設定。</color>");
        }
        else
        {
            Debug.Log("找不到存檔檔案，無需重置。");
        }
    }
}
