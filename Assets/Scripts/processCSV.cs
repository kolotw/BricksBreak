using UnityEngine;
using System;
using System.Collections;
using UnityEngine.Networking;

/*
這個是專門處理CSV檔
.csv檔 是文字檔為基礎，以逗號分格欄位 的一種excel格式
.csv檔 編碼要用 UTF-8，如果要在Excel打開，編碼要用UTF-8 BOM格式，如果要放網站則不要BOM
 */
public class processCSV : MonoBehaviour
{
    private string[,] allValue;
    public static event Action OnBricksGenerated; // 事件宣告 磚塊已生成

    public void getLevel(int level)
    {
        GameObject.Find("00GameMaster").GetComponent<GameController>().關卡載入完成 = false;
        string levelName = "/lv" + string.Format("{0:D2}", level) + ".csv";
        StartCoroutine(InitializeValues(levelName));
    }

    private IEnumerator InitializeValues(string filename)
    {
        yield return StartCoroutine(LoadAllValues(filename, () =>
        {
            // CSV 資料已載入到 allValue，執行生磚塊邏輯
            生磚塊();
        }));
    }

    public IEnumerator LoadAllValues(string fileName, Action callback)
    {
        //CSV檔要存在StreamingAssets資料夾
        string filePath = Application.streamingAssetsPath + "/" + fileName;

        //如果運行平台是WebGL，資料會從網站讀取，會比其他平台花時間，所以要用協程等待
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
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
        string[] rows = csvContent.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        int columns = rows[0].Split(',').Length;

        allValue = new string[rows.Length, columns];

        for (int i = 0; i < rows.Length; i++)
        {
            string[] rowValues = rows[i].Split(',');
            for (int j = 0; j < columns; j++)
            {
                allValue[i, j] = j < rowValues.Length ? rowValues[j] : ""; // 填充空白值
            }
        }

        callback?.Invoke();
        //上面等於下面，簡寫
        //if(callback != null)
        //{
        //    callback();
        //}
    }

    void 生磚塊()
    {
        string brickID = string.Empty;
        int brickLife = 0;

        for (int i = allValue.GetLength(0) - 1; i > 0; i--)
        {
            for (int j = 1; j < allValue.GetLength(1); j++)
            {
                if (allValue[i, j].Length > 0)
                {
                    if (j % 2 == 1)
                    {
                        brickID = allValue[i, j];
                    }
                    else
                    {
                        brickLife = int.Parse(allValue[i, j]);
                        int ii = allValue.GetLength(0) - i;
                        GetComponent<產生磚塊>().levelBricks(ii, j - 1, brickID, brickLife);
                    }
                }
            }
        }

        allValue = null;
        GameObject.Find("00GameMaster").GetComponent<GameController>().關卡載入完成 = true;

        // 觸發事件，通知磚塊生成完成
        OnBricksGenerated?.Invoke();
    }
}
