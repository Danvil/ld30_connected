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
		GetComponentInChildren<UnityEngine.Canvas>().worldCamera = WorldSelector.Singleton.camera;

		btnFactory = this.transform.Search("ButtonFactory").GetComponent<UnityEngine.UI.Button>();
		btnFactory.onClick.AddListener(BuildFactory);

		btnDriller = this.transform.Search("ButtonDriller").GetComponent<UnityEngine.UI.Button>();
		btnDriller.onClick.AddListener(BuildDriller);

		tglProduction = this.transform.Search("ToggleProduction").GetComponent<UnityEngine.UI.Toggle>();
		tglProduction.onValueChanged.AddListener(ToogleProduction);
		ToogleProduction(tglProduction.IsActive());

		tglMining = this.transform.Search("ToggleMining").GetComponent<UnityEngine.UI.Toggle>();
		tglMining.onValueChanged.AddListener(ToogleMining);
		ToogleMining(tglMining.IsActive());


	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
