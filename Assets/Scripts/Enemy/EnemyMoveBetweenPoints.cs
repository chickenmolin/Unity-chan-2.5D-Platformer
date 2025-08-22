using UnityEngine;
using System.Collections;

public class EnemyMoveBetweenPoints : MonoBehaviour
{
    public enum MoveAxis { X, Y, Z }

    public Transform pointA; // Điểm A
    public Transform pointB; // Điểm B
    public float speed = 2f; // Tốc độ di chuyển
    public float idleDelay = 2f; // Thời gian chờ
    public float rotationSpeed = 5f; // tốc độ xoay mặt
    public MoveAxis moveAxis = MoveAxis.X; // Chọn trục di chuyển

    private Transform targetPoint; // Điểm đích
    private bool isWaiting = false;

    void Start()
    {
        if (pointA == null || pointB == null)
        {
            Debug.LogError("Bạn chưa gán pointA hoặc pointB!");
            enabled = false;
            return;
        }
        targetPoint = pointB;
    }

    void Update()
    {
        if (isWaiting) return;

        // Lấy vị trí hiện tại và đích
        Vector3 currentPos = transform.position;
        Vector3 targetPos = targetPoint.position;

        // Tạo vị trí mới chỉ khác nhau trên trục được chọn
        switch (moveAxis)
        {
            case MoveAxis.X:
                targetPos.y = currentPos.y;
                targetPos.z = currentPos.z;
                break;
            case MoveAxis.Y:
                targetPos.x = currentPos.x;
                targetPos.z = currentPos.z;
                break;
            case MoveAxis.Z:
                targetPos.x = currentPos.x;
                targetPos.y = currentPos.y;
                break;
        }

        // Tính khoảng cách trên trục di chuyển
        float distance = Vector3.Distance(currentPos, targetPos);

        if (distance < 0.05f)
        {
            StartCoroutine(IdleBeforeSwitch());
            return;
        }

        // Di chuyển theo trục đã chọn
        Vector3 direction = (targetPos - currentPos).normalized;
        transform.position = Vector3.MoveTowards(currentPos, targetPos, speed * Time.deltaTime);

        // Xoay mặt theo hướng di chuyển (chỉ xoay quanh trục Y để tránh xoay loạn)
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            Vector3 euler = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime).eulerAngles;
            transform.rotation = Quaternion.Euler(0, euler.y, 0);
        }
    }

    IEnumerator IdleBeforeSwitch()
    {
        isWaiting = true;
        yield return new WaitForSeconds(idleDelay);
        targetPoint = (targetPoint == pointA) ? pointB : pointA;
        isWaiting = false;
    }
}
