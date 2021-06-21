using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

#region SummarySection
/// <summary>
///  system class that is used to change the asteroids scale and tell the asteroid manager class to spawn in extra smaller size asteroids
///  or to delete asteroid all together
/// </summary>
/// <param name="AsteroidSystem"></param>

#endregion
public class AsteroidSystem : SystemBase
{
    protected bool InstantiateNewAsteroid;
    protected override void OnUpdate()
    {
        if (!AsteroidManager.Instance.RemoveEnityQueue)
        {
            Entities.ForEach((ref CompositeScale scale, ref AsteroidData asteroid, ref Entity ent, in Translation pos, in AsteroidData asteroidData) =>
            {
                if (!AsteroidManager.Instance.RemoveEnityQueue)
                {
                    if (!asteroid.Initalised)
                    {
                        //sets current asteroid scale to correct scale on initlisation
                        switch (asteroid.Health)
                        {
                            case 3:
                                asteroid.currentAsteroidScale = asteroidData.asteroidLargeScale;
                                break;
                            case 2:
                                asteroid.currentAsteroidScale = asteroidData.asteroidMediumScale;
                                break;
                            case 1:
                                asteroid.currentAsteroidScale = asteroidData.asteroidSmallScale;
                                break;
                        }
                        asteroid.Initalised = true;
                        scale.Value = asteroid.currentAsteroidScale;
                    }
                    else if(asteroid.Hit && !asteroid.Dead)
                    {
                        //sets the asteroid scale to the correct scale on hit and plays sfx 
                        //also tells asteroid maanger to spawn sub asteroids
                        AudioManager.Instance.PlayRockDestory();
                        switch (asteroid.Health)
                        {
                            case 3:
                                asteroid.currentAsteroidScale = asteroidData.asteroidLargeScale;
                                asteroid.Hit = false;
                                break;
                            case 2:
                                asteroid.currentAsteroidScale = asteroidData.asteroidMediumScale;
                                AsteroidManager.Instance.mediumSpawnPos = pos.Value;
                                AsteroidManager.Instance.spawnMediumAsteroid = true;
                                asteroid.Hit = false;
                                break;
                            case 1:
                                asteroid.currentAsteroidScale = asteroidData.asteroidSmallScale;
                                AsteroidManager.Instance.smallSpawnPos = pos.Value;
                                AsteroidManager.Instance.spawnSmallAsteroid = true;

                                asteroid.Hit = false;
                                break;
                        }
                        asteroid.Initalised = true;
                        scale.Value = asteroid.currentAsteroidScale;
                    }
                    else if (asteroid.Dead)
                    {
                        //removes hit asteroid
                        AsteroidManager.Instance.RemoveEnityQueue = true;
                        if (ent != null)
                        {
                            AudioManager.Instance.PlayRockDestory();
                            AsteroidManager.Instance.DestoryedEntityPos = pos.Value;
                            AsteroidManager.Instance.EntityToRemove = ent;
                        }
                        else
                        {
                            Debug.Log("Ent is null");
                        }
                    }
                }
            }).WithoutBurst().Run();// needed to write external vars (currently a bug in this version of unity
                                    //.run should work by it self)
        }
    }
}
