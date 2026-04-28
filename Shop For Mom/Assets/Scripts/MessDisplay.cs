using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MessDisplay : MonoBehaviour
{
	public GameObject displayArt_tidy;
	public GameObject displayArt_mess;	

	public bool isMessedUp;
	private bool wasMess;

    void Start()
    {
        displayArt_tidy.SetActive(true);
		displayArt_mess.SetActive(false);
    }

	void Update()
	{
		if ((isMessedUp) && (!wasMess))
		{
			MakeMess();
			wasMess = true;
		} 
		else if ((!isMessedUp) && (wasMess))
		{
			MakeTidy();
			wasMess = false;
		}
	}

	public void MakeMess()
	{
		displayArt_tidy.SetActive(false);
		displayArt_mess.SetActive(true);
	}

	public void MakeTidy()
	{
		displayArt_tidy.SetActive(true);
		displayArt_mess.SetActive(false);
	}

}
