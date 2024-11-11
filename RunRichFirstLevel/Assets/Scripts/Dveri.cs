using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dveri : MonoBehaviour
{
    public int PorogVhoda;

    public void Active()
    {
        transform.parent.GetComponent<Animator>().SetTrigger("Open");
    }
}
