using System;
using InfernusMod.Modules;
using InfernusMod.Survivors.Infernus.Achievements;

namespace InfernusMod.Survivors.Infernus
{
    public static class InfernusTokens
    {
        public static void Init()
        {
            AddInfernusTokens();

            ////uncomment this to spit out a lanuage file with all the above tokens that people can translate
            ////make sure you set Language.usingLanguageFolder and printingEnabled to true
            //Language.PrintOutput("Infernus.txt");
            ////refer to guide on how to build and distribute your mod with the proper folders
        }

        public static void AddInfernusTokens()
        {
            string prefix = InfernusSurvivor.INFERNUS_PREFIX;

            string desc = "Infernus is a bartender who utilizes his Ixian flames to light up his enemies.<color=#CCD3E0>" + Environment.NewLine + Environment.NewLine
             + "< ! > Afterburn deals lingering flame damage, its timer is reset by Infernus' other abilities." + Environment.NewLine + Environment.NewLine
             + "< ! > Napalm sweeps through crowds, those affected suffer vulnerability to all damage" + Environment.NewLine + Environment.NewLine
             + "< ! > Flame Dash is a swift engagement and disengage tool that leaves behind a fiery trail." + Environment.NewLine + Environment.NewLine
             + "< ! > Concussive Combustion uses one spark to wipe out crowds and bosses alike." + Environment.NewLine + Environment.NewLine;

            string outro = "..and so he left, to enjoy his life anew.";
            string outroFailure = "..and so he couldn't be bailed out, forever a sputtered flame.";

            Language.Add(prefix + "NAME", "Infernus");
            Language.Add(prefix + "DESCRIPTION", desc);
            Language.Add(prefix + "SUBTITLE", "The Jezebel's Bartender");
            Language.Add(prefix + "LORE", "Infernus is an Ixian with a powerful control of flame that he uses to enhance his speed and ignite his enemies. Combining his Flame Dash with the ability to spew a Napalm catalyst, Infernus can chase down and ignite his foes with a deadly Afterburn. Any targets left standing will be subject to his explosive Concussive Combustion, knocking them out and leaving them vulnerable to further immolation.\r\n\r\nAs a creature from Ixia, Infernus had a troubled and intense youth while he was raised in the United States. Despite a rash of deadly crime as a teenager, he found himself tending a bar in his adulthood and learning to calm his intense emotions. With quality music and interesting people to talk to, Infernus rarely has a need these days to call up his dangerous supernatural abilities.");
            Language.Add(prefix + "OUTRO_FLAVOR", outro);
            Language.Add(prefix + "OUTRO_FAILURE", outroFailure);

            #region Skins
            Language.Add(prefix + "MASTERY_SKIN_NAME", "Alternate");
            #endregion

            #region Passive
            Language.Add(prefix + "PASSIVE_NAME", "Afterburn");
            Language.Add(prefix + "PASSIVE_DESCRIPTION", Tokens.agilePrefix + $"Bullets build up afterburn, it deals <style=cIsDamage>{100f * InfernusStaticValues.dashDamageCoefficient}</style> damage over time.");
            #endregion

            #region Primary
            Language.Add(prefix + "PRIMARY_GUN_NAME", "Incendiary Remarks");
            Language.Add(prefix + "PRIMARY_GUN_DESCRIPTION", Tokens.agilePrefix + $"Fire flames for <style=cIsDamage>{60f * InfernusStaticValues.gunDamageCoefficient}%</style> damage.");
            #endregion

            #region Secondary
            Language.Add(prefix + "SECONDARY_NAPALM_NAME", "Napalm");
            Language.Add(prefix + "SECONDARY_NAPALM_DESCRIPTION", Tokens.agilePrefix + $"Fire a homemade cocktail for <style=cIsDamage>{100f * InfernusStaticValues.gunDamageCoefficient}%</style> damage.");
            #endregion

            #region Utility
            //Base Roll
            Language.Add(prefix + "UTILITY_ROLL_NAME", "Roll");
            Language.Add(prefix + "UTILITY_ROLL_DESCRIPTION", Tokens.agilePrefix + $"Dash forward, gaining <style=cIsUtility>20 movement speed</style>. You leave a trail of fire that burns enemies for <style=cIsDamage>{100f * InfernusStaticValues.dashDamageCoefficient}%</style> damage.");
            
            //Flame Dash
            Language.Add(prefix + "UTILITY_DASH_NAME", "Flame Dash");
            Language.Add(prefix + "UTILITY_DASH_DESCRIPTION", Tokens.agilePrefix + $"Dash forward, gaining <style=cIsUtility>20 movement speed</style>. You leave a trail of fire that burns enemies for <style=cIsDamage>{100f * InfernusStaticValues.dashDamageCoefficient}%</style> damage.");
            #endregion



            #region Special
            Language.Add(prefix + "SPECIAL_BOMB_NAME", "Concussive Combustion");
            Language.Add(prefix + "SPECIAL_BOMB_DESCRIPTION", $"Become a living bomb, unleashing <style=cIsDamage>{100f * InfernusStaticValues.bombDamageCoefficient}%</style> damage in a radius.");
            #endregion

            #region Achievements
            Language.Add(Tokens.GetAchievementNameToken(InfernusMasteryAchievement.identifier), "Infernus: Mastery");
            Language.Add(Tokens.GetAchievementDescriptionToken(InfernusMasteryAchievement.identifier), "As Infernus, beat the game or obliterate on Monsoon.");
            #endregion
        }
    }
}
