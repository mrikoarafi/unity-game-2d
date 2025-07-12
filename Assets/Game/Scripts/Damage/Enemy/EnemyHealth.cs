using UnityEngine;
using UnityEngine.InputSystem;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private float startingHealth; // Health awal enemy
    public float currentHealth { get; private set; } // Health saat ini (read-only dari luar)
    private Animator anim;
    private bool dead;
    private AudioSource dieAudioSource; // Audio Source untuk die sound (posisi 0)

    void Start()
    {
        currentHealth = startingHealth; // Set health awal
        anim = GetComponent<Animator>();

        // Mengambil Audio Source pertama untuk die sound
        AudioSource[] audioSources = GetComponents<AudioSource>();

        dieAudioSource = audioSources[0]; // Audio Source pertama untuk die (index 0)
    }

    void Update()
    {
        // Testing: tekan P untuk mengurangi health enemy
        if (Keyboard.current.pKey.wasPressedThisFrame)
        {
            TakeDamage(1);
        }
    }

    // Fungsi untuk menerima damage
    public void TakeDamage(float _damage)
    {
        // Kurangi health dengan batasan minimum 0 dan maksimum startingHealth
        currentHealth = Mathf.Clamp(currentHealth - _damage, 0, startingHealth);

        // Hanya trigger die saat health habis
        if (currentHealth <= 0 && !dead)
        {
            anim.SetTrigger("die");

            // Mainkan die sound
            dieAudioSource.Play();

            // Ubah posisi Y enemy saat mati (turunkan -0.30)
            Vector3 currentPos = transform.position;
            transform.position = new Vector3(currentPos.x, currentPos.y - 0.30f, currentPos.z);

            // Disable semua MonoBehaviour scripts kecuali EnemyHealth
            MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();
            foreach (MonoBehaviour script in scripts)
            {
                if (script != this) // Jangan disable EnemyHealth sendiri
                    script.enabled = false;
            }

            dead = true;
        }
    }

    // Tampilkan health bar di game (bukan hanya di Scene View)
    private void OnGUI()
    {
        if (!dead && Camera.main != null)
        {
            // Konversi posisi world ke screen - mendekatkan ke enemy
            Vector3 healthBarPos = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 1f);

            // Pastikan enemy terlihat di layar
            if (healthBarPos.z > 0)
            {
                // Flip Y coordinate untuk GUI
                healthBarPos.y = Screen.height - healthBarPos.y;

                // Ukuran health bar
                float barWidth = 100f;
                float barHeight = 10f;
                float healthPercentage = currentHealth / startingHealth;

                // Background (merah)
                GUI.color = Color.red;
                GUI.DrawTexture(new Rect(healthBarPos.x - barWidth / 2, healthBarPos.y - barHeight / 2, barWidth, barHeight), Texture2D.whiteTexture);

                // Health (hijau)
                GUI.color = Color.green;
                GUI.DrawTexture(new Rect(healthBarPos.x - barWidth / 2, healthBarPos.y - barHeight / 2, barWidth * healthPercentage, barHeight), Texture2D.whiteTexture);

                // Reset color
                GUI.color = Color.white;
            }
        }
    }
}
