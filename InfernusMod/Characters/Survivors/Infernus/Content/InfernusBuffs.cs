using RoR2;
using UnityEngine;

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

        public static void Init(AssetBundle assetBundle)
        {
            afterburnDebuff = Modules.Content.CreateAndAddBuff(
                "InfernusAfterburn",
                LegacyResourcesAPI.Load<BuffDef>("BuffDefs/OnFire").iconSprite,
                Color.red,
                true,
                false
            );
        }
    }
}
