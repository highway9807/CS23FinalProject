using UnityEngine;

public class SlipVolume : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        other.GetComponentInParent<PlayerController_TopDown>().BeginGlide();
    }
}
