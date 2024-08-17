using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Objects implementing this can bounce on a bouncing platform
 */
public interface IBounceable
{
    public float forceMultiplier { get { return 1.0f; } }

    
}
