using EntityStates;
using InfernusMod.Characters.Survivors.Infernus.Content;
using InfernusMod.Survivors.Infernus;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace InfernusMod.Survivors.Infernus.SkillStates
{
    public class FlameDash : BaseSkillState
    {
        public static float duration = 3f;
        public static float initialSpeedCoefficient = 1f;
        public static float finalSpeedCoefficient = 4f;
        public static GameObject flameZonePrefab;

        public static string dodgeSoundString = "HenryRoll";
        public static float dodgeFOV = global::EntityStates.Commando.DodgeState.dodgeFOV;

        private float rollSpeed;
        private Vector3 forwardDirection;
        private Animator animator;
        private Vector3 previousPosition;

        //Dash values
        private float spawnInterval = 5f; //How far dash blazes are placed
        private float distanceTraveled = 0f; //Changing over time, how far character has traveled since last blaze placement
        private float nextSpawnDistance = 0f; //Temporary Dash value, rolls forward in parallel to distanceTraveled



        public override void OnEnter()
        {
            base.OnEnter();
            animator = GetModelAnimator();


            bool isRunning = false;

            if (isRunning == false)
            {
                isRunning = true;

                if (isAuthority && inputBank && characterDirection)
                {
                    forwardDirection = (inputBank.moveVector == Vector3.zero ? characterDirection.forward : inputBank.moveVector).normalized;
                }

                Vector3 rhs = characterDirection ? characterDirection.forward : forwardDirection;
                Vector3 rhs2 = Vector3.Cross(Vector3.up, rhs);

                float num = Vector3.Dot(forwardDirection, rhs);
                float num2 = Vector3.Dot(forwardDirection, rhs2);

                RecalculateRollSpeed();

                if (characterMotor && characterDirection)
                {
                    characterMotor.velocity.y = 0f;
                    characterMotor.velocity = forwardDirection * rollSpeed;
                }

                Vector3 b = characterMotor ? characterMotor.velocity : Vector3.zero;
                previousPosition = transform.position - b;

                PlayAnimation("FullBody, Override", "Roll", "Roll.playbackRate", duration);
                Util.PlaySound(dodgeSoundString, gameObject);

                if (NetworkServer.active)
                {
                    characterBody.AddTimedBuff(InfernusBuffs.speedBuff, 3f * duration);
                    characterBody.AddTimedBuff(RoR2Content.Buffs.HiddenInvincibility, 0.5f * duration);
                }
            }
        }

        private void SpawnFlameZone()
        {
            if (!NetworkServer.active) return;

            RaycastHit hit;
            Vector3 spawnPos = transform.position;

            if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out hit, 20f, LayerIndex.world.mask))
            {
                spawnPos = hit.point;
            }

            GameObject zone = GameObject.Instantiate(
                InfernusAssets.flameZonePrefab,
                spawnPos,
                Quaternion.identity
            );

            FlameZoneController controller = zone.GetComponent<FlameZoneController>();
            if (controller)
            {
                controller.owner = gameObject;
            }

            NetworkServer.Spawn(zone);
        }


        private void RecalculateRollSpeed()
        {
            rollSpeed = moveSpeedStat * Mathf.Lerp(initialSpeedCoefficient, finalSpeedCoefficient, fixedAge / duration);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            RecalculateRollSpeed();

            float frameDistance = Vector3.Distance(transform.position, previousPosition);
            distanceTraveled += frameDistance;

            if (distanceTraveled >= nextSpawnDistance)
            {
                SpawnFlameZone();
                nextSpawnDistance += spawnInterval;
            }


            if (isAuthority && inputBank && characterDirection)
            {
                forwardDirection = (inputBank.moveVector == Vector3.zero ? characterDirection.forward : inputBank.moveVector).normalized;
            }

            if (characterDirection) characterDirection.forward = forwardDirection;
            // CHANGEPARSE <- Keyword for undoing changes
            //if (cameraTargetParams) cameraTargetParams.fovOverride = Mathf.Lerp(dodgeFOV, 60f, fixedAge / duration);

            Vector3 normalized = (transform.position - previousPosition).normalized;
            if (characterMotor && characterDirection && normalized != Vector3.zero)
            {
                Vector3 vector = normalized * rollSpeed;
                float d = Mathf.Max(Vector3.Dot(vector, forwardDirection), 0f);
                vector = forwardDirection * d;
                vector.y = 0f;

                characterMotor.velocity.x = vector.x;
                characterMotor.velocity.z = vector.z;
            }
            previousPosition = transform.position;

            if (isAuthority && fixedAge >= duration)
            {
                outer.SetNextStateToMain();
                return;
            }
        }

        public override void OnExit()
        {
            // CHANGEPARSE <- Keyword for undoing changes
            //if (cameraTargetParams) cameraTargetParams.fovOverride = -1f;
            base.OnExit();

            characterMotor.disableAirControlUntilCollision = false;
        }

        public override void OnSerialize(NetworkWriter writer)
        {
            base.OnSerialize(writer);
            writer.Write(forwardDirection);
        }

        public override void OnDeserialize(NetworkReader reader)
        {
            base.OnDeserialize(reader);
            forwardDirection = reader.ReadVector3();
        }
    }
}