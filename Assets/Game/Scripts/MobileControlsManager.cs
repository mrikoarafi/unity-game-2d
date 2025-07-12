using UnityEngine;

public class MobileControlsManager : MonoBehaviour
{
    void Start()
    {
        // Hide/show canvas berdasarkan platform
        SetCanvasVisibility();
    }

    private void SetCanvasVisibility()
    {
        // Tampilkan canvas hanya di mobile platform
        bool showOnMobile = Application.isMobilePlatform;
        gameObject.SetActive(showOnMobile);
    }
}

