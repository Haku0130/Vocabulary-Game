using UnityEngine;
using System.Collections;

public class furniture : MonoBehaviour
{
    [Header("移動限制")]
    public bool isLimitMovement = false;
    public Vector2 moveMin;
    public Vector2 moveMax;

    [Header("長按設定")]
    public float longPressTime = 0.5f;

    private SpriteRenderer spriteRenderer;
    private Vector3 offset;
    private float pressTimer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // 【座標偏移計算】
    // 在點擊瞬間記錄手指與物體中心的距離，確保拖動時物件不會突然跳轉位移。
    void OnMouseDown()
    {
        pressTimer = Time.time;

        // 計算手指與物體中心的偏移，防止物體中心直接跳到手指位置
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        offset = transform.position - new Vector3(mousePos.x, mousePos.y, 0);

        // 開啟長按檢查的協程
        StartCoroutine(CheckLongPress());
    }

    // 【拖放移動功能】
    // 根據手指位置同步移動家具，並根據設定決定是否限制家具只能在特定區域內移動。
    void OnMouseDrag()
    {
        if (FurnitureUIManager.Instance.Dragfurniture)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 targetPos = new Vector3(mousePos.x, mousePos.y, 0) + offset;

            if (isLimitMovement)
            {
                targetPos.x = Mathf.Clamp(targetPos.x, moveMin.x, moveMax.x);
                targetPos.y = Mathf.Clamp(targetPos.y, moveMin.y, moveMax.y);
            }

            transform.position = targetPos;
        }
    }

    // 【輸入狀態終止】
    // 當手指放開時，停止所有的計時器或檢查程序。
    void OnMouseUp()
    {
        StopAllCoroutines();
    }

    // 【長按判定功能】
    // 檢查使用者是否按住物件超過指定時間，若成功則觸發選單開啟邏輯。
    IEnumerator CheckLongPress()
    {
        if (FurnitureUIManager.Instance.Dragfurniture == false) { 
        yield return new WaitForSeconds(longPressTime);

        OpenMenu();
        }
    }

    // 【選單呼叫功能】
    // 通知界面管理器開啟該家具的專屬互動選單。
    void OpenMenu()
    {
        Debug.Log("長按成功，開啟家具選單");

        if (FurnitureUIManager.Instance != null)
        {
            FurnitureUIManager.Instance.Show(this);
        }
    }

    // 【外觀變更接口】
    // 提供外部程式修改家具顏色或濾鏡的功能。
    public void SetColor(Color color)
    {
        if (spriteRenderer != null) spriteRenderer.color = color;
    }
}