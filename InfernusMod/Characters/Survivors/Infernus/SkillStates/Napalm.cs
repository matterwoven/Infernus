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

        private float attackDelay;
        private float fireTime;
        private bool hasFired;

        private OverlapAttack overlapAttack;
        private readonly List<OverlapAttack.OverlapInfo> hitResults = new List<OverlapAttack.OverlapInfo>();

        public override void OnEnter()
        {
            base.OnEnter();

            attackDelay = baseDuration / attackSpeedStat;
            fireTime = firePercentTime * attackDelay;
            characterBody.SetAimTimer(2f);

            if (isAuthority)
            {
                Fire();
            }

            //Once you have anims PlayAnimation();

            //Once you have the audio Util.PlaySound("InfernusNapalm", gameObject);
        }

        public void Fire()
        {
            if (!isAuthority || hasFired) return;

            Vector3 aimDirection = GetAimRay().direction;
            Transform hitBoxTransform = FindHitBoxGroup("NapalmGroup")?.transform;
            if (hitBoxTransform != null)
            {
                ChatMessage.Send("Napalm group was null, contact matterwoven in the modding discord about this issue");
                hitBoxTransform.rotation = Quaternion.LookRotation(aimDirection, Vector3.up);
            }
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
                hitBoxGroup = FindHitBoxGroup("NapalmGroup"),

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
            // Fire only during active hit window
            if (fixedAge >= fireTime)
            {
                hasFired = true;
                Fire();
            }

            if (isAuthority && fixedAge >= attackDelay)
            {
                outer.SetNextStateToMain();
            }
        }

        //private OverlapAttack.ModifyOverlapInfoCallback OnHitNap()
        //{

        //return (OverlapAttack overlapAttack, ref OverlapAttack.OverlapInfo hitList) =>
        //{
        //bool returnValue = OverlapAttack.ModifyOverlapInfoCallback(overlapAttack, ref hitList);


        //return returnValue;
        //};
        //}
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