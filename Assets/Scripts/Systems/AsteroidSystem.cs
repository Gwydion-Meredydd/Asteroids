using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[System.Serializable]
public class AsteroidSystem : SystemBase
{
    protected bool InstantiateNewAsteroid;
    protected override void OnUpdate()
    {
        Entities.ForEach((ref CompositeScale scale, ref AsteroidTag asteroid, in AsteroidTag asteroidTag) =>
        {
            if (!asteroid.Initalised || asteroid.Hit)
            {
                switch (asteroid.Health)
                {
                    case 3:
                        asteroid.currentAsteroidScale = asteroidTag.asteroidLargeScale;
                        break;
                    case 2:
                        asteroid.currentAsteroidScale = asteroidTag.asteroidMediumScale;
                        break;
                    case 1:
                        asteroid.currentAsteroidScale = asteroidTag.asteroidSmallScale;
                        break;
                }
                asteroid.Initalised = true;
                scale.Value = asteroid.currentAsteroidScale;
            }
        }).ScheduleParallel();
    }
}
