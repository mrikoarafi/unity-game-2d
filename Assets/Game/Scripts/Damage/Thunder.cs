using UnityEngine;

public class Thunder : MonoBehaviour
{
    private Animator anim;
    private AudioSource thunderAudioSource; // Audio Source untuk thunder sound

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim = GetComponent<Animator>();

        // Mengambil Audio Source pertama untuk thunder sound
        AudioSource[] audioSources = GetComponents<AudioSource>();
        if (audioSources.Length >= 1)
        {
            thunderAudioSource = audioSources[0]; // Audio Source pertama (index 0)
        }
    }

    // Method untuk trigger animasi petir
    public void ShowThunder()
    {
        anim.SetTrigger("hit");

        // Mainkan thunder sound
        if (thunderAudioSource != null)
        {
            thunderAudioSource.Play();
        }
    }
}
