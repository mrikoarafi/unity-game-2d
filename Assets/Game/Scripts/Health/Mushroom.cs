using UnityEngine;

public class Mushroom : MonoBehaviour
{
    [SerializeField] private float healthValue; // Nilai health yang diberikan
    private SpriteRenderer playerSprite; // Referensi sprite player
    private AudioSource healAudioSource; // Audio Source untuk heal sound (posisi 0)

    void Start()
    {
        // Mengambil Audio Source pertama untuk heal sound
        AudioSource[] audioSources = GetComponents<AudioSource>();

        healAudioSource = audioSources[0]; // Audio Source pertama untuk heal (index 0)
    }

    // Dipanggil saat objek lain masuk ke trigger collider
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Cek apakah objek yang masuk adalah Player
        if (collision.tag == "Player")
        {
            Health playerHealth = collision.GetComponent<Health>();

            // Cek apakah health player belum penuh
            if (playerHealth.currentHealth < playerHealth.GetMaxHealth())
            {
                // Mainkan heal sound
                healAudioSource.Play();

                // Tambah health ke player
                playerHealth.AddHealth(healthValue);

                // Sembunyikan visual mushroom langsung
                GetComponent<SpriteRenderer>().enabled = false;
                GetComponent<Collider2D>().enabled = false;

                // Simpan referensi sprite dan buat efek berkedip
                playerSprite = collision.GetComponent<SpriteRenderer>();

                // Efek berkedip 2x
                playerSprite.color = new Color(1f, 1f, 1f, 0.5f);
                Invoke("SetFullOpacity", 0.15f);
                Invoke("SetHalfOpacity", 0.3f);
                Invoke("SetFullOpacity", 0.45f);

                // Hapus mushroom setelah sound selesai
                Invoke("DisableMushroom", 1f);
            }
        }
    }

    // Method untuk disable mushroom dengan delay
    private void DisableMushroom()
    {
        gameObject.SetActive(false);
    }

    private void SetHalfOpacity()
    {
        if (playerSprite != null)
            playerSprite.color = new Color(1f, 1f, 1f, 0.5f);
    }

    private void SetFullOpacity()
    {
        if (playerSprite != null)
            playerSprite.color = new Color(1f, 1f, 1f, 1f);
    }
}
