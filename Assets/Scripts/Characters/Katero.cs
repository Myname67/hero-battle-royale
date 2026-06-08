using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Katero : Character
{
    [Header("Katero - Speedster Stats")]
    [SerializeField] private int basicAttackDamage = 20;
    [SerializeField] private int basicAttackFinalDamage = 65;
    [SerializeField] private int alphaDamage = 100;
    [SerializeField] private int betaKickDamage = 100;
    [SerializeField] private int betaUppercutDamage = 100;
    [SerializeField] private int betaPunchDamage = 250;
    [SerializeField] private float betaCooldownDuration = 120f;
    [SerializeField] private float betaCooldownMax = 114f;
    [SerializeField] private float specialCooldownDuration = 120f;
    
    [Header("Gamma - Dark Aura")]
    [SerializeField] private float gammaSpeedMultiplier = 1.6f;
    [SerializeField] private float gammaDamageMultiplier = 1.3f;
    [SerializeField] private float gammaRegenAmount = 20f;
    [SerializeField] private float gammaRegenInterval = 5f;
    [SerializeField] private float gammaDuration = 60f;
    
    private float baseSpeed;
    private bool isGammaActive = false;
    private float gammaTimeRemaining = 0f;
    private float gammaRegenTimer = 0f;
    private bool isInfiniteCombo = false;
    private int comboCount = 0;
    
    private const int MAX_USES_PER_12H = 3;
    private const float TIME_PERIOD = 43200f;
    private List<float> usageTimestamps = new List<float>();
    
    private string[] rareVoiceLines = new string[]
    {
        "Already?",
        "Get up... you can't be dead yet.",
        "And another broken toy..."
    };
    
    private string victoryLine = "Wow... that was quick.";
    
    protected override void Start()
    {
        base.Start();
        baseSpeed = moveSpeed;
        characterName = "Katero Mikitu";
    }
    
    protected override void Update()
    {
        base.Update();
        if (!isAlive) return;
        
        if (isGammaActive)
            UpdateGamma();
        
        CleanupUsageTimestamps();
    }
    
    public override void BasicAttack(Vector3 direction)
    {
        if (!isAlive) return;
        
        int damage = isInfiniteCombo ? basicAttackDamage : basicAttackFinalDamage;
        if (isGammaActive) damage = Mathf.RoundToInt(damage * gammaDamageMultiplier);
        
        // Raycast for target
        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction, out hit, 5f))
        {
            Character target = hit.collider.GetComponent<Character>();
            if (target != null && target.GetTeamId() != teamId)
            {
                target.TakeDamage(damage, this);
                damageDealt += damage;
            }
        }
        
        isInfiniteCombo = true;
        comboCount++;
        StartCoroutine(ResetComboTimer());
    }
    
    private IEnumerator ResetComboTimer()
    {
        yield return new WaitForSeconds(0.5f);
        isInfiniteCombo = false;
        comboCount = 0;
    }
    
    public override void UseAlpha(Vector3 targetDirection)
    {
        if (!isAlive || alphaCooldown > 0) return;
        if (!CanUseCharacter())
        {
            Debug.Log("Character usage limit reached (3 uses per 12 hours)");
            return;
        }
        
        // Speed blitz to target
        RaycastHit hit;
        float maxRange = 1000f;
        Character target = null;
        
        if (Physics.Raycast(transform.position, targetDirection, out hit, maxRange))
        {
            target = hit.collider.GetComponent<Character>();
        }
        
        if (target != null && target.GetTeamId() != teamId)
        {
            // Teleport to target and attack
            transform.position = target.transform.position + Vector3.back * 2f;
            target.TakeDamage(alphaDamage, this);
            damageDealt += alphaDamage;
        }
        
        alphaCooldown = 8f;
        usageTimestamps.Add(Time.time);
    }
    
    public override void UseBeta()
    {
        if (!isAlive || betaCooldown > 0) return;
        StartCoroutine(TripleStrike());
    }
    
    private IEnumerator TripleStrike()
    {
        // First hit: Kick
        int damage = isGammaActive ? Mathf.RoundToInt(betaKickDamage * gammaDamageMultiplier) : betaKickDamage;
        DealAreaDamage(transform.position, 3f, damage);
        yield return new WaitForSeconds(0.3f);
        
        // Second hit: Uppercut
        damage = isGammaActive ? Mathf.RoundToInt(betaUppercutDamage * gammaDamageMultiplier) : betaUppercutDamage;
        DealAreaDamage(transform.position, 3f, damage);
        yield return new WaitForSeconds(0.3f);
        
        // Third hit: Massive Punch
        damage = isGammaActive ? Mathf.RoundToInt(betaPunchDamage * gammaDamageMultiplier) : betaPunchDamage;
        DealAreaDamage(transform.position, 4f, damage);
        
        betaCooldown = characterLevel >= 5 ? betaCooldownMax : betaCooldownDuration;
    }
    
    private void DealAreaDamage(Vector3 center, float radius, int damage)
    {
        Collider[] colliders = Physics.OverlapSphere(center, radius);
        foreach (Collider col in colliders)
        {
            Character target = col.GetComponent<Character>();
            if (target != null && target != this && target.GetTeamId() != teamId)
            {
                target.TakeDamage(damage, this);
                damageDealt += damage;
            }
        }
    }
    
    public override void UseGamma()
    {
        if (!isAlive || isGammaActive) return;
        
        isGammaActive = true;
        gammaTimeRemaining = gammaDuration;
        moveSpeed = baseSpeed * gammaSpeedMultiplier;
        Debug.Log("[Katero] Dark Aura activated!");
    }
    
    private void UpdateGamma()
    {
        gammaTimeRemaining -= Time.deltaTime;
        gammaRegenTimer += Time.deltaTime;
        
        if (gammaRegenTimer >= gammaRegenInterval)
        {
            Heal(Mathf.RoundToInt(gammaRegenAmount));
            gammaRegenTimer = 0f;
        }
        
        if (gammaTimeRemaining <= 0)
            DeactivateGamma();
    }
    
    private void DeactivateGamma()
    {
        isGammaActive = false;
        moveSpeed = baseSpeed;
        Debug.Log("[Katero] Dark Aura deactivated.");
    }
    
    public override void UseSpecial()
    {
        if (specialCooldown > 0) return;
        
        if (!isAlive)
        {
            isAlive = true;
            currentHealth = maxHealth;
            Debug.Log("[Katero] Revived!");
        }
        else
        {
            currentHealth = maxHealth;
            Debug.Log("[Katero] Healed to full!");
        }
        
        specialCooldown = specialCooldownDuration;
    }
    
    public override void OnKill(Character victim)
    {
        base.OnKill(victim);
        
        // Rare voice line at 15 kills
        if (killCount >= 15 && Random.value < 0.0001f)
        {
            Debug.Log("[Katero] " + rareVoiceLines[Random.Range(0, rareVoiceLines.Length)]);
        }
        
        // Maintain gamma on kill
        if (isGammaActive)
        {
            gammaTimeRemaining = gammaDuration;
        }
    }
    
    private bool CanUseCharacter()
    {
        if (usageTimestamps.Count >= MAX_USES_PER_12H)
            return false;
        return true;
    }
    
    private void CleanupUsageTimestamps()
    {
        usageTimestamps.RemoveAll(t => Time.time - t > TIME_PERIOD);
    }
    
    public void OnVictory()
    {
        if (Random.value < 0.01f)
        {
            Debug.Log("[Katero] " + victoryLine);
        }
    }
}
