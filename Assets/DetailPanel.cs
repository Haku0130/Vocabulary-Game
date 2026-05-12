using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DetailPanel : MonoBehaviour
{
    public static DetailPanel Instance;
    public TextMeshProUGUI wordText, transText;
    public Toggle markToggle;
    public Slider familiaritySlider;

    private VocabData currentData;

    void Awake() { Instance = this; }

    // 【顯示單字資料功能】
    // 將選定的單字數據載入面板，更新介面上的文字、標記狀態與熟練度進度條。
    public void Show(VocabData data)    //顯示單字資料
    {
        currentData = data;
        gameObject.SetActive(true);

        wordText.text = data.word;
        transText.text = data.combinedDescription;
        markToggle.isOn = data.isMarked;
        familiaritySlider.value = data.familiarity;
    }

    // 【數據同步與回傳功能】
    // 當使用者在 UI 上修改狀態時，將變動同步回原始數據對象，並觸發全局存檔。
    public void UpdateData()
    {
        currentData.isMarked = markToggle.isOn;
        currentData.familiarity = (int)familiaritySlider.value;

        // 每次修改後自動存檔
        VocabManager.Instance.dataManager.GlobalSave();
    }
}
