using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class PlayerItemPickup : MonoBehaviour
{
    public Transform player;
    public GameObject[] pickups;

    void Start()
    {
        player = GetComponent<Transform>();
        pickups = GameObject.FindGameObjectsWithTag("Pickups");
    }

    void Update()
    {
        foreach (GameObject pickup in pickups)
        {
            if (pickup == null) return;
            Renderer renderer = pickup.GetComponent<Renderer>();
            if (Vector3.Distance(player.position, pickup.transform.position) <= 2f)
            {
                Debug.Log(pickup.name + " is within the radius.");
                // Change the color
                renderer.material.color = Color.blue;
            }
            else {renderer.material.color = Color.red;}
        }

    }
}
