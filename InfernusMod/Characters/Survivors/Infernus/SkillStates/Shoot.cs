using EntityStates;
using InfernusMod.Survivors.Infernus;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using R2API;
using static UnityEngine.SendMouseEvents;

namespace InfernusMod.Survivors.Infernus.SkillStates
{
    public class Shoot : BaseSkillState
    {
        public static float damageCoefficient = InfernusStaticValues.gunDamageCoefficient;
        public static float procCoefficient = 0.6f;
        public static float baseDuration = 0.23f;
        //delay on firing is usually ass-feeling. only set this if you know what you're doing
        public static float firePercentTime = 0.0f;
        public static float force = 200f;
        public static float recoil = 0.5f;
        public static float range = 256f;
        public static GameObject tracerEffectPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/Tracers/TracerGoldGat");

        private float duration;
        private float fireTime;
        private bool hasFired;
        private string muzzleString;

        //Afterburn defs
        private const int MaxBuildupStacks = 10;
        private const float BuildupDuration = 5f;
        private const float AfterburnDuration = 10f;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            fireTime = firePercentTime * duration;
            characterBody.SetAimTimer(2f);
            muzzleString = "Muzzle";

            PlayAnimation("LeftArm, Override", "ShootGun", "ShootGun.playbackRate", 1.8f);
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (fixedAge >= fireTime)
            {
                Fire();
            }

            if (fixedAge >= duration && isAuthority)
            {
                outer.SetNextStateToMain();
                return;
            }
        }

        private void Fire()
        {
            if (!hasFired)
            {
                hasFired = true;

                characterBody.AddSpreadBloom(1.5f);
                EffectManager.SimpleMuzzleFlash(EntityStates.Commando.CommandoWeapon.FirePistol2.muzzleEffectPrefab, gameObject, muzzleString, false);
                Util.PlaySound("InfernusShootPistol", gameObject);

                if (isAuthority)
                {
                    Ray aimRay = GetAimRay();
                    AddRecoil(-1f * recoil, -2f * recoil, -0.5f * recoil, 0.5f * recoil);

                    new BulletAttack
                    {
                        bulletCount = 1,
                        aimVector = aimRay.direction,
                        origin = aimRay.origin,
                        damage = damageCoefficient * damageStat,
                        damageColorIndex = DamageColorIndex.Default,
                        damageType = DamageTypeCombo.GenericPrimary,
                        falloffModel = BulletAttack.FalloffModel.None,
                        maxDistance = range,
                        force = force,
                        hitMask = LayerIndex.CommonMasks.bullet,
                        minSpread = 0f,
                        maxSpread = 0f,
                        isCrit = RollCrit(),
                        owner = gameObject,
                        muzzleName = muzzleString,
                        smartCollision = true,
                        procChainMask = default,
                        procCoefficient = procCoefficient,
                        radius = 0.75f,
                        sniper = false,
                        stopperMask = LayerIndex.CommonMasks.bullet,
                        weapon = null,
                        tracerEffectPrefab = tracerEffectPrefab,
                        spreadPitchScale = 1f,
                        spreadYawScale = 1f,
                        queryTriggerInteraction = QueryTriggerInteraction.UseGlobal,
                        hitEffectPrefab = EntityStates.Commando.CommandoWeapon.FirePistol2.hitEffectPrefab,

                        //Apply afterburn stack on hit
                        hitCallback = ApplyAfterburnOnHit()
                    }.Fire();
                    
                }
            }
        }

        private BulletAttack.HitCallback ApplyAfterburnOnHit()
        {
            return (BulletAttack bulletAttack, ref BulletAttack.BulletHit hitInfo) =>
            {
                bool returnValue = BulletAttack.DefaultHitCallbackImplementation(bulletAttack, ref hitInfo);
                bool isCrit = bulletAttack.isCrit;

                if (!hitInfo.hitHurtBox)
                {
                    return returnValue;
                }

                //Victim data
                HealthComponent victimHealthComponent = hitInfo.hitHurtBox.healthComponent;
                CharacterBody victimCharacterBody = victimHealthComponent.body;
                GameObject victimGameObject = victimHealthComponent.gameObject;
                HurtBox victimHurtBox = hitInfo.hitHurtBox;

                //Victim dotController
                DotController dotController = DotController.FindDotController(victimGameObject);
                int currentBuildupStacks = GetBuildupStackCount(dotController);

                if (currentBuildupStacks >= MaxBuildupStacks)
                {
                    ConvertBuildupToAfterburn(victimGameObject, victimHurtBox, isCrit, dotController, victimCharacterBody);
                    return returnValue;
                }

                applyBuildup();
                if (isCrit)
                {
                    applyBuildup();
                }

                currentBuildupStacks = GetBuildupStackCount(dotController);

                if (currentBuildupStacks >= MaxBuildupStacks)
                {
                    ConvertBuildupToAfterburn(victimGameObject, victimHurtBox, isCrit, dotController, victimCharacterBody);
                }
                int buildupCount = GetBuildupStackCount(dotController);


                if (buildupCount >= 10)
                {
                    clearBuildup(dotController, victimCharacterBody);
                    applyAfterburn();
                }

                void applyBuildup()
                {
                    DotController.InflictDot(
                    victimGameObject,
                    gameObject,
                    victimHurtBox,
                    InfernusDebuffs.afterburnBuildupIndex,
                    BuildupDuration,
                    0f
                    );
                }

                void applyAfterburn()
                {
                    // Apply real afterburn
                    if (isCrit != false) { 
                        DotController.InflictDot(
                            victimGameObject,
                            gameObject, //Attacker (us) gameobject
                            victimHurtBox,
                            InfernusDebuffs.afterburnDebuffIndex,
                            AfterburnDuration,
                            0f
                        );
                    }
                    else
                    {
                        DotController.InflictDot(
                            victimGameObject,
                            gameObject, //Attacker (us) gameobject
                            victimHurtBox,
                            InfernusDebuffs.afterburnDebuffIndex,
                            AfterburnDuration,
                            1f
                        );
                    }
                }


                int GetBuildupStackCount(DotController controller)
                {
                    if (controller == null) return 0;

                    int count = 0;
                    var stacks = controller.dotStackList;

                    for (int i = 0; i < stacks.Count; i++)
                    {
                        if (stacks[i].dotIndex == InfernusDebuffs.afterburnBuildupIndex)
                        {
                            count++;
                        }
                    }

                    return count;
                }

                void clearBuildup(DotController controller, CharacterBody body)
                {
                    // Remove buildup dot stacks manually
                    for (int i = controller.dotStackList.Count - 1; i >= 0; i--)
                    {
                        if (controller.dotStackList[i].dotIndex == InfernusDebuffs.afterburnBuildupIndex)
                        {
                            controller.dotStackList.RemoveAt(i);
                        }
                    }

                    // Clear associated buff visuals
                    body.ClearTimedBuffs(InfernusDebuffs.afterburnBuildup);
                }

                void ConvertBuildupToAfterburn(GameObject victim, HurtBox hurtBox, bool isCritting, DotController controller, CharacterBody body)
                {
                    clearBuildup(controller, body);

                    float damageMultiplier = isCritting ? 0.5f : 1f;

                    damageMultiplier *= FlameCanisterMult();

                    DotController.InflictDot(
                        victim,
                        gameObject,
                        hurtBox,
                        InfernusDebuffs.afterburnDebuffIndex,
                        AfterburnDuration,
                        damageMultiplier
                    );
                }


                return returnValue;
            };
        }

        public float FlameCanisterMult()
        {
            float stacks = 1f;
            ItemDef itemDef = DLC1Content.Items.StrengthenBurn;
            int ItemCount = characterBody.inventory.GetItemCountEffective(itemDef);
            if (ItemCount > 0)
            {
                stacks *= ItemCount;
                stacks *= 4;
            }

            return stacks;
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}