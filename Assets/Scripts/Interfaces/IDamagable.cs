using UnityEngine;
using System.Collections;

public interface IDamagable
{
	//Properties
	int MaxHealth { get; set; }
	int Health { get; set; }

	//Functions
	void Heal(int hp);
	void Damage(int hp);
	bool IsDead();
}