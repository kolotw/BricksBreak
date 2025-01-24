using UnityEngine;
using System;
using System.Collections;
using UnityEngine.Networking;

/*
�o�ӬO�M���B�zCSV��
.csv�� �O��r�ɬ���¦�A�H�r��������� ���@��excel�榡
.csv�� �s�X�n�� UTF-8�A�p�G�n�bExcel���}�A�s�X�n��UTF-8 BOM�榡�A�p�G�n������h���nBOM
 */
public class processCSV : MonoBehaviour
{
    private string[,] allValue;
    public static event Action OnBricksGenerated; // �ƥ�ŧi �j���w�ͦ�

    public void getLevel(int level)
    {
        GameObject.Find("00GameMaster").GetComponent<GameController>().���d���J���� = false;
        string levelName = "/lv" + string.Format("{0:D2}", level) + ".csv";
        StartCoroutine(InitializeValues(levelName));
    }

    private IEnumerator InitializeValues(string filename)
    {
        yield return StartCoroutine(LoadAllValues(filename, () =>
        {
            // CSV ��Ƥw���J�� allValue�A����Ϳj���޿�
            �Ϳj��();
        }));
    }

    public IEnumerator LoadAllValues(string fileName, Action callback)
    {
        //CSV�ɭn�s�bStreamingAssets��Ƨ�
        string filePath = Application.streamingAssetsPath + "/" + fileName;

        //�p�G�B�業�x�OWebGL�A��Ʒ|�q����Ū���A�|���L���x��ɶ��A�ҥH�n�Ψ�{����
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
                allValue[i, j] = j < rowValues.Length ? rowValues[j] : ""; // ��R�ťխ�
            }
        }

        callback?.Invoke();
        //�W������U���A²�g
        //if(callback != null)
        //{
        //    callback();
        //}
    }

    void �Ϳj��()
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
                        GetComponent<���Ϳj��>().levelBricks(ii, j - 1, brickID, brickLife);
                    }
                }
            }
        }

        allValue = null;
        GameObject.Find("00GameMaster").GetComponent<GameController>().���d���J���� = true;

        // Ĳ�o�ƥ�A�q���j���ͦ�����
        OnBricksGenerated?.Invoke();
    }
}
