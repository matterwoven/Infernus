using System;
using System.Collections.Generic;
using System.Text;
using EntityStates;
using InfernusMod.Survivors.Infernus;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace InfernusMod.Survivors.Infernus.SkillStates
{
    internal class InfernusOnHit
    {

        public static void Init()
        {
            GlobalEventManager.onServerDamageDealt += OnDamageDealt;
        }

        private static void OnDamageDealt(DamageReport report)
        {
            if (!report.attacker || !report.victim)
                return;

            CharacterBody attackerBody = report.attackerBody;
            if (attackerBody)
            {
                BepInEx.Logging.Logger.CreateLogSource("Infernus").LogError("InfernusRegistered");
            }

            CharacterBody victimBody = report.victimBody;
            if (!victimBody)
                return;

            ApplyAfterburn(attackerBody, victimBody);
        }

        private static void ApplyAfterburn(CharacterBody attacker, CharacterBody victim)
        {
            int stacksPerHit = 6;
            float afterburnDuration = 6f;

            if (victim.HasBuff(InfernusDebuffs.afterburnDebuff))
                inflictAfterburn();

            int currentStacks = victim.GetBuffCount(InfernusDebuffs.afterburnBuildup);

            for (int i = 0; i < stacksPerHit; i++)
            {
                victim.AddBuff(
                    InfernusDebuffs.afterburnBuildup
                );
            }

            if (currentStacks >= 100)
            {
                inflictAfterburn();
            }

            void inflictAfterburn()
            {
                victim.RemoveBuff(InfernusDebuffs.afterburnBuildup);
                DotController.InflictDot(
                    victim.gameObject,
                    attacker.gameObject,
                    InfernusDebuffs.afterburnBuildup,
                    InfernusDebuffs.afterburnDebuff,
                    afterburnDuration
                );
            }
        }
    }

}
