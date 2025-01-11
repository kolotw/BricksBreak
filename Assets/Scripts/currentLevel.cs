using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class currentLevel : MonoBehaviour
{
    /*
    �o�̥u�O���ثe���d�A��b������currentLevel���󤤡C
    �B�T�{�����|�Q�R�� DontDestroyOnLoad
    ��C���S�^�쭺���ɡA���i��A���ͤ@���o�Ӫ���
    �ҥH�n�T�{�����|�A�Q����
    �o�O Singleton�Ҧ��C
     */
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

}
