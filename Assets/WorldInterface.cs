using UnityEngine;
using System.Collections;

public class WorldInterface : MonoBehaviour {

	public World world;

	public UnityEngine.UI.Button btnFactory;
	public UnityEngine.UI.Button btnDriller;
	public UnityEngine.UI.Toggle tglProduction;
	public UnityEngine.UI.Toggle tglMining;

	public void BuildFactory()
	{
		world.BuildFactory();
	}

	public void BuildDriller()
	{
		world.BuildDriller();
	}

	public void ToogleProduction(bool v)
	{
		world.ToogleProduction(v);
	}

	public void ToogleMining(bool v)
	{
		world.ToogleMining(v);
	}

	// Use this for initialization
	void Start () {
		btnFactory = this.transform.FindChild("ButtonFactory").GetComponent<UnityEngine.UI.Button>();
		btnDriller = this.transform.FindChild("ButtonDriller").GetComponent<UnityEngine.UI.Button>();
		tglProduction = this.transform.FindChild("ToggleProduction").GetComponent<UnityEngine.UI.Toggle>();
		tglMining = this.transform.FindChild("ToggleMining").GetComponent<UnityEngine.UI.Toggle>();
		btnFactory.onClick.AddListener(BuildFactory);
		btnDriller.onClick.AddListener(BuildDriller);
		tglProduction.onValueChanged.AddListener(ToogleProduction);
		tglMining.onValueChanged.AddListener(ToogleMining);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
