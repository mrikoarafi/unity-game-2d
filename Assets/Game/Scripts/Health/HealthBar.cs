using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Health playerHealth;  // Referensi ke script Health
    [SerializeField] private Image heartPrefab;    // Prefab gambar hati
    [SerializeField] private Transform heartsContainer; // Container untuk menyimpan hati-hati

    private Image[] hearts; // Array untuk menyimpan semua hati

    private void Start()
    {
        // Buat hati sesuai dengan max health
        CreateHearts();
    }

    private void Update()
    {
        // Update tampilan hati berdasarkan current health
        UpdateHearts();
    }

    // Membuat hati sesuai dengan max health
    private void CreateHearts()
    {
        // Hapus child yang sudah ada di container (jika ada)
        foreach (Transform child in heartsContainer)
        {
            Destroy(child.gameObject);
        }

        int maxHealth = Mathf.RoundToInt(playerHealth.GetMaxHealth());
        hearts = new Image[maxHealth];

        for (int i = 0; i < maxHealth; i++)
        {
            // Instantiate hati baru sebagai child dari heartsContainer
            GameObject newHeartObj = Instantiate(heartPrefab.gameObject, heartsContainer);
            hearts[i] = newHeartObj.GetComponent<Image>();
        }
    }

    // Update tampilan hati berdasarkan current health
    private void UpdateHearts()
    {
        int currentHealth = Mathf.RoundToInt(playerHealth.currentHealth);

        for (int i = 0; i < hearts.Length; i++)
        {
            // Jika index lebih kecil dari current health, tetap warna asli
            // Jika tidak, ubah warna menjadi hitam
            if (i < currentHealth)
            {
                hearts[i].color = Color.white; // Warna asli (putih/normal)
            }
            else
            {
                hearts[i].color = Color.black; // Warna hitam untuk hati yang hilang
            }
        }
    }
}
