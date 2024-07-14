using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor.UIElements;
using UnityEngine;

public partial class FieldContent // Task
{
    public class TaskFieldInit : ITaskUpdater
    {
        public async Task<bool> Await()
        {
            Transform level = GameObject.FindWithTag("Field").transform;
            FieldContent field = new FieldContent(level);

            Debug.Log("For Test: Set Player");
            //TODO: Player "Group"이 움직일 수 있도록 개발 필요
            field.player = await AssetMgr.SpawnUnit<UnitPlayer>(PlayData.PartyData[0].UnitCode, level.Find("Unit"));
            field.player.transform.position = new Vector3(0.5f, 0f, 0.5f);
            field.SetLayer(0);

            Main.SetContenData(EContentType.Field, field);
            return true;
        }
    }
}
