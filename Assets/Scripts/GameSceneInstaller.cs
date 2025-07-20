using DefaultNamespace;
using DefaultNamespace.EnemySystem;
using PlayerSystem;
using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "GameSceneInstaller", menuName = "Installers/GameSceneInstaller")]
public class GameSceneInstaller : ScriptableObjectInstaller<GameSceneInstaller>
{
    [Header("Player System")]
    [SerializeField] private PlayerConfig playerConfig;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private HudView hudView;
    
    [Header("Enemy System")]
    [SerializeField] private GameObject normalEnemyPrefab;
    [SerializeField] private GameObject bigEnemyPrefab;
    [SerializeField] private EnemyConfig normalEnemyConfig;
    [SerializeField] private EnemyConfig bigEnemyConfig;
    [SerializeField] private Transform enemyPoolParent;
    [SerializeField] private EnemyManager enemyManager;
    
    public override void InstallBindings()
    {
        // Player System Bindings
        BindPlayerSystem();
        
        // Enemy System Bindings
        BindEnemySystem();
        
        // Bind HUD View
        Container.Bind<HudView>().FromComponentInNewPrefab(hudView).AsSingle().NonLazy();
    }
    
    private void BindPlayerSystem()
    {
        // Bind Config
        Container.Bind<PlayerConfig>().FromInstance(playerConfig).AsSingle();
        
        // Bind Services
        Container.Bind<IScoreService>().To<ScoreService>().AsSingle();
        
        // Bind Model Factory (creates new model for each player)
        Container.BindFactory<PlayerModel, PlayerModel.Factory>();
        
        // Bind Presenter (will be created per player)
        Container.Bind<PlayerPresenter>().AsTransient();
        
        // Bind Player Factory
        Container.Bind<IPlayerFactory>()
            .To<PlayerFactory>()
            .AsSingle()
            .WithArguments(playerPrefab);
    }
    
    private void BindEnemySystem()
    {
        // Bind Enemy Model Factory
        Container.BindFactory<EnemyConfig, EnemyModel, EnemyModel.Factory>();
        
        // Bind Enemy Factory
        Container.Bind<IEnemyFactory>()
            .To<EnemyFactory>()
            .AsSingle()
            .WithArguments(
                normalEnemyPrefab, 
                bigEnemyPrefab, 
                normalEnemyConfig, 
                bigEnemyConfig, 
                enemyPoolParent
            );
        
        // Bind Enemy Manager
        Container.Bind<EnemyManager>().FromComponentInNewPrefab(enemyManager).AsSingle();
    }
}