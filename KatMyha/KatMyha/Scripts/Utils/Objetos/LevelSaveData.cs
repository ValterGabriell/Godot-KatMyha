using System;
using System.Collections.Generic;

namespace PrototipoMyha.Scripts.Utils.Objetos
{
    public class LevelSaveData
    {
        public int LevelNumber { get; set; }
        public float PlayerPosition_X_OnLevel { get; set; }
        public float PlayerPosition_Y_OnLevel { get; set; }
        public List<PyramdFallKillSaveData> PyramdsFallKill { get; set; }
        public List<PlayerHabilityKey> PlayerHabilitiesUnlocked { get; set; }
        public List<PlayerSubphaseKey> PlayerSubphaseKeysObtained { get; set; }


    }



    public class PyramdFallKillSaveData
    {
        public Guid InstanceID { get; set; }
        public float PositionX { get; set; }
        public float PositionY { get; set; }
    }
}
