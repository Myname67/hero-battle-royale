using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioClip attackSfx;
    [SerializeField] private AudioClip hitSfx;
    [SerializeField] private AudioClip killSfx;
    [SerializeField] private AudioClip abilitySfx;
    [SerializeField] private float sfxVolume = 0.7f;
    
    private AudioSource audioSource;
    
    public static AudioManager Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }
    
    public void PlayAttackSound()
    {
        if (attackSfx != null)
            audioSource.PlayOneShot(attackSfx, sfxVolume);
    }
    
    public void PlayHitSound()
    {
        if (hitSfx != null)
            audioSource.PlayOneShot(hitSfx, sfxVolume);
    }
    
    public void PlayKillSound()
    {
        if (killSfx != null)
            audioSource.PlayOneShot(killSfx, sfxVolume * 1.2f);
    }
    
    public void PlayAbilitySound()
    {
        if (abilitySfx != null)
            audioSource.PlayOneShot(abilitySfx, sfxVolume);
    }
}
