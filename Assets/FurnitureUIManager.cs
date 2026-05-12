using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FurnitureUIManager : MonoBehaviour
{
    public static FurnitureUIManager Instance;
    public Canvas panel;
    private furniture currentFurniture;
    public FlexibleColorPicker fcp;
    public Canvas FCP;
    public Canvas MoveConfirmUI;
    public bool Dragfurniture;

    void Awake()
    {
        if (Instance == null) Instance = this;
        if (fcp != null)
        {
            fcp.onColorChange.AddListener(OnColorChanged);
            panel.enabled = false;
            FCP.enabled = false;
            MoveConfirmUI.enabled = false;
        }
    }

    // 【開啟互動選單功能】
    // 顯示家具控制面板，並根據當前選擇的家具顏色初始化調色盤數值。
    public void Show(furniture furniture)
    {
        currentFurniture = furniture;
        panel.enabled = true;
        if (fcp != null && furniture.GetComponent<SpriteRenderer>() != null)
        {
            fcp.onColorChange.RemoveListener(OnColorChanged);
            fcp.color = furniture.GetComponent<SpriteRenderer>().color;
            fcp.onColorChange.AddListener(OnColorChanged);
        }
    }

    // 【調色盤切換功能】
    // 隱藏主選單並開啟顏色選擇介面，供使用者修改家具視覺外觀。
    public void ColorPanel()
    {
        panel.enabled = false;
        FCP.enabled = true;
    }

    // 【移動模式切換功能】
    // 開啟家具拖動權限並顯示確認 UI，允許使用者在場景中重新佈置物件位置。
    public void Move()
    {
        Dragfurniture = true;
        MoveConfirmUI.enabled = true;
        panel.enabled = false;
    }

    // 【介面關閉功能】
    // 關閉調色盤介面，結束顏色編輯狀態。
    public void CloseColorPanel()
    {
        FCP.enabled = false;
    }

    // 【位置確認功能】
    // 關閉移動權限與確認 UI，完成家具的位置擺放。
    public void MoveConfirm()
    {
        Dragfurniture = false;
        MoveConfirmUI.enabled = false;
    }

    // 【顏色即時同步功能】
    // 當調色盤數值變動時，即時更新目標家具的顏色表現。
    public void OnColorChanged(Color newColor)
    {
        if (currentFurniture != null)
        {
            currentFurniture.SetColor(newColor);
        }
    }
}