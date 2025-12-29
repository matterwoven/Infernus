using InfernusMod.Survivors.Infernus;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Networking;
using UnityEngine;
using RoR2;
using HG;

namespace InfernusMod.Characters.Survivors.Infernus.Content
{
    public class FlameZoneController : NetworkBehaviour
    {
        public float lifetime = 6f;
        public float radius = 4f;
        public float damagePerSecond = InfernusStaticValues.dashDamageCoefficient;
        public float tickInterval = 0.5f;

        public GameObject owner;

        private float tickStopwatch;

        private static readonly Collider[] overlapResults = new Collider[64];

        private readonly HashSet<HurtBox> victims = new HashSet<HurtBox>();

        private TeamIndex ownerTeam = TeamIndex.None;
        private CharacterBody ownerBody;

        private static readonly Dictionary<CharacterBody, float> lastDamageTick = new Dictionary<CharacterBody, float>();
        private static float currentTickTime = 0f;

        private void Start()
        {
            if (NetworkServer.active)
            {
                Destroy(gameObject, lifetime);
            }
            if (owner)
            {
                ownerBody = owner.GetComponent<CharacterBody>();
                if (ownerBody)
                {
                    ownerTeam = ownerBody.teamComponent.teamIndex;
                }
            }
        }

        private void FixedUpdate()
        {
            if (!NetworkServer.active || !ownerBody) return;

            // Increment global tick once per FixedUpdate
            FlameZoneDamageManager.UpdateGlobalTick();

            tickStopwatch += Time.fixedDeltaTime;
            if (tickStopwatch < tickInterval) return;

            tickStopwatch -= tickInterval;

            float damageThisTick = damagePerSecond * ownerBody.damage * tickInterval;

            int count = Physics.OverlapSphereNonAlloc(
                transform.position,
                radius,
                overlapResults,
                LayerIndex.entityPrecise.mask
            );

            for (int i = 0; i < count; i++)
            {
                Collider col = overlapResults[i];
                if (!col) continue;

                HurtBox hurtBox = col.GetComponent<HurtBox>();
                if (!hurtBox || !hurtBox.healthComponent) continue;

                CharacterBody victimBody = hurtBox.healthComponent.body;
                if (!victimBody || victimBody == ownerBody) continue;
                if (victimBody.teamComponent && victimBody.teamComponent.teamIndex == ownerTeam) continue;

                // Check against global manager
                if (!FlameZoneDamageManager.CanDamage(victimBody))
                    continue;

                // Apply damage
                DamageInfo damageInfo = new DamageInfo
                {
                    attacker = ownerBody.gameObject,
                    inflictor = gameObject,
                    damage = damageThisTick,
                    damageColorIndex = DamageColorIndex.Default,
                    damageType = DamageType.AOE,
                    crit = ownerBody.RollCrit(),
                    position = hurtBox.transform.position,
                    force = Vector3.zero,
                    procCoefficient = 10f
                };

                hurtBox.healthComponent.TakeDamage(damageInfo);

                // Register damage in global manager
                FlameZoneDamageManager.RegisterDamage(victimBody);
            }
        }


        public static class FlameZoneDamageManager
        {
            public static float globalTickTime = 0f; // increments every FixedUpdate globally
            public static readonly Dictionary<CharacterBody, float> lastDamageTick = new Dictionary<CharacterBody, float>();

            public static void UpdateGlobalTick()
            {
                globalTickTime += Time.fixedDeltaTime;
            }

            public static bool CanDamage(CharacterBody body)
            {
                return !lastDamageTick.TryGetValue(body, out float lastTick) || lastTick < globalTickTime;
            }

            public static void RegisterDamage(CharacterBody body)
            {
                lastDamageTick[body] = globalTickTime;
            }
        }



        //foreach (HurtBox hurtBox in victims)
        //{
        //if (!hurtBox) continue;

        //HealthComponent hc = hurtBox.healthComponent;
        //if (!hc || !hc.body) continue;

        //DotController.InflictDot(
        //hc.gameObject,                     // victim
        //owner,                             // attacker
        //hurtBox,                           // hurtbox of victim
        //InfernusDebuffs.afterburnDebuffIndex,
        //1f,                                // refresh duration
        //1f                                 // stack
        //);
        //}
    }

        //private void OnTriggerEnter(Collider other)
        //{
            //if (!NetworkServer.active) return;

            //HurtBox hurtBox = other.GetComponent<HurtBox>();
            //if (!hurtBox || !hurtBox.healthComponent) return;

            //victims.Add(hurtBox);
        //}

        //private void OnTriggerExit(Collider other)
        //{
            //if (!NetworkServer.active) return;

            //HurtBox hurtBox = other.GetComponent<HurtBox>();
            //if (!hurtBox) return;

            //victims.Remove(hurtBox);
        //}
}
