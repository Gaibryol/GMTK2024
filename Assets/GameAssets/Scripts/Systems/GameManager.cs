using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

	private readonly EventBrokerComponent eventBroker = new EventBrokerComponent();


    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    

}
