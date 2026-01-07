using EntityStates;
using InfernusMod.Modules.BaseStates;
using R2API.Utils;
using RoR2;
using System.Collections.Generic;
using UnityEngine;
using static RoR2.OverlapAttack;

namespace InfernusMod.Survivors.Infernus.SkillStates
{
    public class Napalm : BaseSkillState
    {
        public OverlapAttack napalmAttack;
        private const float NapalmDebuffDuration = 15f;
        public static float baseDuration = 0.6f;
        public static float procCoefficient = 2f;
        public static float napalmDuration = 15f;
        public static float firePercentTime = 1f;
        public static float pushForce = 10f;
        public static float offsetFloat = 3f;
        public Transform hitBoxTransform;

        private float attackDelay;
        private float fireTime;
        private bool hasFired;

        private OverlapAttack overlapAttack;
        private readonly List<OverlapAttack.OverlapInfo> hitResults = new List<OverlapAttack.OverlapInfo>();

        public override void OnEnter()
        {
            base.OnEnter();

            hitBoxTransform = FindHitBoxGroup("NapalmGroup")?.transform;

            attackDelay = baseDuration / attackSpeedStat;
            fireTime = firePercentTime * attackDelay;
            characterBody.SetAimTimer(2f);

            hasFired = false;

            //Once you have anims PlayAnimation();

            //Once you have the audio Util.PlaySound("InfernusNapalm", gameObject);
        }

        public void Fire()
        {
            if (!isAuthority || hasFired) return;
            if (!hitBoxTransform) return;
            HitBoxGroup hitBoxGroup = FindHitBoxGroup("NapalmGroup");
            if (!hitBoxGroup)
            {
                ChatMessage.Send("NapalmGroup not found!");
                return;
            }

            hasFired = true;

            Vector3 aimDirection = inputBank.aimDirection;
            
            hitBoxTransform.rotation = Quaternion.LookRotation(aimDirection, Vector3.up);

            hitBoxTransform.position = characterBody.corePosition + aimDirection * offsetFloat;

            napalmAttack = new OverlapAttack
            {
                attacker = gameObject,
                inflictor = gameObject,
                teamIndex = characterBody.teamComponent.teamIndex,
                damage = InfernusStaticValues.napalmDamageCoefficient * damageStat,
                procCoefficient = procCoefficient,
                //hitEffectPrefab = hitEffectPrefab,
                forceVector = aimDirection * pushForce,
                isCrit = RollCrit(),
                damageType = DamageType.Generic,
                hitBoxGroup = hitBoxGroup,

                // Hook for applying the napalm debuff
                modifyOutgoingOverlapInfoCallback = (List<OverlapAttack.OverlapInfo> hitList) =>
                {
                    foreach (var hit in hitList)
                    {
                        var body = hit.hurtBox.healthComponent?.body;
                        if (body != null)
                        {
                            body.AddTimedBuff(InfernusDebuffs.napalmDebuff, NapalmDebuffDuration);
                        }
                    }
                }
            };
            napalmAttack.Fire();

        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            float readyState = PercentageDone();


            // Fire only during active hit window

            if (isAuthority && (readyState >= 1f))
            {
                hasFired = false;
                Fire();
                PlayAnimation(attackDelay);

                outer.SetNextStateToMain();
            }

            // Fire only during active hit window
            //if (isAuthority && !hasFired && fixedAge >= fireTime)
            //{
                //hasFired = true;
                //Fire();

            //}

            //if (isAuthority && fixedAge >= attackDelay)
            //{ 
                //outer.SetNextStateToMain();
            //}
        }

        public void PlayAnimation(float duration)
        {

            if (GetModelAnimator())
            {
                PlayAnimation("Gesture, Override", "Napalm", "Slash.playbackRate", duration);
            }
        }

        protected float PercentageDone()
        {
            return Mathf.Clamp01(base.fixedAge / attackDelay);
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Any;
        }

        public override void OnExit()
        {
            base.OnExit();
        }
    }
}