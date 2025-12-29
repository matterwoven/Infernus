using InfernusMod.Modules.BaseStates;
using RoR2;
using System.Collections.Generic;
using UnityEngine;
using static RoR2.OverlapAttack;

namespace InfernusMod.Survivors.Infernus.SkillStates
{
    public class Napalm : BaseMeleeAttack
    {
        private OverlapAttack napalmAttack;
        private const float NapalmDebuffDuration = 6f;

        private bool hasFired;

        private float fireTime;
        //Fire delay, feels ass if above 0
        public static float firePercentTime = 0.0f;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            fireTime = firePercentTime * duration;
            characterBody.SetAimTimer(2f);
            muzzleString = "Muzzle";

        }

        public void Fire()
        {
            if (!hasFired)
            {
                hasFired = true;

                hitboxGroupName = "NapalmGroup";

                Util.PlaySound("InfernusShootPistol", gameObject);

                damageType = DamageTypeCombo.GenericSecondary;
                damageCoefficient = InfernusStaticValues.napalmDamageCoefficient;
                procCoefficient = 1f;
                pushForce = 300f;
                bonusForce = Vector3.zero;
                baseDuration = 1f;

                //0-1 multiplier of baseduration, used to time when the hitbox is out (usually based on the run time of the animation)
                //for example, if attackStartPercentTime is 0.5, the attack will start hitting halfway through the ability. if baseduration is 3 seconds, the attack will start happening at 1.5 seconds
                attackStartPercentTime = 0.2f;
                attackEndPercentTime = 0.4f;

                //this is the point at which the attack can be interrupted by itself, continuing a combo
                earlyExitPercentTime = 0.6f;

                hitStopDuration = 0.012f;
                attackRecoil = 0.5f;
                hitHopVelocity = 4f;

                swingSoundString = "InfernusSwordSwing";
                hitSoundString = "";
                //muzzleString = swingIndex % 2 == 0 ? "SwingLeft" : "SwingRight";
                muzzleString = swingIndex % 2 == 0 ? "NapalmGroup" : "NapalmGroup";
                playbackRateParam = "Slash.playbackRate";
                swingEffectPrefab = InfernusAssets.swordSwingEffect;
                hitEffectPrefab = InfernusAssets.swordHitImpactEffect;

                impactSound = InfernusAssets.swordHitSoundEvent.index;

                if (isAuthority)
                {
                    napalmAttack = new OverlapAttack
                    {
                        attacker = gameObject,
                        inflictor = gameObject,
                        teamIndex = characterBody.teamComponent.teamIndex,
                        damage = damageCoefficient * damageStat,
                        procCoefficient = procCoefficient,
                        hitEffectPrefab = hitEffectPrefab,
                        forceVector = transform.forward * pushForce,
                        isCrit = RollCrit(),
                        damageType = damageType,
                        hitBoxGroup = FindHitBoxGroup(hitboxGroupName),

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
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            // Fire only during active hit window
            if (fixedAge >= fireTime)
            {
                Fire();
                hasFired = true;
            }
        }

        protected override void PlayAttackAnimation()
        {
            PlayCrossfade("Gesture, Override", "Slash" + (1 + swingIndex), playbackRateParam, duration, 0.1f * duration);
        }

        protected override void PlaySwingEffect()
        {
            base.PlaySwingEffect();
        }

        //private OverlapAttack.ModifyOverlapInfoCallback OnHitNap()
        //{

            //return (OverlapAttack overlapAttack, ref OverlapAttack.OverlapInfo hitList) =>
            //{
                //bool returnValue = OverlapAttack.ModifyOverlapInfoCallback(overlapAttack, ref hitList);


                //return returnValue;
            //};
        //}


        protected override void OnHitEnemyAuthority()
        {
            base.OnHitEnemyAuthority();
        }

        public override void OnExit()
        {
            base.OnExit();
        }
    }
}