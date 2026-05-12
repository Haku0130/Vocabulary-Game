using JetBrains.Annotations;
using UnityEngine;

public class FurnitureManager : MonoBehaviour
{
    public Transform furnitureRoot;

    // 【儲存家具資料功能】
    // 遍歷場景中的所有家具物件，將其名稱、位置、旋轉以及顏色數值紀錄到資料結構中。
    public void SaveFurnitures(GameSaveData data)
    {
        data.furnitureList.Clear();

        foreach (Transform child in furnitureRoot)
        {
            FurnitureData fData = new FurnitureData();
            fData.furnitureID = child.name.Replace("(Clone)", "").Trim();
            fData.position = child.localPosition;
            fData.rotation = child.localEulerAngles;

            SpriteRenderer sr = child.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                // 將 Color 轉換為 #RRGGBB 字串以方便資料紀錄
                fData.colorHex = "#" + ColorUtility.ToHtmlStringRGB(sr.color);
            }

            data.furnitureList.Add(fData);
        }
    }

    // 【讀取家具資料功能】
    // 清除當前場景物件，並根據存檔清單重新從資源庫載入模型，還原其空間坐標與視覺外觀。
    public void LoadFurnitures(GameSaveData data)
    {
        // 1. 先把現場現有的家具清空，避免重複生成
        foreach (Transform child in furnitureRoot)
        {
            Destroy(child.gameObject);
        }

        // 2. 開始根據存檔清單生成家具
        foreach (FurnitureData fData in data.furnitureList)
        {
            GameObject prefab = Resources.Load<GameObject>("Furnitures/" + fData.furnitureID);

            if (prefab != null)
            {
                // 生成家具，並設為 furnitureRoot 的子物件
                GameObject newFurniture = Instantiate(prefab, furnitureRoot);

                // 把名字改回 ID，確保下次存檔時名稱辨識正確
                newFurniture.name = fData.furnitureID;

                // 還原位置與旋轉
                newFurniture.transform.localPosition = fData.position;
                newFurniture.transform.localEulerAngles = fData.rotation;

                if (!string.IsNullOrEmpty(fData.colorHex))
                {
                    Color myColor;
                    // 將字串轉回 Unity 的 Color 物件並套用
                    if (ColorUtility.TryParseHtmlString(fData.colorHex, out myColor))
                    {
                        SpriteRenderer sr = newFurniture.GetComponent<SpriteRenderer>();
                        if (sr != null)
                        {
                            sr.color = myColor;
                        }
                    }
                }
            }
            else
            {
                Debug.LogWarning($"[Furniture] 找不到家具 Prefab: {fData.furnitureID}");
            }
        }
        Debug.Log("<color=green>家具配置已載入！</color>");
    }
}