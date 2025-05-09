using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player; // 플레이어 캐릭터
    public Vector3 offset = new Vector3(0, 10, -5); // 카메라 위치 오프셋
    public float smoothSpeed = 5f; // 부드러운 이동 속도
    public Quaternion cameraPOV;

    void LateUpdate()
    {
        if (player == null)
            if (GameManager.Instance.player != null)
                player = GameManager.Instance.player.transform;

        if (player == null) return;
        // 플레이어의 위치 + 카메라 오프셋 적용
        Vector3 targetPosition = player.position + offset;

        // 부드럽게 이동 (Lerp 사용)
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);

        // 완전한 탑뷰 시점 고정
        transform.rotation = cameraPOV;
    }
}
