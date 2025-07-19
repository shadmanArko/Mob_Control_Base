using PlayerSystem;
using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "GameSceneInstaller", menuName = "Installers/GameSceneInstaller")]
public class GameSceneInstaller : ScriptableObjectInstaller<GameSceneInstaller>
{
    [SerializeField] private PlayerConfig playerConfig;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform poolParent;
    public override void InstallBindings()
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
            .WithArguments(playerPrefab, poolParent);
    }
}