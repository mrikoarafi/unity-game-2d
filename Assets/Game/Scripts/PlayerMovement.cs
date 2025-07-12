using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    // Variabel kecepatan gerakan player yang bisa diatur dari Inspector
    [SerializeField] private float speed;
    [SerializeField] private float jump;
    [SerializeField] private float minX = -8.3f; // Batas minimum posisi X player
    [SerializeField] private float colliderOffsetX = 0.1f; // Offset X untuk BoxCollider

    [SerializeField] private LayerMask groundLayer;
    private BoxCollider2D boxCollider;
    private AudioSource walkAudioSource; // Audio Source untuk walk sound
    private AudioSource jumpAudioSource; // Audio Source untuk jump sound

    // Komponen Rigidbody2D untuk mengontrol fisika player
    private Rigidbody2D body;
    private Animator anim;
    // Komponen SpriteRenderer untuk mengontrol tampilan sprite (flip kiri/kanan)
    private SpriteRenderer spriteRenderer;

    private bool wasWalking = false; // Status walking sebelumnya

    // Mobile input variables
    private bool mobileLeftPressed = false;
    private bool mobileRightPressed = false;
    private bool mobileJumpPressed = false;

    // Dipanggil saat objek pertama kali dibuat
    void Start()
    {
        // Mengambil referensi komponen Rigidbody2D yang ada di GameObject ini
        body = GetComponent<Rigidbody2D>();

        anim = GetComponent<Animator>(); // Mengambil referensi komponen Animator untuk animasi player
        boxCollider = GetComponent<BoxCollider2D>();

        // Mengambil referensi komponen SpriteRenderer yang ada di GameObject ini
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Mengambil Audio Source components (setup manual di Unity)
        AudioSource[] audioSources = GetComponents<AudioSource>();
        if (audioSources.Length >= 2)
        {
            walkAudioSource = audioSources[0]; // Audio Source pertama untuk walk
            jumpAudioSource = audioSources[1]; // Audio Source kedua untuk jump
        }
    }

    // Dipanggil setiap frame untuk menghandle input dan pergerakan
    void Update()
    {
        // Input gerakan horizontal
        HandleMovement();

        // Input lompat
        HandleJump();

        // Update animasi
        UpdateAnimations();

        // Batasi posisi player agar tidak melewati batas kiri
        LimitPlayerPosition();
    }

    // Menangani input gerakan kiri dan kanan
    private void HandleMovement()
    {
        float moveInput = 0f;
        Vector2 offset = boxCollider.offset;

        // Cek input PC (keyboard)
        bool leftInput = Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed;
        bool rightInput = Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed;

        // Gabungkan input PC dan mobile
        leftInput = leftInput || mobileLeftPressed;
        rightInput = rightInput || mobileRightPressed;

        // Cek input kiri
        if (leftInput)
        {
            moveInput = -1f;
            spriteRenderer.flipX = true;    // Menghadap kiri
            offset.x = colliderOffsetX;    // Offset negatif untuk kiri
        }
        // Cek input kanan
        else if (rightInput)
        {
            moveInput = 1f;
            spriteRenderer.flipX = false;   // Menghadap kanan
            offset.x = -colliderOffsetX;     // Offset positif untuk kanan
        }

        // Terapkan offset dan gerakan
        boxCollider.offset = offset;
        body.linearVelocity = new Vector2(moveInput * speed, body.linearVelocity.y);

        // Handle walk sound
        HandleWalkSound(moveInput != 0 && isGrounded());
    }

    // Handle sound effect untuk walking
    private void HandleWalkSound(bool isWalking)
    {
        if (isWalking && !wasWalking && walkAudioSource != null)
        {
            // Mulai walking sound dengan loop
            walkAudioSource.loop = true;
            walkAudioSource.Play();
        }
        else if (!isWalking && wasWalking && walkAudioSource != null)
        {
            // Stop walking sound
            walkAudioSource.Stop();
            walkAudioSource.loop = false;
        }

        wasWalking = isWalking;
    }

    // Menangani input lompat
    private void HandleJump()
    {
        // Gabungkan input PC dan mobile
        bool jumpInput = (Keyboard.current.spaceKey.isPressed || mobileJumpPressed) && isGrounded();

        if (jumpInput)
        {
            Jump();
        }
    }

    // Mengupdate parameter animasi
    private void UpdateAnimations()
    {
        anim.SetBool("walk", body.linearVelocity.x != 0);      // Animasi berjalan
        anim.SetBool("grounded", isGrounded());                // Animasi di tanah
    }

    // Melakukan lompatan
    private void Jump()
    {
        body.linearVelocity = new Vector2(body.linearVelocity.x, jump);
        anim.SetTrigger("jump");

        // Stop walk sound saat jump dan reset status
        if (walkAudioSource != null && walkAudioSource.isPlaying)
        {
            walkAudioSource.Stop();
            walkAudioSource.loop = false;
        }
        wasWalking = false; // Reset status walking agar walk sound bisa dimulai lagi

        // Mainkan sound effect jump
        if (jumpAudioSource != null)
        {
            jumpAudioSource.Play();
        }
    }

    // Mengecek apakah player menyentuh tanah
    private bool isGrounded()
    {
        // Raycast ke bawah untuk deteksi ground menggunakan BoxCast
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.1f, groundLayer);
        return raycastHit.collider != null;
    }

    // Membatasi posisi player agar tidak melewati batas yang ditentukan
    private void LimitPlayerPosition()
    {
        // Jika posisi X player kurang dari batas minimum, pindahkan ke batas minimum
        if (transform.position.x < minX)
        {
            transform.position = new Vector3(minX, transform.position.y, transform.position.z);
        }
    }

    public bool canAttack()
    {
        return body.linearVelocity.x == 0 && isGrounded();
    }

    void OnDisable()
    {
        // Stop movement dan set idle saat PlayerMovement disabled
        if (body != null)
            body.linearVelocity = Vector2.zero;

        // Stop walk sound
        if (walkAudioSource != null && walkAudioSource.isPlaying)
        {
            walkAudioSource.Stop();
            walkAudioSource.loop = false;
        }

        // Set idle animation
        if (anim != null)
        {
            anim.SetBool("walk", false);
            anim.SetBool("grounded", true);
        }
    }

    // Mobile input methods - dipanggil dari UI buttons
    public void OnMobileLeftPressed()
    {
        mobileLeftPressed = true;
    }

    public void OnMobileLeftReleased()
    {
        mobileLeftPressed = false;
    }

    public void OnMobileRightPressed()
    {
        mobileRightPressed = true;
    }

    public void OnMobileRightReleased()
    {
        mobileRightPressed = false;
    }

    public void OnMobileJumpPressed()
    {
        mobileJumpPressed = true;
    }

    public void OnMobileJumpReleased()
    {
        mobileJumpPressed = false;
    }
}
