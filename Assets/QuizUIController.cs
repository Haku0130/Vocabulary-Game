using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using TMPro;

public class QuizUIController : MonoBehaviour
{
    public TMPro.TextMeshProUGUI questionText;
    public Button[] optionButtons;
    public QuizFlowManager flowManager;
    public ASelectUI selectUI;
    public GameObject NextQuestion;

    public VocabData currentCorrect;

    // 【題目顯示功能】
    // 將指定的單字數據呈現於介面，並動態生成包含正確答案與干擾項的隨機選項按鈕。
    public void DisplayQuestion(VocabData data, List<VocabData> allWords)
    {
        currentCorrect = data;
        questionText.text = data.word;

        // 【選項生成邏輯】
        // 優先從相同難度的單字庫中選取 3 個錯誤項（Distractors），以增加測驗的辨識難度與有效性。
        List<string> options = new List<string> { data.combinedDescription };

        var distractors = allWords
            .Where(v => v.word != data.word)
            .OrderBy(v => v.difficulty == data.difficulty ? 0 : 1) // 優先排序同難度單字
            .ThenBy(v => Random.value)
            .Take(3)
            .Select(v => v.combinedDescription);

        options.AddRange(distractors);
        options = options.OrderBy(x => Random.value).ToList(); // 隨機洗牌以確保正確答案位置不固定

        // 【按鈕行為初始化】
        // 遍歷所有按鈕進行外觀重置，並動態綁定點擊事件，觸發答案判定與下一題按鈕的顯示邏輯。
        for (int i = 0; i < 4; i++)
        {
            int captureIndex = i;
            string answerText = options[i];

            // 更新 UI 視覺狀態
            var btnText = optionButtons[i].GetComponentInChildren<TextMeshProUGUI>();
            optionButtons[i].image.color = Color.white;
            if (btnText != null) btnText.text = answerText;

            // 清除並重新註冊點擊監聽，確保事件執行的唯一性
            optionButtons[i].onClick.RemoveAllListeners();
            optionButtons[i].onClick.AddListener(() => flowManager.OnAnswer(captureIndex));
            optionButtons[i].onClick.AddListener(() => selectUI.Show(NextQuestion));
        }
    }
}