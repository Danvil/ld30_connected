using UnityEngine;
using System.Collections;

public enum PickableType
{
	GOO,
	MINERALS
}

[RequireComponent(typeof(Entity))]
public class Pickable : MonoBehaviour {

	public Entity entity { get; private set; }

	public PickableType type;

	public float maxAmount = 1.0f;

	public float skewValue = 1.0f;

	public float Amount { get; set; }

	public float Gather(float x)
	{
		x = Mathf.Min(Amount, x);
		Amount -= x;
		return x;
	}

	public bool Depleted
	{
		get { return Amount <= 0.0f; }
	}

	public float AmountPercent
	{
		get { return Amount / maxAmount; }
	}

	Vector3 baseScale = Vector3.one;

	void Awake()
	{
		entity = GetComponent<Entity>();
		entity.pickable = this;
		this.transform.localRotation *= Quaternion.AngleAxis(Tools.Random(0,360), Vector3.up);
		if(type == PickableType.GOO) {
			float a = Tools.Random(1.0f/skewValue, skewValue);
			float b = Tools.Random(1.0f/skewValue, skewValue);
			float lambda = 1.0f / Mathf.Sqrt(a*b);
			baseScale = new Vector3(lambda*a, 1.0f, lambda*b);
		}
	}

	// Use this for initialization
	void Start () {
		Amount = maxAmount;
	}
	
	// Update is called once per frame
	void Update () {
		float scl1 = Mathf.Sqrt(0.3f*maxAmount);
		float scl2 = 0.2f + 0.8f*Mathf.Sqrt(AmountPercent);
		this.transform.localScale = scl1 * scl2 * baseScale;
		if(Depleted) {
			Destroy(gameObject);
		}
	}
}
