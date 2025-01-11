using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Networking;


public class processCSV : MonoBehaviour
{
    private string[,] allValue;
    public void getLevel(int level)
    {
        GameObject.Find("00GameMaster").GetComponent<gameMaster>().levelHasLoadCompleted=false;
        string levelName = "/lv" + string.Format("{0:D2}", level) + ".csv";        
        StartCoroutine(InitializeValues(levelName));        
    }

    IEnumerator InitializeValues(string filename)
    {
        yield return StartCoroutine(LoadAllValues(filename, () =>
        {
            // CSV ��Ƥw���J�� allValue
            //Debug.Log($"Data Loaded: {allValue.GetLength(0)} rows, {allValue.GetLength(1)} columns");

            // ���տ�X
            //for (int i = 0; i < allValue.GetLength(0); i++)
            //{
            //    for (int j = 0; j < allValue.GetLength(1); j++)
            //    {
            //        Debug.Log($"Value at [{i}, {j}]: {allValue[i, j]}");
            //    }
            //}
            //Debug.Log("CSV Data Loaded. Ready for the next step!");
            //print(allValue.GetLength(0) + ", " + allValue.GetLength(1));
            �Ϳj��();
        }));
    }

    public IEnumerator LoadAllValues(string fileName, Action callback)
    {
        string filePath = Application.streamingAssetsPath + "/" + fileName;
        
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            // WebGL ���x�ϥ� UnityWebRequest
            //GameObject.Find("/Canvas/filepath").GetComponent<Text>().text = filePath;
            UnityWebRequest request = UnityWebRequest.Get(filePath);
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to load CSV: " + request.error);
                yield break;
            }

            ParseCSV(request.downloadHandler.text, callback);
        }
        else
        {
            // ��L���x����Ū���ɮ�
            try
            {
                string fileContent = System.IO.File.ReadAllText(filePath);
                ParseCSV(fileContent, callback);
            }
            catch (Exception e)
            {
                Debug.LogError("Error reading CSV file: " + e.Message);
            }
        }
    }

    private void ParseCSV(string csvContent, Action callback)
    {
        // ���Φ�
        string[] rows = csvContent.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

        // �p��C�ơ]���]�Ĥ@�檺�C�Ƭ���ǡ^
        int columns = rows[0].Split(',').Length;

        // ��l�� allValue �}�C
        allValue = new string[rows.Length, columns];

        // ��R allValue
        for (int i = 0; i < rows.Length; i++)
        {
            string[] rowValues = rows[i].Split(',');

            for (int j = 0; j < columns; j++)
            {
                allValue[i, j] = j < rowValues.Length ? rowValues[j] : ""; // ��R�ťխ�
            }
        }

        //Debug.Log($"CSV Loaded: {rows.Length} rows, {columns} columns.");
        callback?.Invoke();
    }

    void �Ϳj��()
    {
        string brickID = string.Empty;
        int brickLife = 0;
        // ���տ�X
        //print(allValue.GetLength(0) + " " + allValue.GetLength(1));
        for (int i = allValue.GetLength(0)-1; i > 0; i--)
        {
            for (int j = 1; j < allValue.GetLength(1); j++)
            {
                //Debug.Log($"Value at [{i}, {j}]: {allValue[i, j]}" +  " = " + allValue[i,j]);
                if (allValue[i, j].Length > 0)
                {
                    //�C���A��ƬO�j��ID�A���ƬO�j���ͩR
                    if (j % 2 == 1)
                    {
                        //allValue[i, j] �j��ID
                        brickID = allValue[i, j];
                    }
                    else
                    {
                        //allValue[i, j] �ͩR
                        brickLife = int.Parse(allValue[i, j]);
                        int newi = (i / 2) + 1;
                        int ii = allValue.GetLength(0) - i;
                        GetComponent<���Ϳj��>().levelBricks(ii, j-1, brickID, brickLife);
                    }
                }
            }            
        }
        allValue = null;
        GameObject.Find("00GameMaster").GetComponent<gameMaster>().levelHasLoadCompleted = true;
        //GameObject.Find("/00GameMaster").GetComponent<gameMaster>().isPlaying = true;
    }
}
