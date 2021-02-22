using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ares
{
    public class AfflictionPersistentEffect : InstantiationEffectInstance
    {
        private Affliction affliction;
        private Actor[] afflictionTargets;

        public override void OnSpawned<T>(Actor caster, Actor[] targets, T origin)
        {
            ActiveEffects.Add(this);

            if (lifetime > 0f)
            {
                StartCoroutine(ScheduleDestruction());
            }
            else
            {
                // In this case, if the lifetime is 0, persist the effect until the affliction is cured.
                if (origin is Affliction)
                {
                    affliction = origin as Affliction;
                    afflictionTargets = targets;
                    foreach (Actor target in afflictionTargets)
                    {
                        target.OnAfflictionCure.AddListener(CureAfflictionEffect);
                    }
                }
            }
        }

        public void CureAfflictionEffect(Affliction curedAffliction)
        {
            if (affliction != curedAffliction) { return; }

            foreach (Actor target in afflictionTargets)
            {
                if (target)
                {
                    target.OnAfflictionCure.RemoveListener(CureAfflictionEffect);
                }
            }

            ActiveEffects.Remove(this);
			Destroy(gameObject);
        }
    }
}