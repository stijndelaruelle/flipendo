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

	public Mage Target { get; set; }

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
		//Manage our effects (loop backwards so we can delete within the loop)
		for (int i = m_Effects.Count - 1; i >= 0; --i)
		{
			m_Effects[i].Update();
			if (m_Effects[i].DeleteMe) RemoveEffect(m_Effects[i]);
		}
	}

    //----------------
    // Spellcasting
    //----------------
	protected void CreateSpell()
	{
		m_CurrentSpell = Instantiate(m_SpellPrefab, m_SpellSpawn.transform.position, m_SpellSpawn.transform.rotation) as GameObject;
	}

	protected void CastSpell()
	{
		m_CurrentSpell.GetComponent<Spell>().Cast(this);
	}

	protected void CastSpellAt()
	{
		Vector2 targetPos = new Vector2(Target.gameObject.transform.position.x, Target.gameObject.transform.position.y);
        m_CurrentSpell.GetComponent<Spell>().CastAt(targetPos, Target);
	}

	protected void CancelSpell()
	{
		Destroy (m_CurrentSpell);
	}

    public void SpellHit(Spell spell)
    {
        //Loop trough all the effects (may affect the spell)
        //They also wether or not the effect is destroyed
        bool destroyEffect = true;

        foreach (IEffect effect in m_Effects)
        {
            //If one effect wants to keep the spell, it will be kept no matter what the others say
            if (!effect.ProcessSpell(spell)) destroyEffect = false;
        }

        spell.Execute();
        if (destroyEffect) spell.Delete();
    }

	//----------------
	// IDamagable
	//----------------
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

	//----------------
	// IAffectable
	//----------------
	public void AddEffect(IEffect effect)
	{
		m_Effects.Add(effect);
	}

	public void RemoveEffect(IEffect effect)
	{
		m_Effects.Remove(effect);
	}
}