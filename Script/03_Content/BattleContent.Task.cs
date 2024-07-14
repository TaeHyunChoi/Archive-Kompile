using System.Threading.Tasks;
using UnityEngine;

public partial class BattleContent //Task
{
    public class TaskBattleInit : ITaskUpdater
    {
        private int mChapter;

        public TaskBattleInit(int chapter)
        {
            mChapter = chapter;
        }
        public async Task<bool> Await()
        {
            BattleContent battle = new BattleContent(transform: Main.Instance.transform.Find("Battle"));

            GameObject map;
            string code;

            for (int i = 0; i < COUNT_MAP; ++i)
            {
                code = AssetMgr.GetAssetAddress(EAssetType.BattleMap, mChapter * 100 + i);
                map = await AssetMgr.InstantiateGameObjectAsync(code, battle.transform, false);

                if (null != map)
                {
                    battle.mBattleMapArray[i] = map.transform.GetComponent<BattleMapComponent>();
                }
            }

            Main.SetContenData(EContentType.Battle, battle);
            return true;
        }
    }
}
