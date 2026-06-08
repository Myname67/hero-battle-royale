using UnityEngine;
using System.Collections.Generic;

public class CombatSystem : MonoBehaviour
{
    [SerializeField] private LayerMask characterLayer;
    public static CombatSystem Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    
    public void DealDamage(Character attacker, Character target, int damage)
    {
        if (target == null || !target.IsAlive()) return;
        target.TakeDamage(damage, attacker);
    }
    
    public Character FindTargetInDirection(Vector3 origin, Vector3 direction, float range)
    {
        RaycastHit hit;
        if (Physics.Raycast(origin, direction, out hit, range, characterLayer))
        {
            return hit.collider.GetComponent<Character>();
        }
        return null;
    }
    
    public List<Character> FindTargetsInRadius(Vector3 center, float radius)
    {
        List<Character> targets = new List<Character>();
        Collider[] colliders = Physics.OverlapSphere(center, radius, characterLayer);
        
        foreach (Collider col in colliders)
        {
            Character character = col.GetComponent<Character>();
            if (character != null)
                targets.Add(character);
        }
        
        return targets;
    }
}
