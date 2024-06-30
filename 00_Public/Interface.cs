using System.Collections.Generic;
using System.IO;

public interface IDataSetter<T> where T : struct
{
    public void SetByCSV(Dictionary<string, string> table);
    /// <summary>
    /// 구조체에 선언한 변수 순서와 동일하게 작성하라. 그렇지 않으면 데이터를 잘못 읽는다.
    /// </summary>
    public void WriteAsBinary(BinaryWriter writer);
    /// <summary>
    /// 구조체에 선언한 변수 순서와 동일하게 읽어라. 그렇지 않으면 데이터를 잘못 읽는다.
    /// </summary>
    public void ReadAsBinary(BinaryReader reader);
}

public interface IRoutineUpdater
{
    public int MoveNext(int index);
}

public interface IInputHandler
{
    public void Input(Index.IDxInput.EInput input);
}