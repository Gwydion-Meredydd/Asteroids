using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;

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
            float deltatime = Time.DeltaTime;
            Entities.WithAll<AsteroidData>().ForEach((ref CompositeScale scale, ref AsteroidData asteroid, ref PhysicsVelocity vel, ref Entity ent, in Translation pos, in AsteroidData asteroidData) =>
            {
                if (!AsteroidManager.Instance.RemoveEnityQueue)
                {
                    #region asteroid Health
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
                                ScoreManger.Instance.currentScore = ScoreManger.Instance.currentScore + ScoreManger.Instance.largeAsteroidScore;
                                asteroid.Hit = false;
                                break;
                            case 1:
                                asteroid.currentAsteroidScale = asteroidData.asteroidSmallScale;
                                AsteroidManager.Instance.smallSpawnPos = pos.Value;
                                AsteroidManager.Instance.spawnSmallAsteroid = true;
                                ScoreManger.Instance.currentScore = ScoreManger.Instance.currentScore + ScoreManger.Instance.mediumAsteroidScore;

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
                            ScoreManger.Instance.currentScore = ScoreManger.Instance.currentScore + ScoreManger.Instance.smallAsteroidScore;
                        }
                        else
                        {
                            Debug.Log("Ent is null");
                        }
                    }
                    UserInterfaceManager.Instance.UpdateScore();
                    #endregion

                    #region asteroid Movement
                    //keeps veloctiy consistent
                    if (vel.Linear.x < 1 && vel.Linear.x > -1)
                    {
                        if (UnityEngine.Random.Range(0, 2) == 0)
                        {
                            vel.Linear.x = -asteroidData.LinearVelocity.x * asteroidData.LinearVelocity.x;
                        }
                        else 
                        {
                            vel.Linear.x = asteroidData.LinearVelocity.x * asteroidData.LinearVelocity.x;
                        }
                    }
                    if (vel.Linear.y < 1 && vel.Linear.y > -1)
                    {
                        if (UnityEngine.Random.Range(0, 2) == 0)
                        {
                            vel.Linear.y = -asteroidData.LinearVelocity.y * asteroidData.LinearVelocity.y;
                        }
                        else
                        {
                            vel.Linear.y = asteroidData.LinearVelocity.y * asteroidData.LinearVelocity.y;
                        }
                    }
                    if (vel.Linear.z < 1 && vel.Linear.z > -1)
                    {
                        if (UnityEngine.Random.Range(0, 2) == 0)
                        {
                            vel.Linear.z = -asteroidData.LinearVelocity.z * asteroidData.LinearVelocity.z;
                        }
                        else
                        {
                            vel.Linear.z = asteroidData.LinearVelocity.z * asteroidData.LinearVelocity.z;                         }
                    }
                    #endregion
                }
            }).WithoutBurst().Run();// needed to write external vars (currently a bug in this version of unity
                                    //.run should work by it self)
        }
    }
}
