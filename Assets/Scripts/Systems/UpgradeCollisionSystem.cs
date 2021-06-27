using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

#region SummarySection
/// <summary>
/// System that calls a method in the upgrade manager if the bool has collided is true in upgrade data
/// </summary>
/// <param name="UpgradeCollisionSystem"></param>

#endregion
public class UpgradeCollisionSystem : SystemBase
{
    protected bool InstantiateNewAsteroid;
    protected override void OnUpdate()
    {
        Entities.WithAll<UpgradeData>().ForEach((ref UpgradeData upgradeData, ref Entity ent) =>
        {
            if (upgradeData.HasCollided)
            {
                UpgradeManager.Instance.removeEntity = true;
                UpgradeManager.Instance.entityToRemove = ent;
            }
        }).WithoutBurst().Run(); // needed to write external vars (currently a bug in this version of unity
                                 //.run should work by it self)
    }
}
