using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Mage : MonoBehaviour, IDamagable, IAffectable
{
	//Fields
	public Transform m_SpellSpawn; //Otherwise we can't get them in the inspector
	public GameObject m_SpellPrefab;
    public GameObject m_ScrollingTextPrefab;

	protected GameObject m_CurrentSpell = null;
	private List<IEffect> m_Effects = new List<IEffect>();
    private float m_HitTimer = 0.0f;

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

        //Hit effect (sliding backwards)
        if (m_HitTimer > 0.0f) m_HitTimer -= Time.deltaTime;
        if (m_HitTimer <= 0.0f)
        {
            if (transform.position.x < 0.0f && transform.position.x != -4.0f) transform.Translate(new Vector3(-transform.position.x - 4.0f, 0.0f, 0.0f));
            if (transform.position.x > 0.0f && transform.position.x != 4.0f) transform.Translate(new Vector3(4.0f - transform.position.x, 0.0f, 0.0f));
        }
	}

    //----------------
    // Spellcasting
    //----------------
	protected void CreateSpell()
	{
		m_CurrentSpell = Instantiate(m_SpellPrefab, m_SpellSpawn.transform.position, m_SpellSpawn.transform.rotation) as GameObject;
	}

	protected void CastSpell(Mage target)
	{
        Vector2 targetPos = new Vector2(target.gameObject.transform.position.x, target.gameObject.transform.position.y);
        m_CurrentSpell.GetComponent<Spell>().CastAt(targetPos, target);
	}

	protected void CancelSpell()
	{
		Destroy(m_CurrentSpell);
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

        if (destroyEffect)
        {
            spell.Execute();
            spell.Delete();
        }
    }

	//----------------
	// IDamagable
	//----------------
	public void Heal(int hp)
	{
		Health += hp;
        SpawnText("+" + hp, Color.green);
	}

	public void Damage(int hp)
	{
		Health -= hp;
        SpawnText("-" + hp, Color.red);

		Debug.Log ("Aww! " + Health + " out of " + MaxHealth + " left!");

        if (transform.position.x < 0.0f) transform.Translate(new Vector3(-0.15f, 0.0f, 0.0f));
        if (transform.position.x > 0.0f) transform.Translate(new Vector3(0.15f, 0.0f, 0.0f));

        m_HitTimer = 0.05f;
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
        foreach (IEffect origEffect in m_Effects)
        {
            if (origEffect.GetType() == effect.GetType())
            {
                origEffect.OnDuplicate();
                return;
            }
        }

		m_Effects.Add(effect);
	}

	public void RemoveEffect(IEffect effect)
	{
		m_Effects.Remove(effect);
	}

    public bool HasEffect(IEffect effect)
    {
        foreach (IEffect origEffect in m_Effects)
        {
            if (origEffect.GetType() == effect.GetType())
            {
                return true;
            }
        }

        return false;
    }

    //----------------
    // Utility
    //----------------
    public void SpawnText(string text, Color color)
    {
        Vector3 viewPos = Camera.main.WorldToViewportPoint(new Vector3(transform.position.x, transform.position.y + 1.0f, transform.position.z));

        GameObject go = Instantiate(m_ScrollingTextPrefab, viewPos, Quaternion.identity) as GameObject;
        ScrollingText scrollingText = go.GetComponent<ScrollingText>();

        scrollingText.Text = text;
        scrollingText.TextColor = color;
    }
}