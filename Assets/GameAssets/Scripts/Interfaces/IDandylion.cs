using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDandylion
{
    public bool IsHeavyPath();

    public void OnAttached(GameObject dandylion);

    public void OnDetached(GameObject dandylion);
}
