using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using System.Linq;

public class ActivityController : MonoBehaviour
{
    public QuizBuilder quizBuilder;
    public QuizFlowManager quizFlowManager;
    public PetStats pet;

    public int DifficultyPoint;

    // 【情境切換功能】
    // 根據使用者行為變更系統操作情境，確保後續邏輯切換到正確的模式。
    public void changeMode(string md)
    {
        quizFlowManager.currentMode = md;
    }

    // 【數據轉化運算】
    // 將測驗表現指標轉換為系統資產，計算該次活動獲得的基礎獎勵。
    public void CalculateRewards(List<VocabData> answeredWords, int correctCount, string type)
    {
        float baseReward;
        if (correctCount == 0) baseReward = 0f; 
        else baseReward = (10 + 2 * (DifficultyPoint / correctCount - 1)) * correctCount;

        // 【資訊分類分發】
        // 依據活動類別標籤分配獎勵權重，將數值存入寵物系統中。
        switch (type)
        {
            case "Work":
                pet.AddGold(Mathf.RoundToInt(baseReward * 2));
                pet.AddExp(Mathf.RoundToInt(baseReward * 1));
                break;
            case "Train":
                pet.AddExp(Mathf.RoundToInt(baseReward * 3));
                break;
            case "Play":
                pet.currentExp += Mathf.RoundToInt(baseReward * 0.5f);
                break;
        }
    }
}
