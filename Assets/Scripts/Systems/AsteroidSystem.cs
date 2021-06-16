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
        if (!AsteroidManager.asteroidManager.RemoveEnityQueue)
        {
            Entities.ForEach((ref CompositeScale scale, ref AsteroidData asteroid, ref Entity ent, in AsteroidData asteroidData) =>
            {
                if (!AsteroidManager.asteroidManager.RemoveEnityQueue)
                {
                    if (!asteroid.Initalised || asteroid.Hit && !asteroid.Dead)
                    {
                        switch (asteroid.Health)
                        {
                            case 3:
                                asteroid.currentAsteroidScale = asteroidData.asteroidLargeScale;
                                asteroid.Hit = false;
                                break;
                            case 2:
                                asteroid.currentAsteroidScale = asteroidData.asteroidMediumScale;
                                asteroid.Hit = false;
                                break;
                            case 1:
                                asteroid.currentAsteroidScale = asteroidData.asteroidSmallScale;
                                asteroid.Hit = false;
                                break;
                        }
                        asteroid.Initalised = true;
                        scale.Value = asteroid.currentAsteroidScale;
                    }
                    else if (asteroid.Dead)
                    {
                        AsteroidManager.asteroidManager.RemoveEnityQueue = true;
                        if (ent != null)
                        {
                            AsteroidManager.asteroidManager.EntityToRemove = ent;
                        }
                        else
                        {
                            Debug.Log("Ent is null");
                        }
                    }
                }
            }).WithoutBurst().Run();
        }
    }
}
