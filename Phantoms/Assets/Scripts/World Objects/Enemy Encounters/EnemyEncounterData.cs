using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyEncounterData : ScriptableObject
{
    public abstract Dictionary<CombatantInstanceData, Ares.Actor> GetEnemies();


    // Utility functions used in a few different types of enemy encounters
    protected Ares.Actor SpawnGenericPhantom(PhantomInstanceData phantom)
    {
        GameObject spawnedPhantom = Instantiate(phantom.Data.BattlePrefab, Vector3.zero, Quaternion.identity);

        Ares.Actor actorComponent = spawnedPhantom.GetComponent<Ares.Actor>();
        if (actorComponent == null)
        {
            actorComponent = spawnedPhantom.AddComponent<Ares.AIActor>();
        }

        SetPhantomActor(spawnedPhantom, actorComponent, phantom);

        return actorComponent;
    }

    protected void SetPhantomActor(GameObject spawnedPhantom, Ares.Actor actorComponent, PhantomInstanceData phantom)
    {
        phantom.SetCurrentStats();
        PhantomDataUtility.SetPhantomActor(actorComponent, phantom);
        Ares.ActorComponents.ActorAnimation actorAnimation = spawnedPhantom.GetComponent<Ares.ActorComponents.ActorAnimation>();
        if (actorAnimation == null)
        {
            actorAnimation = spawnedPhantom.AddComponent<Ares.ActorComponents.ActorAnimation>();
            SetActorAnimations(actorAnimation);
        }
    }

    protected void SetActorAnimations(Ares.ActorComponents.ActorAnimation actorAnimation)
    {
        actorAnimation.Init(false, false, false, true, Ares.ActorComponents.ActorAnimation.ParamaterResetType.DefaultValue, 0, Ares.ActorComponents.ActorAnimation.ParamaterResetType.DefaultValue, 0f, Ares.ActorComponents.ActorAnimation.ParamaterResetType.DefaultValue, false);
        Ares.ActorComponents.ActorAnimationEventElement takeDamageCallback = actorAnimation.GetEventCallback(Ares.ActorComponents.EventCallbackType.TakeDamage);
        takeDamageCallback.Effect.SetAsTrigger("TakeDamage");
        takeDamageCallback.Enabled = true;

        Ares.ActorComponents.ActorAnimationEventElement deathCallback = actorAnimation.GetEventCallback(Ares.ActorComponents.EventCallbackType.Die);
        deathCallback.Effect.SetAsTrigger("Dead");
        deathCallback.Enabled = true;

        foreach(Ares.ActorComponents.ActorAnimationAbilityElement abilityElement in actorAnimation.AbilityCallbacks)
        {
            abilityElement.Enabled = true;
        }
    }
}