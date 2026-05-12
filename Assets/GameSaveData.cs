using System.Collections.Generic;
using UnityEngine;

// 【家具序列化資料結構】
// 定義單一家具物件需要被持久化儲存的關鍵屬性，包含識別碼、空間座標與外觀顏色。
[System.Serializable]
public class FurnitureData
{
    public string furnitureID;
    public Vector3 position;
    public Vector3 rotation;
    public string colorHex;
}

// 【全局遊戲存檔模型】
// 整合遊戲中所有需要儲存的數據類型，作為 JSON 序列化時的數據載體，確保存取檔的一致性。
[System.Serializable]
public class GameSaveData
{
    [Header("寵物數值")]
    public int level;
    public int maxExp;
    public int currentExp;
    public int currentStamina;
    public int gold;

    [Header("外觀與環境")]
    public string themeColorHex; // 用十六進位存顏色 (例如 "#FF0000")
    public List<FurnitureData> furnitureList = new List<FurnitureData>();

    [Header("單字進度")]
    public List<VocabData> vocabProgress = new List<VocabData>();
}