using UnityEngine;

public class DialogueTriggerZone : MonoBehaviour
{
    public NPC npc; // Tham chiếu tới NPC muốn nói chuyện

    private void OnTriggerEnter(Collider other) // 3D
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player đã vào vùng hội thoại: " + gameObject.name);

            if (npc != null)
            {
                npc.Interact();
                Debug.Log("Đã gọi NPC.Interact() cho NPC: " + npc.name);
            }
            else
            {
                Debug.LogWarning("Chưa gán NPC vào DialogueTriggerZone!");
            }
        }
    }
}