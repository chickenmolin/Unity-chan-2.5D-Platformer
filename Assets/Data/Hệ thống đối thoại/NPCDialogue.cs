using UnityEngine;
using System.Collections.Generic;
using System.Collections;



[CreateAssetMenu(fileName = "NewNPCDialogue", menuName = "NPC Dialogue")] // Tạo tệp dữ liệu dialogue       
public class NPCDialogue : ScriptableObject // Tạo tệp dữ liệu dialogue
{
    [Header("Basic Info")]
    public string npcName; // Tên NPC
    public Sprite npcPortrait; // Hình ảnh NPC
    [TextArea(2, 5)]
    public string[] dialogueLines; // Dòng hội thoại

    [Header("Dialogue Settings")]
    public float typingSpeed = 0.05f; // Tốc độ gõ
    public AudioClip voiceSound; // Âm thanh NPC
    [Range(0.5f, 2f)]
    public float voicePitch = 1f; // Độ cao âm thanh
    public bool[] autoProgressLines; // Tự động tiến hành dòng hội thoại
    public float autoProgressDelay = 1.5f; // Thời gian chờ để tự động tiến hành dòng hội thoại

    [Header("3D Settings")]
    public Transform npcTransform; // Vị trí NPC trong scene
    public float triggerRadius = 2f; // Khoảng cách để kích hoạt
    public Animator npcAnimator; // Animator để chạy animation
    public string talkAnimationTrigger = "Talk"; // Tên trigger animation
    public Transform cameraFocusPoint; // Điểm camera nhìn khi hội thoại
}
