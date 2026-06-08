using UnityEngine;

public class EffectManager : MonoBehaviour
{
    [SerializeField] private ParticleSystem hitEffectPrefab;
    [SerializeField] private ParticleSystem killEffectPrefab;
    [SerializeField] private ParticleSystem abilityEffectPrefab;
    
    public static EffectManager Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    
    public void PlayHitEffect(Vector3 position)
    {
        if (hitEffectPrefab != null)
        {
            ParticleSystem effect = Instantiate(hitEffectPrefab, position, Quaternion.identity);
            effect.Play();
            Destroy(effect.gameObject, 2f);
        }
    }
    
    public void PlayKillEffect(Vector3 position)
    {
        if (killEffectPrefab != null)
        {
            ParticleSystem effect = Instantiate(killEffectPrefab, position, Quaternion.identity);
            effect.Play();
            Destroy(effect.gameObject, 3f);
        }
    }
    
    public void PlayAbilityEffect(Vector3 position, Vector3 direction)
    {
        if (abilityEffectPrefab != null)
        {
            ParticleSystem effect = Instantiate(abilityEffectPrefab, position, Quaternion.LookRotation(direction));
            effect.Play();
            Destroy(effect.gameObject, 2f);
        }
    }
}
