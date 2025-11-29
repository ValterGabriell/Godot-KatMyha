using Godot;
using KatMyha.Scripts.Enemies.DroneEnemy;
using KatMyha.Scripts.Managers;
using PrototipoMyha.Enemy;
using PrototipoMyha.Player.StateManager;
using PrototipoMyha.Scripts.Utils.Objetos;
using PrototipoMyha.Utilidades;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Godot.Control;

namespace PrototipoMyha.Scripts.Managers
{
    public partial class GameManager : Node
    {
        private static GameManager _instance;
        private CanvasLayer _fadeLayer;
        private ColorRect _fadeRect;
        private int TIME_TO_LOAD_GAME = TIME_TO_LOAD_GAME_CONST;
        private const  int TIME_TO_LOAD_GAME_CONST = 150;
        private bool playerHasBeenKilled = false;
        public int CurrentLevelNumber { get; private set; } = 1;
        private bool hasEmittedKillSignal = false;
        private bool stopEnemy = false;
        public LevelSaveData CurrentLevelObjData { get; private set; }

        public void SetCurrentLevelNumber(int levelNumber)
        {
            CurrentLevelNumber = levelNumber;
        }


        // Salvar quando o jogador apertar uma tecla
        public override void _Input(InputEvent @event)
        {
            // F5 para salvar
            if (@event.IsActionPressed("save_game") 
                && PlayerManager.GetPlayerGlobalInstance().PlayerCanSaveTheGame)
            {
                SaveGame();
            }

            // F9 para carregar
            if (@event.IsActionPressed("load_game"))
            {
                LoadGame();
            }
        }

        public override void _Ready()
        {
            SignalManager.Instance.EnemyKillMyha += OnEnemyKillMyha;
            if (_instance == null)
            {
                _instance = this;
            }
            else
            {
                QueueFree();
            }

           
            _fadeLayer = new CanvasLayer();
            AddChild(_fadeLayer);

  
            _fadeRect = new ColorRect
            {
                Color = new Color(0, 0, 0, 0), // Preto transparente
                AnchorLeft = 0,
                AnchorTop = 0,
                AnchorRight = 1,
                AnchorBottom = 1,
                Position = Vector2.Zero,
                Size = Vector2.Zero,
                MouseFilter = MouseFilterEnum.Ignore
            };
            _fadeLayer.AddChild(_fadeRect);
        }


        private void FadeScreen()
        {
            var tween = CreateTween();
            tween.TweenProperty(_fadeRect, "color:a", 1.0f, 1.0f); // Fade para preto em 1 segundo
        }

        public static GameManager GetGameManagerInstance()
        {
            return _instance;
        }

        public void SetCurrentLevelInitialData(List<EnemyBaseV2> enemyBaseV2s, List<KillFallPyramd> piramedFallList)
        {
            var enemyV2SaveDataList = enemyBaseV2s.Select(e => e.ToSaveData()).ToList();
            var pyramedsToSave = piramedFallList.Select(e => new PyramdFallKillSaveData
            {
                InstanceID = e.InstanceID,
                PositionX = e.GlobalPosition.X,
                PositionY = e.GlobalPosition.Y  
            }).ToList();
            CurrentLevelObjData = new LevelSaveData
            {
                LevelNumber = GameManager.GetGameManagerInstance().CurrentLevelNumber,
                Enemies = enemyV2SaveDataList,
                PyramdsFallKill = pyramedsToSave,
                PlayerPosition_X_OnLevel = PlayerManager.GetPlayerGlobalInstance().GetPlayerPosition().X,
                PlayerPosition_Y_OnLevel = PlayerManager.GetPlayerGlobalInstance().GetPlayerPosition().Y
            };
        }

        public LevelSaveData GetCurrentLevelUpdatedData()
        {
            if (CurrentLevelObjData != null)
            {
                return CurrentLevelObjData;
            }
            return null;
        }

        private Task FadeScreenAsync(float duration)
        {
            var tcs = new TaskCompletionSource<bool>();

            var tween = CreateTween();
            tween.TweenProperty(_fadeRect, "color:a", 1.0f, duration);

            tween.Finished += () =>
            {
                tcs.SetResult(true);
            };

            return tcs.Task;
        }
        private async void OnEnemyKillMyha()
        {
            SetGameSpeed(0.1f);
            await FadeScreenAsync(0.2f);
            
            LoadGame();
        }

        public void SetGameSpeed(float timeScale)
        {
            Engine.TimeScale = timeScale;
        }
        public void SaveGame()
        {
            var playerManager = PlayerManager.GetPlayerGlobalInstance();
            Vector2 currentPlayerPosition = playerManager.GetPlayerPosition();
            
            var saveData = new LevelSaveData()
            {
                LevelNumber = this.CurrentLevelObjData.LevelNumber,
                PlayerPosition_X_OnLevel = currentPlayerPosition.X,
                PlayerPosition_Y_OnLevel = currentPlayerPosition.Y,
                Enemies = this.CurrentLevelObjData.Enemies
            };

            SignalManager.Instance.EmitSignal(nameof(SignalManager.Instance.PlayerSaveTheGame));
            SaveSystem.SaveSystemInstance.SaveGame(saveData);
        }

        public void LoadGame()
        {
      
            playerHasBeenKilled = false;
            TIME_TO_LOAD_GAME = TIME_TO_LOAD_GAME_CONST;
            SetGameSpeed(1.0f);

            var tween = CreateTween();
            tween.TweenProperty(_fadeRect, "color:a", 0.0f, 1.0f);

            var saveData = SaveSystem.SaveSystemInstance.LoadGame();
            if (saveData != null)
            {
                Vector2 loadedPosition = new Vector2(saveData.PlayerPosition_X_OnLevel, saveData.PlayerPosition_Y_OnLevel);

                SignalManager.Instance.EmitSignal(nameof(SignalManager.GameLoaded), loadedPosition);

                var instanceManager = PlayerManager.GetPlayerGlobalInstance();
                instanceManager.UpdatePlayerPosition(loadedPosition);
                instanceManager.BasePlayer.SetState(PlayerState.IDLE);

                var enemiesInScene = GetTree().GetNodesInGroup("enemy"); 
                var pyramdInScene = GetTree().GetNodesInGroup("pyramd"); 
                var alertsInScene = GetTree().GetNodesInGroup(EnumGroups.AlertSprite.ToString());

                foreach (var item in alertsInScene)
                {
                    item.QueueFree();
                }


                foreach (var enemySave in saveData.Enemies)
                {
                    // Encontre o inimigo correspondente pelo Id ou outro identificador
                    var enemy = enemiesInScene
                        .OfType<EnemyBaseV2>()
                        .FirstOrDefault(e => e.GetIdentifier() == enemySave.InstanceID); 

                    if (enemy != null)
                    {
                        enemy.GlobalPosition = new Vector2(enemySave.PositionX, enemySave.PositionY);
                        enemy.EnemyStateBase.TransitionTo(enemySave.EnemyState);
                        enemy.JustLoaded = true;

                    }
                }

                foreach (var item in saveData.PyramdsFallKill)
                {
                    // Encontre o inimigo correspondente pelo Id ou outro identificador
                    var pyramed = pyramdInScene
                        .OfType<KillFallPyramd>()
                        .FirstOrDefault(e => e.InstanceID == item.InstanceID);

                    if (pyramed != null)
                    {
                        var newPyramed = (KillFallPyramd)GD.Load<PackedScene>("res://Scenes/Items/Itens/KillFallPyramd.tscn").Instantiate();
                        newPyramed.InstanceID = item.InstanceID;
                        newPyramed.GlobalPosition = new Vector2(item.PositionX, item.PositionY);
                        GetTree().CurrentScene.AddChild(newPyramed);
                    }
                }


            }

          
        }
        public void KillPlayer(EnemyBaseV2 enemyBaseV2)
        {
            if (!hasEmittedKillSignal)
            {
                SignalManager.Instance.EmitSignal(nameof(SignalManager.EnemyKillMyha));
                enemyBaseV2.Velocity = Vector2.Zero;
                hasEmittedKillSignal = true;
                stopEnemy = true;
            }
   
        }

    }



}
