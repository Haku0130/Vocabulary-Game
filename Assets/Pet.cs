using UnityEngine;
using System.Collections;

public class Pet : MonoBehaviour
{
    [Header("移動數值")]
    [Tooltip("寵物移動速度")]
    public float moveSpeed = 2.0f;
    [Tooltip("到達目的地後的停留時間範圍")]
    public float minIdleTime = 1.0f;
    public float maxIdleTime = 3.0f;

    [Header("邊界限制")]
    public float minX = -5.0f;
    public float maxX = 5.0f;
    public float minY = -3.0f;
    public float maxY = 3.0f;

    public PetStats petstats;
    private Vector2 targetPosition;
    private bool isWalking = false;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        targetPosition = transform.position;
        StartCoroutine(WanderRoutine());
    }

    void Update()
    {
        // 只有當寵物等級大於 3 時才會在行走狀態下移動
        if (isWalking && petstats.level > 3)
        {
            MovePet();
        }
    }

    // 【隨機漫遊行為功能】
    // 循環執行「選取隨機目標、開始行走、抵達等待、休息」的行為鏈，模擬生物的自然活動。
    IEnumerator WanderRoutine()
    {
        while (true)
        {
            // 目標選取
            targetPosition = new Vector2(Random.Range(minX, maxX), Random.Range(minY, maxY));
            isWalking = true;

            // 根據目標位置翻轉角色朝向
            FlipSprite(targetPosition.x);

            // 等待直到接近目標點
            yield return new WaitUntil(() => Vector2.Distance(transform.position, targetPosition) < 0.1f);

            isWalking = false;

            // 隨機休息時間
            float waitTime = Random.Range(minIdleTime, maxIdleTime);
            yield return new WaitForSeconds(waitTime);
        }
    }

    // 【位移執行功能】
    // 使用平滑插值讓寵物向目標座標移動。
    void MovePet()
    {
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }

    // 【碰撞處理功能】
    // 當寵物撞到障礙物時立即停止當前路徑，並重新尋找新的隨機目標以避免卡死。
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Interactable"))
        {
            StopAllCoroutines();
            return;
        }
        if (isWalking)
        {
            StopAllCoroutines();
            isWalking = false;
            StartCoroutine(WanderRoutine());
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Interactable")) return;
        if (isWalking)
        {
            StopAllCoroutines();
            isWalking = false;
            StartCoroutine(WanderRoutine());
        }
    }

    // 【視覺朝向翻轉功能】
    // 根據水平移動的方向自動切換 Sprite 的 X 軸翻轉狀態。
    void FlipSprite(float targetX)
    {
        if (spriteRenderer == null) return;

        if (targetX > transform.position.x)
            spriteRenderer.flipX = false;
        else
            spriteRenderer.flipX = true;
    }

    // 【邊界可視化輔助】
    // 在編輯器視窗中繪製綠色線框，定義寵物的活動範圍限制。
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Vector3 topLeft = new Vector3(minX, maxY, 0);
        Vector3 topRight = new Vector3(maxX, maxY, 0);
        Vector3 bottomLeft = new Vector3(minX, minY, 0);
        Vector3 bottomRight = new Vector3(maxX, minY, 0);

        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);
        Gizmos.DrawLine(bottomLeft, topLeft);
    }
}