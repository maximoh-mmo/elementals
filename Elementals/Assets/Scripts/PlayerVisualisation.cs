using System;
using UnityEngine;

public class PlayerVisualisation : MonoBehaviour
{
    private void Awake()
    {
        Instantiate(Resources.Load<GameObject>("Prefabs/Kachujin"), transform);    
    }
}