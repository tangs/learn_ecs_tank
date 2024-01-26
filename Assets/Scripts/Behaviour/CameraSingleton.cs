using System;
using UnityEngine;

namespace Behaviour
{
    public class CameraSingleton : MonoBehaviour
    {
        public static Camera Instance;

        public void Awake()
        {
            Instance = GetComponent<Camera>();
        }
    }
}