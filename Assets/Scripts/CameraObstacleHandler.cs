//using System.Collections.Generic;
//using UnityEngine;

//public class CameraObstacleHandler : MonoBehaviour
//{
//    private Transform player;
//    public string playerTag = "Player";  // 동적으로 생성되는 플레이어 감지
//    public LayerMask obstacleMask;       // 벽이 있는 레이어 (예: "Wall")
//    private Dictionary<MeshRenderer, bool> hiddenObjects = new Dictionary<MeshRenderer, bool>();

//    void Update()
//    {
//        // 플레이어가 동적으로 생성되었는지 확인하고 찾아서 할당
//        if (player == null)
//        {
//            GameObject foundPlayer = GameObject.FindWithTag(playerTag);
//            if (foundPlayer != null)
//            {
//                player = foundPlayer.transform;
//            }
//        }

//        if (player == null) return; // 플레이어가 없으면 종료

//        // 기존에 숨겨진 벽을 다시 보이게 설정
//        foreach (var entry in hiddenObjects)
//        {
//            if (entry.Key != null)
//            {
//                entry.Key.enabled = true;
//            }
//        }
//        hiddenObjects.Clear();

//        // 카메라와 플레이어 사이의 벽 감지
//        Vector3 direction = player.position - transform.position;
//        float distance = Vector3.Distance(transform.position, player.position);

//        RaycastHit[] hits = Physics.RaycastAll(transform.position, direction.normalized, distance, obstacleMask);

//        float closestDistance = Mathf.Infinity;  // 가장 가까운 벽을 찾기 위한 변수
//        MeshRenderer closestWall = null;

//        foreach (RaycastHit hit in hits)
//        {
//            MeshRenderer renderer = hit.collider.GetComponent<MeshRenderer>();
//            if (renderer != null)
//            {
//                float hitDistance = Vector3.Distance(transform.position, hit.point);
//                if (hitDistance < closestDistance)
//                {
//                    closestDistance = hitDistance;
//                    closestWall = renderer;
//                }
//            }
//        }

//        // 가장 가까운 벽만 비활성화
//        if (closestWall != null)
//        {
//            closestWall.enabled = false;
//            hiddenObjects[closestWall] = true;
//        }
//    }
//}
