using UnityEngine;
using UnityEngine.UI;

public class CloudMovement : MonoBehaviour
{
    [SerializeField] private float speed = 2f; // Kecepatan pergerakan cloud
    [SerializeField] private bool moveLeft = true; // Arah pergerakan (true = kiri, false = kanan)

    private float screenLeftBound;
    private float screenRightBound;
    private float cloudWidth;
    private RectTransform rectTransform;
    private Canvas canvas;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();

        // Untuk UI Image, gunakan Canvas sebagai referensi
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        float canvasWidth = canvasRect.rect.width;

        screenLeftBound = -canvasWidth / 2;
        screenRightBound = canvasWidth / 2;

        // Mendapatkan lebar cloud dari RectTransform
        cloudWidth = rectTransform.rect.width;

    }

    void Update()
    {
        // Pergerakan cloud sesuai arah yang dipilih
        Vector2 direction = moveLeft ? Vector2.left : Vector2.right;
        rectTransform.anchoredPosition += direction * speed * Time.deltaTime * 100f;

        // Cek boundary sesuai arah pergerakan
        if (moveLeft)
        {
            // Jika bergerak ke kiri dan keluar dari sisi kiri
            if (rectTransform.anchoredPosition.x < screenLeftBound - cloudWidth / 2)
            {
                // Reset ke sisi kanan layar
                rectTransform.anchoredPosition = new Vector2(screenRightBound + cloudWidth / 2, rectTransform.anchoredPosition.y);
            }
        }
        else
        {
            // Jika bergerak ke kanan dan keluar dari sisi kanan
            if (rectTransform.anchoredPosition.x > screenRightBound + cloudWidth / 2)
            {
                // Reset ke sisi kiri layar
                rectTransform.anchoredPosition = new Vector2(screenLeftBound - cloudWidth / 2, rectTransform.anchoredPosition.y);
            }
        }
    }
}
