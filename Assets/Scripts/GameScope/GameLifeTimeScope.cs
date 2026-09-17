using UnityEngine;
using VContainer;
using VContainer.Unity;

public class GameLifeTimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        builder.Register<ScoreModel>(Lifetime.Singleton);
        builder.Register<InputService>(Lifetime.Singleton).As<ITickable>().As<IInputService>();

        builder.RegisterComponentInHierarchy<ScoreView>();
        builder.RegisterComponentInHierarchy<BucketCatcher>();
        builder.RegisterComponentInHierarchy<ControlBucket>();
        builder.RegisterComponentInHierarchy<FishSpawner>();
        builder.RegisterComponentInHierarchy<TimerView>();
        builder.RegisterComponentInHierarchy<GameOverUI>();
        builder.RegisterComponentInHierarchy<PauseController>();
        builder.RegisterComponentInHierarchy<AudioManager>();

        builder.RegisterEntryPoint<GameLoopController>();

    }
}