using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class currentLevel : MonoBehaviour
{
    public static int _CurrentLevel = 0;
    public static currentLevel Instance { get; private set; }
    // Start is called before the first frame update
    void Start()
    {
        if (Instance == null)
        {
            Instance = this; // �]�m���
            DontDestroyOnLoad(gameObject); // �����H�b���������ɳQ�P��
        }
        else
        {
            Destroy(gameObject); // �p�G�w�s�b��ҡA�P����e��H
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
