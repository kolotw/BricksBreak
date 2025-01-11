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
            // CSV 資料已載入到 allValue
            //Debug.Log($"Data Loaded: {allValue.GetLength(0)} rows, {allValue.GetLength(1)} columns");

            // 測試輸出
            //for (int i = 0; i < allValue.GetLength(0); i++)
            //{
            //    for (int j = 0; j < allValue.GetLength(1); j++)
            //    {
            //        Debug.Log($"Value at [{i}, {j}]: {allValue[i, j]}");
            //    }
            //}
            //Debug.Log("CSV Data Loaded. Ready for the next step!");
            //print(allValue.GetLength(0) + ", " + allValue.GetLength(1));
            生磚塊();
        }));
    }

    public IEnumerator LoadAllValues(string fileName, Action callback)
    {
        string filePath = Application.streamingAssetsPath + "/" + fileName;
        
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            // WebGL 平台使用 UnityWebRequest
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
            // 其他平台直接讀取檔案
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
        // 分割行
        string[] rows = csvContent.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

        // 計算列數（假設第一行的列數為基準）
        int columns = rows[0].Split(',').Length;

        // 初始化 allValue 陣列
        allValue = new string[rows.Length, columns];

        // 填充 allValue
        for (int i = 0; i < rows.Length; i++)
        {
            string[] rowValues = rows[i].Split(',');

            for (int j = 0; j < columns; j++)
            {
                allValue[i, j] = j < rowValues.Length ? rowValues[j] : ""; // 填充空白值
            }
        }

        //Debug.Log($"CSV Loaded: {rows.Length} rows, {columns} columns.");
        callback?.Invoke();
    }

    void 生磚塊()
    {
        string brickID = string.Empty;
        int brickLife = 0;
        // 測試輸出
        //print(allValue.GetLength(0) + " " + allValue.GetLength(1));
        for (int i = allValue.GetLength(0)-1; i > 0; i--)
        {
            for (int j = 1; j < allValue.GetLength(1); j++)
            {
                //Debug.Log($"Value at [{i}, {j}]: {allValue[i, j]}" +  " = " + allValue[i,j]);
                if (allValue[i, j].Length > 0)
                {
                    //列號，單數是磚塊ID，雙數是磚塊生命
                    if (j % 2 == 1)
                    {
                        //allValue[i, j] 磚塊ID
                        brickID = allValue[i, j];
                    }
                    else
                    {
                        //allValue[i, j] 生命
                        brickLife = int.Parse(allValue[i, j]);
                        int newi = (i / 2) + 1;
                        int ii = allValue.GetLength(0) - i;
                        GetComponent<產生磚塊>().levelBricks(ii, j-1, brickID, brickLife);
                    }
                }
            }            
        }
        allValue = null;
        GameObject.Find("00GameMaster").GetComponent<gameMaster>().levelHasLoadCompleted = true;
        //GameObject.Find("/00GameMaster").GetComponent<gameMaster>().isPlaying = true;
    }
}
