using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Chain_MountainBikeMinigame2 : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!GameController_MountainBikeMinigame2.instance.isHoldChain)
        {
            gameObject.SetActive(false);
            Destroy(gameObject, 1);
            GameController_MountainBikeMinigame2.instance.VFXChain();
        }
    }
}
