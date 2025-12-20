using RoR2;
using UnityEngine;
using R2API;

namespace InfernusMod.Survivors.Infernus
{


    public static class InfernusBuffs
    {
        // armor buff gained during roll
        public static BuffDef speedBuff;

        public static void Init(AssetBundle assetBundle)
        {
            speedBuff = Modules.Content.CreateAndAddBuff("InfernusSpeedBuff",
                LegacyResourcesAPI.Load<BuffDef>("BuffDefs/WhipBoost").iconSprite,
                Color.white,
                false,
                false);

        }
    }

    public static class InfernusDebuffs
    {
        public static BuffDef afterburnDebuff;
        public static BuffDef afterburnBuildup;

        public static DotController.DotIndex afterburnDebuffIndex = DotAPI.RegisterDotDef(0.25f, 0.25f, DamageColorIndex.Bleed, afterburnDebuff);

        public static void Init(AssetBundle assetBundle)
        {

            afterburnDebuff = Modules.Content.CreateAndAddBuff(
                "InfernusAfterburn",
                LegacyResourcesAPI.Load<BuffDef>("BuffDefs/OnFire").iconSprite,
                Color.red,
                false,
                true
            );

            afterburnBuildup = Modules.Content.CreateAndAddBuff(
                "InfernusBuildup",
                assetBundle.LoadAsset<Sprite>("texBazookaFireIcon"),
                Color.black,
                true,
                true
            );

            float afterburnDamageMult = 1f;

            DotController.DotDef afterburnDot = new DotController.DotDef
            {
                associatedBuff = afterburnDebuff,
                damageCoefficient = afterburnDamageMult,
                interval = 0.5f,
                damageColorIndex = DamageColorIndex.Void,
                resetTimerOnAdd = true
            };

            //Way to add afterburn using these definitions
            DotController.DotIndex afterburnDebuffIndex = DotAPI.RegisterDotDef(0.25f, 0.25f, DamageColorIndex.Bleed, afterburnDebuff);

        }
    }
}
