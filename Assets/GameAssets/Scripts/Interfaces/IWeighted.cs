using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Objects implmenting this will interact with weight dependent platforms
 */
public interface IWeighted
{
    public float GetWeight();
}
