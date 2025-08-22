using UnityEngine;

public class MoveObject : MonoBehaviour
{
    public enum Axis { X, Y, Z }               // Thêm trục Z
    public Axis moveAxis = Axis.Y;             // Mặc định là trục Y

    public float speed = 1.0f;                 // Tốc độ di chuyển
    public float distance = 2.0f;              // Biên độ di chuyển (qua lại / lên xuống)

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;         // Lưu vị trí ban đầu
    }

    void Update()
    {
        float movement = Mathf.Sin(Time.time * speed) * distance;

        Vector3 newPosition = startPos;

        switch (moveAxis)
        {
            case Axis.X:
                newPosition.x += movement;
                break;
            case Axis.Y:
                newPosition.y += movement;
                break;
            case Axis.Z:
                newPosition.z += movement;
                break;
        }

        transform.position = newPosition;      // Gán vị trí mới cho object
    }
}
