using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Objects implementing this will interact with the grass bridge
 */
public interface IGrassBendable
{
    public bool CanBendGrass() { return true; }
}
