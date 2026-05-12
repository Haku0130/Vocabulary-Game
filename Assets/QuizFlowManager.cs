using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class QuizFlowManager : MonoBehaviour
{
    [Header("關聯腳本")]
    public QuizBuilder quizBuilder;
    public QuizUIController quizUI;
    public ActivityController activityManager;
    public VocabManager vocabManager;
    public DataManager dataManager;
    public PetStats petStats;

    [Header("UI 面板")]
    public GameObject prepPanel;
    public GameObject quizPanel;
    public GameObject resultPanel;

    [Header("準備面板顯示")]
    public TMPro.TextMeshProUGUI resultGoldText;
    public TMPro.TextMeshProUGUI resultExpText;
    public TMPro.TextMeshProUGUI resultCorrectCountText;
    public TMPro.TextMeshProUGUI resultInCorrectCountText;

    private Queue<VocabData> questionQueue = new Queue<VocabData>();
    private List<VocabData> currentSessionWords = new List<VocabData>();
    private int totalQuestions;
    private int correctCount;
    public string currentMode;

    // 【測驗啟動功能】
    // 初始化測驗佇列，檢查體力限制並切換至測驗介面，開啟答題流程。
    public void StartQuizSession()
    {
        var selected = quizBuilder.GetFinalPool();
        if (selected.Count == 0) return;

        // 體力檢核邏輯：非遊玩模式下需消耗體力方可開始
        if (currentMode != "Play" && petStats.currentStamina < quizBuilder.GetFinalPool().Count) return;

        currentSessionWords = new List<VocabData>(selected);
        questionQueue = new Queue<VocabData>(selected);
        totalQuestions = questionQueue.Count;
        correctCount = 0;

        prepPanel.SetActive(false);
        quizPanel.SetActive(true);

        NextQuestion();
    }

    // 【題目發送功能】
    // 從佇列中提取下一筆單字數據並更新 UI；若佇列已空則觸發結算流程。
    public void NextQuestion()
    {
        if (questionQueue.Count > 0)
        {
            VocabData next = questionQueue.Dequeue();
            quizUI.DisplayQuestion(next, vocabManager.GetFullList());
        }
        else
        {
            EndQuiz();
        }
    }

    // 【答題回饋處理功能】
    // 判斷玩家選擇是否正確，即時更新按鈕顏色視覺回饋，並記錄答錯次數與難度權重。
    public void OnAnswer(int index)
    {
        string selectedAnswer = quizUI.optionButtons[index].GetComponentInChildren<TextMeshProUGUI>().text;
        bool isCorrect = (selectedAnswer == quizUI.currentCorrect.combinedDescription);

        for (int i = 0; i < 4; i++)
        {
            var btnText = quizUI.optionButtons[i].GetComponentInChildren<TextMeshProUGUI>();

            // 視覺回饋：正確答案恆顯示綠色
            if (btnText.text == quizUI.currentCorrect.combinedDescription)
            {
                quizUI.optionButtons[i].image.color = Color.green;
            }

            // 選取判斷：答對累加點數，答錯則記錄錯誤次數並顯示紅色提示
            if (isCorrect && i == index)
            {
                quizUI.optionButtons[i].image.color = Color.green;
                correctCount++;
                activityManager.DifficultyPoint += quizUI.currentCorrect.difficulty;
            }
            else if (!isCorrect && i == index)
            {
                quizUI.optionButtons[i].image.color = Color.red;
                quizUI.currentCorrect.mistakeCount++;
            }
        }
    }

    // 【測驗結算功能】
    // 計算最終獎勵回饋，扣除對應體力，並將所有變更即時持久化儲存至檔案。
    private void EndQuiz()
    {
        quizPanel.SetActive(false);

        // 調用獎勵算法並更新結算面板資訊
        activityManager.CalculateRewards(currentSessionWords, correctCount, currentMode);
        resultCorrectCountText.text = "答對題數:" + correctCount.ToString();
        resultInCorrectCountText.text = "答錯題數:" + (currentSessionWords.Count - correctCount).ToString();

        resultPanel.SetActive(true);
        petStats.currentStamina -= currentSessionWords.Count;

        // 全局存檔更新
        dataManager.GlobalSave();
    }
}