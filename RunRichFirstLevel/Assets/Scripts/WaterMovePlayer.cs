using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterMovePlayer : MonoBehaviour
{
    [SerializeField] private Transform player;

    void Update()
    {
        transform.position = new Vector3(player.position.x, -0.2f, player.position.z);
    }
}
