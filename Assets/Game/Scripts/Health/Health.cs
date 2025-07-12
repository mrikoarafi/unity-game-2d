using UnityEngine;
using UnityEngine.InputSystem;

public class Health : MonoBehaviour
{
    [SerializeField] private float startingHealth; // Health awal player
    [SerializeField] private float dieYOffset = -0.5f; // Offset Y BoxCollider2D saat mati
    public float currentHealth { get; private set; } // Health saat ini (read-only dari luar)
    private Animator anim;
    private bool dead;
    private BoxCollider2D boxCollider;
    private AudioSource hurtAudioSource; // Audio Source untuk hurt sound (posisi 4)
    private AudioSource dieAudioSource; // Audio Source untuk die sound (posisi 5)

    void Start()
    {
        currentHealth = startingHealth; // Set health awal
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();

        // Mengambil Audio Source untuk hurt sound
        AudioSource[] audioSources = GetComponents<AudioSource>();

        hurtAudioSource = audioSources[3]; // Audio Source keempat untuk hurt (index 3)
        dieAudioSource = audioSources[4]; // Audio Source keelima untuk die (index 4)
    }

    void Update()
    {
        // Testing: tekan E untuk mengambil damage 1 (perbaiki input)
        // if (Keyboard.current.eKey.wasPressedThisFrame)
        //     TakeDamage(1);
    }

    // Fungsi untuk menerima damage
    public void TakeDamage(float _damage)
    {
        // Kurangi health dengan batasan minimum 0 dan maksimum startingHealth
        currentHealth = Mathf.Clamp(currentHealth - _damage, 0, startingHealth);

        if (currentHealth > 0)
        {
            anim.SetTrigger("hurt");

            // Mainkan hurt sound
            hurtAudioSource.Play();
        }
        else
        {
            if (!dead)
            {
                GetComponent<PlayerMovement>().enabled = false;

                // Ubah offset Y BoxCollider2D saat mati
                Vector2 currentOffset = boxCollider.offset;
                boxCollider.offset = new Vector2(currentOffset.x, currentOffset.y + dieYOffset);

                // Panggil UI Die menu
                UiDie uiDie = FindObjectOfType<UiDie>();
                uiDie.ShowDieMenu();

                dead = true;
                anim.SetTrigger("die");

                // Mainkan die sound
                dieAudioSource.Play();
            }
        }
    }

    // Method untuk mengakses startingHealth dari luar
    public float GetMaxHealth()
    {
        return startingHealth;
    }

    // Method untuk menambah health (untuk collectible)
    public void AddHealth(float _health)
    {
        // Tambah health dengan batasan maksimum startingHealth
        currentHealth = Mathf.Clamp(currentHealth + _health, 0, startingHealth);
    }
}
