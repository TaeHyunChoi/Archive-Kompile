using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using DataStruct;

public static class DataTable
{
    /* table */
    public static List<UnitData> UnitTable { get; private set; } = new();
    public static List<MapData>  MapTable  { get; private set; } = new();
    //...

    /* I/O binary file */
    public static void WriteTable<T>(string filePath, List<T> dataList) where T : struct, IDataSetter<T>
    {
        using (BinaryWriter writer = new BinaryWriter(File.Open(filePath, FileMode.Create)))
        {
            foreach (T data in dataList)
            {
                data.WriteAsBinary(writer);
            }
        }
    }
    private static List<T> ReadTable<T>(string filePath) where T : struct, IDataSetter<T>
    {
        List<T> table = new List<T>();

        using (BinaryReader reader = new BinaryReader(File.Open(filePath, FileMode.Open)))
        {
            while (reader.BaseStream.Position != reader.BaseStream.Length)
            {
                T data = new();
                data.ReadAsBinary(reader);
                table.Add(data);
            }
        }

        return table;
    }


    /* load data */
    public static void LoadContentTable()
    {
        string path = Application.dataPath + "/Resources/bin/";

        UnitTable = ReadTable<UnitData>(path + "UnitData.bin");
        MapTable  = ReadTable<MapData>(path + "MapData.bin");
    }
    public static Dictionary<int, T> LoadMappingTable<T>(string fileName) where T : struct
    {
        string filePath = Path.Combine(Application.dataPath, "Resources", "bin", "MapTileData", fileName + ".dat");
        if (File.Exists(filePath))
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream fileStream = File.Open(filePath, FileMode.Open);

            // 파일에서 데이터를 역직렬화하여 Dictionary에 로드
            Dictionary<int, T> map = (Dictionary<int, T>)binaryFormatter.Deserialize(fileStream);

            fileStream.Close();
            return map;
        }

        Debug.LogError("파일이 존재하지 않습니다. " + fileName);
        return null;
    }


    /* try get */
    public static bool TryGetMapData(int code, out MapData map)
    {
        for (int i = 0; i < MapTable.Count; ++i)
        {
            map = MapTable[i];
            if (code == map.Code)
            {
                return true;
            }
        }

        map = default;
        return false;
    }
    public static bool TryGetUnitData(int code, out UnitData unit)
    {
        for (int i = 0; i < UnitTable.Count; ++i)
        {
            unit = UnitTable[i];
            if (code == unit.Code)
            {
                return true;
            }
        }

        unit = default;
        return false;
    }


    #region only for dev
#if UNITY_EDITOR || UNITY_EDITOR_64 || UNITY_EDITOR_WIN
    // (for 기획자) custom editor 사용하여 csv 파일을 .bin 파일로 변환
    public static void LoadCSVTable()
    {
        UnitTable = LoadTable<UnitData>("UnitData");
        MapTable = LoadTable<MapData>("MapData");
    }
    private static List<T> LoadTable<T>(string fileName) where T : struct, IDataSetter<T>
    {
        List<Dictionary<string, string>> table = new();
        TextAsset csv = Resources.Load<TextAsset>("CSV/" + fileName);
        StringReader sr = new StringReader(csv.text);
        StringBuilder sb = new StringBuilder();

        //Setting
        string[] columns;   //칼럼명
        int index;          //칼럼명[] 인덱스
        string line;        //각 줄
        char[] chars;       //각 줄을 char 형태로 쪼갬 (중간 ,를 발라내기 위함)
        bool isSplit;       //분류 여부 (대사 등 본문의 ,와 CSV 구분쉼표를 구분하기 위함)

        //Column Index
        line = sr.ReadLine(); //첫줄 날리기
        columns = line.Split(',');

        //Content
        while (true)
        {
            line = sr.ReadLine();
            if (line == null)
            {
                break;
            }

            Dictionary<string, string> data = new Dictionary<string, string>();
            chars = line.ToCharArray();
            isSplit = true;
            index = -1;

            for (int i = 0; i < chars.Length; ++i)
            {
                //데이터 중간의 ,로 나누지 않기 위해 판별 조건 추가
                if (chars[i] == '\u0022') //큰따옴표(")의 유니코드
                {
                    isSplit = !isSplit;
                    continue;
                }

                if (isSplit
                    && chars[i] == '\u002C') //쉼표(,) 유니코드
                {
                    data.Add(columns[++index], sb.ToString());
                    sb.Clear();
                    continue;
                }

                sb.Append(chars[i]);
            }

            //마지막 데이터 추가 (,가 없어서 위에서 안걸림)
            data.Add(columns[++index], sb.ToString());
            table.Add(data);
            sb.Clear();
        }

        List<T> list = new List<T>();
        for (int i = 0; i < table.Count; ++i)
        {
            T tData = new T();
            tData.SetByCSV(table[i]);
            list.Add(tData);
        }

        return list;
    }
    public static void WriteBinaryTables()
    {
        string path = Application.dataPath + "/Resources/bin/";

        WriteTable(path + "UnitData.bin", UnitTable);
        WriteTable(path + "MapData.bin", MapTable);

        Debug.Log($"[Done] Write Binary File;");
    }

    // map sampling
    public static void WriteBinaryMappingData<T>(Dictionary<int, T> data, string fileName) where T : struct
    {
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        string filePath = Path.Combine(Application.dataPath, "Resources", "bin", "MapTileData", fileName + ".dat");
        FileStream fileStream = File.Create(filePath);

        // Dictionary 직렬화
        binaryFormatter.Serialize(fileStream, data);
        fileStream.Close();
    }
#endif
    #endregion
}
