using UnityEngine;
using UnityEngine.UI;

public class PetStats : MonoBehaviour
{
    [Header("基礎屬性")]
    public string petName = "小幫手";
    public int level = 1;
    public int gold = 0;

    [Header("經驗值系統")]
    public int currentExp = 0;
    public int maxExp = 20;

    [Header("體力系統")]
    public int currentStamina = 20;
    public int maxStamina = 20;

    [Header("外觀顯示")]
    public SpriteRenderer petRenderer;
    public Sprite[] StageSprite;

    private void Start()
    {

    }

    // 【金幣處理功能】
    // 增加玩家持有的金幣數量，並同步記錄於系統數值中。
    public void AddGold(int amount)
    {
        gold += amount;
        Debug.Log($"獲得金幣: {amount}，目前總金幣: {gold}");
    }

    // 【經驗值與升級邏輯】
    // 累加經驗值並檢查是否達到門檻，支援單次獲得大量經驗時連續升級的判定。
    public void AddExp(int amount)
    {
        currentExp += amount;
        Debug.Log($"獲得經驗值: {amount}");

        while (currentExp >= maxExp)
        {
            LevelUp();
        }
    }

    // 【等級提升功能】
    // 執行升級程序：扣除經驗值、提升等級、增加下一級門檻，並根據新等級更新寵物外觀。
    private void LevelUp()
    {
        currentExp -= maxExp;
        level++;
        maxExp = Mathf.RoundToInt(maxExp * 2);
        petRenderer.sprite = StageSprite[level - 1];

        Debug.Log($"恭喜！等級提升至: {level}！");
    }

    // 【體力消耗判斷功能】
    // 檢查體力是否足夠執行活動，若足夠則扣除並回傳成功，否則攔截請求。
    public bool ConsumeStamina(int amount)
    {
        if (currentStamina >= amount)
        {
            currentStamina -= amount;
            Debug.Log($"消耗體力: {amount}，剩餘: {currentStamina}");
            return true;
        }
        else
        {
            Debug.LogWarning("體力不足！");
            return false;
        }
    }

    // 【體力恢復功能】
    // 增加當前體力值，但確保數值不會超過系統設定的體力上限。
    public void RestoreStamina(int amount)
    {
        currentStamina = Mathf.Min(currentStamina + amount, maxStamina);
        Debug.Log($"恢復體力: {amount}，目前體力: {currentStamina}");
    }

    // 【數據封裝功能】
    // 將當前寵物的各項數值狀態打包進存檔結構中，準備進行持久化儲存。
    public void PackData(GameSaveData data)
    {
        data.level = this.level;
        data.maxExp = this.maxExp;
        data.currentExp = this.currentExp;
        data.currentStamina = this.currentStamina;
        data.gold = this.gold;
    }

    // 【數據還原功能】
    // 從存檔結構中提取數值並覆蓋當前狀態，同時確保視覺外觀依據等級正確呈現。
    public void UnpackData(GameSaveData data)
    {
        this.level = data.level;
        this.maxExp = data.maxExp;
        this.currentExp = data.currentExp;
        this.currentStamina = data.currentStamina;
        this.gold = data.gold;
        petRenderer.sprite = StageSprite[level - 1];
    }
}