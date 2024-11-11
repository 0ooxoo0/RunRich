using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    [SerializeField] float Speed;

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y+Speed*Time.deltaTime, 0);
        if(math.abs(transform.rotation.eulerAngles.y) == 360)
        {
            transform.rotation = Quaternion.Euler(0,0,0);
        }
    }
}
