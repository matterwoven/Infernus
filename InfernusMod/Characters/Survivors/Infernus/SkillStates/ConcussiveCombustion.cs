using EntityStates;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using InfernusMod.Survivors.Infernus;
using RoR2;
using static RoR2.OverlapAttack;
using R2API.Utils;

namespace InfernusMod.Survivors.Infernus.SkillStates
{
    public class ConcussiveCombustion : BaseSkillState
    {
        //Stun duration on special, not needed because of how stun is implemented in damage
        //public static float StunDuration = 1.05f;

        //Wind up values
        public OverlapAttack concussiveAttack;
        public static float baseDuration = 3.0f;
        public static float damageCoefficient = 16f;
        public static float procCoefficient = 2f;
        public static float napalmDuration = 15f;
        public static float firePercentTime = 1f;
        public static float pushForce = 10f;
        public static float windupTime = 3f; //3 secs

        private float attackDelay;
        private float fireTime;
        private bool hasFired;


        public override void OnEnter()
        {
            base.OnEnter();

            hasFired = false;

            characterBody.SetAimTimer(baseDuration);

            //Once you have anims PlayAnimation();

            //Once you have the audio Util.PlaySound("InfernusNapalm", gameObject);
        }
        
        public void Fire()
        {
            HitBoxGroup concussiveCombustion = FindHitBoxGroup("ConcussiveGroup");

            OverlapAttack attack = new OverlapAttack
            {
                attacker = gameObject,
                inflictor = gameObject,
                teamIndex = characterBody.teamComponent.teamIndex,
                damage = InfernusStaticValues.napalmDamageCoefficient * damageStat,
                procCoefficient = procCoefficient,
                //hitEffectPrefab = hitEffectPrefab,
                isCrit = RollCrit(),
                damageType = DamageType.Stun1s | DamageType.AOE,
                hitBoxGroup = FindHitBoxGroup("ConcussiveGroup"),
            };

            ChatMessage.Send("Napalm group was null, contact matterwoven in the modding discord about this issue");
            attack.Fire();
        }

        public override void FixedUpdate()
        {
            //Implement windup here through a tickdown
            base.FixedUpdate();



            // Fire only during active hit window
            if (isAuthority && !hasFired && fixedAge >= windupTime)
            {
                hasFired = true;
                Fire();
            }

            if (isAuthority && fixedAge >= attackDelay)
            {
                outer.SetNextStateToMain();
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }

        public void PlayAnimation(float duration)
        {

            if (GetModelAnimator())
            {
                PlayAnimation("Combustion, Override", "Combust", "Combust.playbackRate", duration);
            }
        }

        public override void OnExit()
        {
            base.OnExit();
        }
    }
}