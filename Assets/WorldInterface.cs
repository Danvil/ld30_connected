using UnityEngine;
using System.Collections;

public class WorldInterface : MonoBehaviour {

	public World world;

	UnityEngine.UI.Text txtName;
	UnityEngine.UI.Toggle tglProduction;
	UnityEngine.UI.Toggle tglMining;

	public void SetName(string name)
	{
		txtName.text = name;
	}

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

	void Awake()
	{
		GetComponentInChildren<UnityEngine.Canvas>().worldCamera = WorldSelector.Singleton.camera;
		
		txtName = this.transform.Search("TextName").GetComponent<UnityEngine.UI.Text>();

		var btnFactory = this.transform.Search("ButtonFactory").GetComponent<UnityEngine.UI.Button>();
		btnFactory.onClick.AddListener(BuildFactory);

		var btnDriller = this.transform.Search("ButtonDriller").GetComponent<UnityEngine.UI.Button>();
		btnDriller.onClick.AddListener(BuildDriller);

		tglProduction = this.transform.Search("ToggleProduction").GetComponent<UnityEngine.UI.Toggle>();
		tglProduction.onValueChanged.AddListener(ToogleProduction);

		tglMining = this.transform.Search("ToggleMining").GetComponent<UnityEngine.UI.Toggle>();
		tglMining.onValueChanged.AddListener(ToogleMining);
	}

	// Use this for initialization
	void Start () {
		ToogleProduction(tglProduction.IsActive());
		ToogleMining(tglMining.IsActive());
	}
	
	// Update is called once per frame
	void Update () {
	}
}
