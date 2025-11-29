using PrototipoMyha.Enemy.States;
using System;
using System.Collections.Generic;

namespace PrototipoMyha.Scripts.Utils.Objetos
{
    public class LevelSaveData
    {
        public int LevelNumber { get; set; }
        public float PlayerPosition_X_OnLevel { get; set; }
        public float PlayerPosition_Y_OnLevel { get; set; }
        public List<EnemySaveData> Enemies { get; set; }
        public List<PyramdFallKillSaveData> PyramdsFallKill { get; set; }
    }

    public class EnemySaveData
    {
        public Guid InstanceID { get; set; }
        public float PositionX { get; set; }
        public float PositionY { get; set; }
        public EnumEnemyState EnemyState { get; set; }
    }

    public class PyramdFallKillSaveData
    {
        public Guid InstanceID { get; set; }
        public float PositionX { get; set; }
        public float PositionY { get; set; }
    }
}
