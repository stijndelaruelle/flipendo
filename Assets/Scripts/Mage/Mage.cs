using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Mage : MonoBehaviour, IDamagable, IAffectable
{
	//Fields
	public Transform m_SpellSpawn; //Otherwise we can't get them in the inspector
	public GameObject m_SpellPrefab;
	
	protected GameObject m_CurrentSpell = null;
	private List<IEffect> m_Effects = new List<IEffect>();

	//Properties
	public int MaxHealth { get; set; }
	public int Health { get; set; }

	protected GameObject Target { get; set; }

	//Constructor
	public Mage() {}

	// Use this for initialization
	virtual protected void Start ()
	{
		MaxHealth = 100;
		Health = MaxHealth;
	}
	
	// Update is called once per frame
	virtual protected void Update ()
	{
		//Manage our effects
		for (int i = m_Effects.Count - 1; i >= 0; --i)
		{
			m_Effects[i].OnUpdate();
			if (m_Effects[i].DeleteMe) RemoveEffect(m_Effects[i]);
		}
	}

	protected void CreateSpell()
	{
		m_CurrentSpell = Instantiate(m_SpellPrefab, m_SpellSpawn.transform.position, m_SpellSpawn.transform.rotation) as GameObject;
	}

	protected void CastSpell()
	{
		//Mage mage = Target.GetComponent<Mage>();
		m_CurrentSpell.GetComponent<Spell>().Cast(this);
	}

	protected void CastSpellAt()
	{
        Mage mage = Target.GetComponent<Mage>();

		Vector2 targetPos = new Vector2(Target.transform.position.x, Target.transform.position.y);
		m_CurrentSpell.GetComponent<Spell>().CastAt(targetPos, mage);
	}

	protected void CancelSpell()
	{
		Destroy (m_CurrentSpell);
	}

	//-------------
	// IDamagable
	//-------------
	public void Heal(int hp)
	{
		Health += hp;
	}

	public void Damage(int hp)
	{
		Health -= hp;
		Debug.Log ("Aww! " + Health + " out of " + MaxHealth + " left!");
	}

	public bool IsDead()
	{
		return (Health <= 0);
	}

	//-------------
	// IAffectable
	//-------------
	public void AddEffect(IEffect effect)
	{
		m_Effects.Add(effect);
	}

	public void RemoveEffect(IEffect effect)
	{
		m_Effects.Remove(effect);
	}
}