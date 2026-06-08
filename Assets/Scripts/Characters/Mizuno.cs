using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Mizuno : Character
{
    [Header("Mizuno - Support Stats")]
    [SerializeField] private int basicAttackDamage = 18;
    [SerializeField] private int basicAttackFinalDamage = 60;
    [SerializeField] private int alphaDamage = 80;
    [SerializeField] private float alphaKnockback = 5f;
    [SerializeField] private int betaDamage = 120;
    [SerializeField] private float betaStunDuration = 1.5f;
    [SerializeField] private float betaCooldown = 15f;
    [SerializeField] private float gammaShieldDuration = 45f;
    [SerializeField] private float gammaDamageReduction = 0.4f;
    [SerializeField] private float gammaRadiusAllyHeal = 10f;
    [SerializeField] private int gammaAllyHealAmount = 30;
    [SerializeField] private float specialHealAmount = 100f;
    [SerializeField] private float specialCooldownDuration = 90f;
    [SerializeField] private float specialRadius = 15f;
    
    private bool isCombo = false;
    private bool hasShield = false;
    private float shieldTimeRemaining = 0f;
    
    protected override void Start()
    {
        base.Start();
        characterName = "Mizuno Kanade";
    }
    
    protected override void Update()
    {
        base.Update();
        if (!isAlive) return;
        
        if (hasShield)
            UpdateShield();
    }
    
    public override void BasicAttack(Vector3 direction)
    {
        if (!isAlive) return;
        
        int damage = isCombo ? basicAttackDamage : basicAttackFinalDamage;
        
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
        
        isCombo = true;
        StartCoroutine(ResetComboTimer());
    }
    
    private IEnumerator ResetComboTimer()
    {
        yield return new WaitForSeconds(0.5f);
        isCombo = false;
    }
    
    public override void UseAlpha(Vector3 targetDirection)
    {
        if (!isAlive || alphaCooldown > 0) return;
        
        // Water Sphere with knockback
        RaycastHit hit;
        if (Physics.Raycast(transform.position, targetDirection, out hit, 10f))
        {
            Character target = hit.collider.GetComponent<Character>();
            if (target != null && target.GetTeamId() != teamId)
            {
                target.TakeDamage(alphaDamage, this);
                damageDealt += alphaDamage;
                
                // Apply knockback
                Rigidbody targetRb = target.GetComponent<Rigidbody>();
                if (targetRb != null)
                {
                    targetRb.AddForce(targetDirection * alphaKnockback, ForceMode.Impulse);
                }
            }
        }
        
        alphaCooldown = 6f;
    }
    
    public override void UseBeta()
    {
        if (!isAlive || betaCooldown > 0) return;
        
        // Tidal Wave - Area stun
        Collider[] colliders = Physics.OverlapSphere(transform.position, 8f);
        foreach (Collider col in colliders)
        {
            Character target = col.GetComponent<Character>();
            if (target != null && target != this && target.GetTeamId() != teamId)
            {
                target.TakeDamage(betaDamage, this);
                damageDealt += betaDamage;
                StartCoroutine(StunTarget(target));
            }
        }
        
        betaCooldown = 20f;
    }
    
    private IEnumerator StunTarget(Character target)
    {
        // Disable movement temporarily
        Rigidbody targetRb = target.GetComponent<Rigidbody>();
        if (targetRb != null)
        {
            Vector3 originalVelocity = targetRb.velocity;
            targetRb.velocity = Vector3.zero;
            targetRb.isKinematic = true;
            
            yield return new WaitForSeconds(betaStunDuration);
            
            targetRb.isKinematic = false;
            targetRb.velocity = originalVelocity;
        }
    }
    
    public override void UseGamma()
    {
        if (!isAlive || hasShield) return;
        
        // Aqua Shield - Damage reduction
        hasShield = true;
        shieldTimeRemaining = gammaShieldDuration;
        
        // Heal nearby allies
        Collider[] colliders = Physics.OverlapSphere(transform.position, gammaRadiusAllyHeal);
        foreach (Collider col in colliders)
        {
            Character ally = col.GetComponent<Character>();
            if (ally != null && ally != this && ally.GetTeamId() == teamId)
            {
                ally.Heal(gammaAllyHealAmount);
            }
        }
        
        Debug.Log("[Mizuno] Aqua Shield activated!");
    }
    
    private void UpdateShield()
    {
        shieldTimeRemaining -= Time.deltaTime;
        if (shieldTimeRemaining <= 0)
        {
            hasShield = false;
            Debug.Log("[Mizuno] Aqua Shield deactivated.");
        }
    }
    
    public override void TakeDamage(int damage, Character attacker = null)
    {
        // Apply shield reduction
        if (hasShield)
        {
            damage = Mathf.RoundToInt(damage * (1 - gammaDamageReduction));
        }
        
        base.TakeDamage(damage, attacker);
    }
    
    public override void UseSpecial()
    {
        if (specialCooldown > 0) return;
        
        // Healing Rain - Heal self and nearby allies
        Heal(Mathf.RoundToInt(specialHealAmount));
        
        Collider[] colliders = Physics.OverlapSphere(transform.position, specialRadius);
        foreach (Collider col in colliders)
        {
            Character ally = col.GetComponent<Character>();
            if (ally != null && ally != this && ally.GetTeamId() == teamId)
            {
                ally.Heal(Mathf.RoundToInt(specialHealAmount * 0.7f));
            }
        }
        
        specialCooldown = specialCooldownDuration;
        Debug.Log("[Mizuno] Healing Rain cast!");
    }
}
