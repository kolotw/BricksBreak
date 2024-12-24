using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using System.IO;
using System;



public class cmgw
{
	public bool mIsShowLog = true;

	public cmgw(bool isShowLog = true)
	{
		mIsShowLog = isShowLog;
	}

	//incl index no
	public string[] getCSVHeader(string fName)
	{
		if (fName == null)
			return null;

		try
		{
			string path = Application.dataPath + fName;

			using (StreamReader sr = new StreamReader(path))
			{
				string x;

				if ((x = sr.ReadLine()) != null)
				{
					string[] tmp;

					tmp = x.Split(',');

					return tmp;
				}
				else
				{
					return null;
				}
			}
		}
		catch (Exception e)
		{
			log("------CMGW CSV Parsing Error!-----");
			log(e.Message);
			return null;
		}

	}

	//not incl header
	public Dictionary<int, string[]> getAllCSVDict(string fName)
	{
		Dictionary<int, string[]> outData = new Dictionary<int, string[]>();

		if (fName == null)
			return null;

		try
		{
			string path = Application.dataPath + fName;

			using (StreamReader sr = new StreamReader(path))
			{
				int i = 0;
				string x;

				int headerCount = 0;

				while ((x = sr.ReadLine()) != null)
				{
					string[] tmp;

					if (i == 0) //header
					{

					}
					else  //data
					{
						tmp = x.Split(',');
						headerCount = tmp.Length - 2;

						if (tmp[0] != "")
						{

							string[] tmp1 = new string[headerCount]; //decend index column, and the end symbol

							//generate data 
							for (int j = 0; j < headerCount; j++)
							{
								tmp1[j] = tmp[j + 1];
							}

							//add to dict
							outData.Add(int.Parse(tmp[0]), tmp1);
						}
					}
					i += 1;

				}
			}
		}
		catch (Exception e)
		{
			log("------CMGW CSV Parsing Error!-----");
			log(e.Message);
			return null;
		}

		return outData;
	}




	public string getValueByIdNHeader(string fName, int id, string header)
	{
		string[] headerArray = getCSVHeader(fName);

		bool isFindHeader = false;

		int i;
		for (i = 0; i < headerArray.Length; i++)
		{
			if (headerArray[i] == header)
			{
				isFindHeader = true;
				break;
			}
		}

		if (isFindHeader)
		{

			string[] targetArray = getRowByID(fName, id);
			return targetArray[i - 1];
		}
		else
		{
			log("----getValueByIdHeader Error: Can't Find Header --------");
			return null;
		}
	}


	/// <summary>
	/// 取得整列(Row)的資料
	/// </summary>
	/// <param name="fName">檔案名稱</param>
	/// <param name="id">第一行ID</param>
	/// <returns>整列的資料(但不包含ID)</returns>
	public string[] getRowByID(string fName, int id)
	{
		if (fName == null)
			return null;

		try
		{
			string path = Application.dataPath + fName;

			using (StreamReader sr = new StreamReader(path))
			{
				string x;
				while ((x = sr.ReadLine()) != null)
				{
					List<string> list = new List<string>();
					list.AddRange(x.Split(','));
					if (list[0] != id.ToString())
						continue;

					list.RemoveAt(0);
					return list.ToArray();
				}
			}
		}
		catch (Exception e)
		{
			log("------CMGW CSV Parsing Error!-----");
			log(e.Message);
			return null;
		}

		return null;
	}


	public Dictionary<int, string> getColumnByHeader(string fName, string header)  //not incl header
	{
		Dictionary<int, string[]> tmp = new Dictionary<int, string[]>();

		tmp = getFullCSVDict(fName);

		int i;
		bool isFound = false;
		for (i = 0; i < tmp[-1].Length; i++)
		{
			log(tmp[-1][i]);
			if (tmp[-1][i] == header)
			{
				isFound = true;
				break;
			}
		}

		if (!isFound)
		{
			log("!found");
			return null;
		}
		else
		{
			Dictionary<int, string> outData = new Dictionary<int, string>();

			foreach (KeyValuePair<int, string[]> x in tmp)
			{
				if (x.Key != -1) //header
				{
					outData.Add(x.Key, x.Value[i]);
				}
			}

			return outData;
		}
	}



	//incl header
	// row 0 will be the deader
	// data start from row 1, row 1's id = -1

	public Dictionary<int, string[]> getFullCSVDict(string fName)
	{
		Dictionary<int, string[]> outData = new Dictionary<int, string[]>();

		if (fName == null)
			return null;

		try
		{
			string path = Application.dataPath + fName;

			using (StreamReader sr = new StreamReader(path))
			{
				int i = 0;
				string x;

				int headerCount = 0;

				while ((x = sr.ReadLine()) != null)
				{
					string[] tmp;

					if (i == 0) //header
					{
						tmp = x.Split(',');

						headerCount = tmp.Length - 1; //decend index column
						string[] tmp1 = new string[headerCount];

						//generate data 
						for (int j = 0; j < headerCount; j++)
						{
							tmp1[j] = tmp[j + 1];
						}

						//add to dict
						outData.Add(-1, tmp1);
					}
					else  //data
					{
						tmp = x.Split(',');

						if (tmp[0] != "")
						{
							headerCount = tmp.Length - 1;
							string[] tmp1 = new string[headerCount]; //decend index column

							//generate data 
							for (int j = 0; j < headerCount; j++)
							{
								tmp1[j] = tmp[j + 1];
							}

							//add to dict
							outData.Add(int.Parse(tmp[0]), tmp1);

						}
					}

					i += 1;

				}
			}
			//			try {
			//			foreach (KeyValuePair<int, string[]> kv in outData) {
			//				foreach (string v in kv.Value) {
			//					log (kv.Key + " " + v);
			//				}
			//				log ("------separator-----\n");
			//			}
			//			} catch (Exception e) {
			//			}
		}
		catch (Exception e)
		{
			log("------CMGW CSV Parsing Error!-----");
			log(e.Message);
			return null;
		}

		return outData;
	}

	public int getFirstIdByValueNColumn(string fileName, string val, string colName)
	{
		string[] headers = getCSVHeader(fileName);
		int hIdx = -1;

		for (int i = 0; i < headers.Length; i++)
		{
			if (headers[i] == colName)
			{
				hIdx = i - 1;
				break;
			}
		}

		if (hIdx == -1)
			return -1;

		Dictionary<int, string[]> tmp = getAllCSVDict(fileName);

		foreach (KeyValuePair<int, string[]> x in tmp)
		{
			if (x.Value[hIdx] == val)
			{
				return x.Key;
			}
		}

		return -1;
	}

	// Log
	void log(string message)
	{
		if (mIsShowLog)
			Debug.Log(message);
	}
}
