using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform player; // Referensi ke transform player

    void Update()
    {
        // Ambil posisi X player, tapi batasi minimum di 0
        float targetX = Mathf.Max(0f, player.position.x);
        
        // Camera mengikuti posisi X dan Y player dengan batasan X, Z tetap
        transform.position = new Vector3(targetX, player.position.y, transform.position.z);
    }
}
