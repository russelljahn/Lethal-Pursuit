using UnityEngine;

/* 
	Objects implementing this interface can take damage and die.
 */
interface IDamageable {
	
	void ApplyDamage(float amount, GameObject damager, string message);
	bool IsDead();
	
}