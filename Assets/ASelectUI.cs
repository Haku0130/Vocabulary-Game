using UnityEngine;

public class ASelectUI : MonoBehaviour
{
    // 【介面顯隱功能】
    // 控制特定 UI 物件的開啟與關閉，實現畫面切換。
    public void Show(GameObject obj)
    {
        obj.SetActive (true);
    }
    public void Close(GameObject obj)
    {
        obj.SetActive (false);
    }
}
