using EntityStates;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using InfernusMod.Survivors.Infernus;
using RoR2;

namespace InfernusMod.Survivors.Infernus.SkillStates
{
    public class ConcussiveCombustion : GenericProjectileBaseState
    {
        //Stun duration on special, not needed because of how stun is implemented
        //public static float StunDuration = 1.05f;

        //delays for projectiles feel absolute ass so only do this if you know what you're doing, otherwise it's best to keep it at 0
        public static float BaseDelayDuration = 3.0f;

        //Wind up values
        public static float windupTime = 3f; //3 secs
        private bool hasFired; //Don't want to cast multiple at a time

        public static float DamageCoefficient = 16f;

        public override void OnEnter()
        {
            projectilePrefab = InfernusAssets.bombProjectilePrefab;
            //base.effectPrefab = Modules.Assets.SomeMuzzleEffect;
            //targetmuzzle = "muzzleThrow"

            attackSoundString = "InfernusBombThrow";

            baseDuration = windupTime;
            baseDelayBeforeFiringProjectile = BaseDelayDuration;

            damageCoefficient = DamageCoefficient;
            //proc coefficient is set on the components of the projectile prefab
            force = 80f;

            //base.projectilePitchBonus = 0;
            //base.minSpread = 0;
            //base.maxSpread = 0;

            recoilAmplitude = 0.1f;
            bloom = 10f;

            hasFired = false;

            base.OnEnter();
        }

        public override void ModifyProjectileInfo(ref FireProjectileInfo fireProjectileInfo)
        {
            base.ModifyProjectileInfo(ref fireProjectileInfo);
            fireProjectileInfo.damageTypeOverride = DamageType.Stun1s | DamageType.AOE | DamageTypeCombo.GenericSpecial;
        }

        public override void FixedUpdate()
        {
            //Implement windup here through a tickdown
            base.FixedUpdate();

            if (!hasFired && fixedAge >= baseDuration && isAuthority)
            {
                FireProjectile();
                hasFired = true;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }

        public override void PlayAnimation(float duration)
        {

            if (GetModelAnimator())
            {
                PlayAnimation("Gesture, Override", "ThrowBomb", "ThrowBomb.playbackRate", this.duration);
            }
        }
    }
}