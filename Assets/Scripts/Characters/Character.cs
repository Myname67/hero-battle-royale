using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Character : MonoBehaviour
{
    [SerializeField] protected string characterName;
    [SerializeField] protected int maxHealth = 350;
    [SerializeField] protected float moveSpeed = 5f;
    
    protected int currentHealth;
    protected int teamId = 0;
    protected bool isAlive = true;
    protected Rigidbody rb;
    protected Animator animator;
    
    protected int characterLevel = 1;
    protected int killCount = 0;
    protected int damageDealt = 0;
    
    // Ability cooldowns
    protected float alphaCooldown = 0f;
    protected float betaCooldown = 0f;
    protected float gammaCooldown = 0f;
    protected float specialCooldown = 0f;
    
    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }
    
    protected virtual void Start()
    {
        currentHealth = maxHealth;
        OnCharacterSpawned();
    }
    
    protected virtual void Update()
    {
        UpdateCooldowns();
    }
    
    protected virtual void OnCharacterSpawned() { }
    
    protected virtual void UpdateCooldowns()
    {
        if (alphaCooldown > 0) alphaCooldown -= Time.deltaTime;
        if (betaCooldown > 0) betaCooldown -= Time.deltaTime;
        if (gammaCooldown > 0) gammaCooldown -= Time.deltaTime;
        if (specialCooldown > 0) specialCooldown -= Time.deltaTime;
    }
    
    public virtual void TakeDamage(int damage, Character attacker = null)
    {
        if (!isAlive) return;
        
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die(attacker);
        }
    }
    
    public virtual void Heal(int amount)
    {
        if (!isAlive) return;
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
    }
    
    protected virtual void Die(Character killer)
    {
        isAlive = false;
        if (killer != null)
            killer.OnKill(this);
    }
    
    public virtual void OnKill(Character victim)
    {
        killCount++;
        damageDealt += victim.maxHealth;
    }
    
    public abstract void BasicAttack(Vector3 direction);
    public abstract void UseAlpha(Vector3 targetDirection);
    public abstract void UseBeta();
    public abstract void UseGamma();
    public abstract void UseSpecial();
    
    public virtual void Move(Vector3 direction)
    {
        if (!isAlive) return;
        rb.velocity = new Vector3(direction.x * moveSpeed, rb.velocity.y, direction.z * moveSpeed);
    }
    
    // Getters
    public int GetCurrentHealth() => currentHealth;
    public int GetMaxHealth() => maxHealth;
    public bool IsAlive() => isAlive;
    public string GetCharacterName() => characterName;
    public int GetTeamId() => teamId;
    public int GetKillCount() => killCount;
    public int GetDamageDealt() => damageDealt;
    public int GetCharacterLevel() => characterLevel;
    
    public void SetTeamId(int team) => teamId = team;
}
