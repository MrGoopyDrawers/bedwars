using MelonLoader;
using BTD_Mod_Helper;
using BTD_Mod_Helper.Api.Towers;
using Il2CppAssets.Scripts.Models.TowerSets;
using Il2CppAssets.Scripts.Models.Towers;
using BTD_Mod_Helper.Extensions;
using BTD_Mod_Helper.Api.Display;
using Il2CppAssets.Scripts.Unity.Display;
using BTD_Mod_Helper.Api.Enums;
using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Weapons.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Emissions;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;
using System.Collections.Generic;
using UnityEngine;
using Il2Cpp;
using BTD_Mod_Helper.Api;
using Il2CppAssets.Scripts.Models;
using Il2CppAssets.Scripts.Simulation.Objects;
using Il2CppAssets.Scripts.Simulation.Towers;
using Il2CppAssets.Scripts.Simulation.Towers.Weapons;
using Random = System.Random;
using System.Linq;
using Il2CppAssets.Scripts.Models.GenericBehaviors;
using Il2CppAssets.Scripts.Simulation.SMath;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities;
using Il2CppAssets.Scripts.Models.Towers.TowerFilters;
using Il2CppAssets.Scripts.Models.Map;
using Il2CppAssets.Scripts.Models.Audio;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppAssets.Scripts.Models.Towers.Filters;
using HarmonyLib;
using BedwarsTowers;
using JetBrains.Annotations;
using Il2CppSystem.IO;
using Il2CppAssets.Scripts.Simulation.Towers.Behaviors;
using Il2CppAssets.Scripts.Utils;
using System.Runtime.CompilerServices;
using Il2CppAssets.Scripts.Models.Bloons.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Emissions.Behaviors;

[assembly: MelonInfo(typeof(BedwarsTowers.BedwarsTowers), ModHelperData.Name, ModHelperData.Version, ModHelperData.RepoOwner)]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]

namespace BedwarsTowers;

public class BedwarsTowers : BloonsTD6Mod
{
    public override void OnApplicationStart()
    {
        ModHelper.Msg<BedwarsTowers>("BedwarsTowers loaded!");
    }
    public class BedwarsSet : ModTowerSet
    {
        public override string DisplayName => "Bedwars";

        public override bool AllowInRestrictedModes => true;

        public override int GetTowerSetIndex(List<TowerSet> towerSets) => towerSets.IndexOf(TowerSet.Primary) + 1;
    }
    public class BarbarianMod
    {
        public class Barbarian : ModTower<BedwarsSet>
        {
            public override bool Use2DModel => true;
            public override string BaseTower => TowerType.DartMonkey;
            public override int Cost => 550;

            public override int TopPathUpgrades => 0;
            public override int MiddlePathUpgrades => 5;
            public override int BottomPathUpgrades => 0;
            public override string Description => "Get swords by purchasing upgrades. Emerald Sword is replaced by the Rageblade doing alot of damage";
            public override string DisplayName => "Barbarian";
            public override void ModifyBaseTowerModel(TowerModel towerModel)
            {
                towerModel.RemoveBehavior<AttackModel>();
                towerModel.displayScale *= 0.8f;
                var newAttackModel = Game.instance.model.GetTowerFromId("Sauda").GetAttackModel().Duplicate();
                towerModel.range = Game.instance.model.GetTowerFromId("Sauda").range;
                newAttackModel.weapons[0].projectile.GetDamageModel().damage += 1;
                towerModel.AddBehavior(newAttackModel);
            }
            public override string Get2DTexture(int[] tiers)
            {
                return "Barbarian-Icon";
            }
            public class stonesword : ModUpgrade<Barbarian>
            {
                public override int Path => MIDDLE;
                public override int Tier => 1;
                public override int Cost => 500;
                public override string Icon => "stoneSword";
                public override string DisplayName => "Stone Sword";

                public override string Description => "Upgrade to a stone sword for more damage.";

                public override void ApplyUpgrade(TowerModel towerModel)
                {
                    towerModel.GetAttackModel().weapons[0].projectile.GetDamageModel().damage += 2;
                }
            }
            public class ironsword : ModUpgrade<Barbarian>
            {
                public override int Path => MIDDLE;
                public override int Tier => 2;
                public override int Cost => 1000;
                public override string Icon => "ironSword";
                public override string DisplayName => "Iron Sword";

                public override string Description => "Upgrade to a iron sword for more damage and pierce.";

                public override void ApplyUpgrade(TowerModel towerModel)
                {
                    towerModel.GetAttackModel().weapons[0].projectile.GetDamageModel().damage += 2;
                    towerModel.GetAttackModel().weapons[0].projectile.pierce += 3;
                }
            }
            public class diamondSword : ModUpgrade<Barbarian>
            {
                public override int Path => MIDDLE;
                public override int Tier => 3;
                public override int Cost => 2500;
                public override string Icon => "dimSword";
                public override string DisplayName => "Diamond Sword";

                public override string Description => "Upgrade to a diamond sword for further increased damage.";

                public override void ApplyUpgrade(TowerModel towerModel)
                {
                    towerModel.GetAttackModel().weapons[0].projectile.GetDamageModel().damage += 4;
                }
            }
            public class rageBlade : ModUpgrade<Barbarian>
            {
                public override int Path => MIDDLE;
                public override int Tier => 4;
                public override int Cost => 12500;
                public override string Icon => "rageBlade";
                public override string DisplayName => "Rageblade";

                public override string Description => "Upgrade to a rageblade for lead popping power and much more damage. Also causes a damage over time effect from fire.";

                public override void ApplyUpgrade(TowerModel towerModel)
                {
                    towerModel.GetAttackModel().weapons[0].projectile.GetDamageModel().damage += 15;
                    towerModel.GetAttackModel().weapons[0].projectile.GetDamageModel().immuneBloonProperties = BloonProperties.None;
                    towerModel.GetAttackModel().weapons[0].projectile.collisionPasses = new int[] { -1, 0 };
                    var LavaBehavior = Game.instance.model.GetTowerFromId("Alchemist").GetDescendant<AddBehaviorToBloonModel>().Duplicate();
                    LavaBehavior.GetBehavior<DamageOverTimeModel>().interval = 1 / 12f;
                    LavaBehavior.lifespan = 10;
                    LavaBehavior.lifespanFrames = 600;
                    LavaBehavior.overlayType = "Fire";
                    towerModel.GetAttackModel().weapons[0].projectile.AddBehavior(LavaBehavior);
                    towerModel.GetAttackModel().weapons[0].projectile.GetDescendant<DamageOverTimeModel>().damage += 5f;
                }
            }
            public class voidRage : ModUpgrade<Barbarian>
            {
                public override int Path => MIDDLE;
                public override int Tier => 5;
                public override int Cost => 50000;
                public override string Icon => "voidRage";
                public override string DisplayName => "Dark Void Rageblade";

                public override string Description => "Upgrade to a dark void rageblade for even more damage.";

                public override void ApplyUpgrade(TowerModel towerModel)
                {
                    towerModel.GetAttackModel().weapons[0].projectile.GetDamageModel().damage += 50;
                    towerModel.GetAttackModel().weapons[0].projectile.GetDescendant<DamageOverTimeModel>().damage += 75f;
                }
            }
        }
    }
    public class HannahMod
    {
        public class Hannah : ModTower<BedwarsSet>
        {
            public override bool Use2DModel => true;
            public override string BaseTower => TowerType.DartMonkey;
            public override int Cost => 700;

            public override int TopPathUpgrades => 0;
            public override int MiddlePathUpgrades => 5;
            public override int BottomPathUpgrades => 0;
            public override string Description => "Activate your Execute ability to defeat a nearby bloon with low health! Also has a small cutlass attack.";
            public override string DisplayName => "Hannah";
            public override void ModifyBaseTowerModel(TowerModel towerModel)
            {
                towerModel.RemoveBehavior<AttackModel>();
                var newAttackModel = Game.instance.model.GetTowerFromId("Sauda").GetAttackModel().Duplicate();
                towerModel.range = Game.instance.model.GetTowerFromId("Sauda").range;
                towerModel.AddBehavior(newAttackModel);
                newAttackModel.RemoveBehavior<TargetFirstModel>();
                newAttackModel.RemoveBehavior<TargetLastModel>();
                newAttackModel.RemoveBehavior<TargetCloseModel>();
                var abilityModel = new AbilityModel("AbilityModel_Execute", "Execute",
                "Executes a nearby bloon", 1, 0,
                GetSpriteReference("executeIcon"), 15f, null, false, false, null,
                0, 0, 9999999, false, false);
                towerModel.AddBehavior(abilityModel);
                var activateAttackModel = new ActivateAttackModel("ActivateAttackModel_Execute", 0, true,
                new Il2CppReferenceArray<AttackModel>(1), true, false, false, false, false);
                abilityModel.AddBehavior(activateAttackModel);
                var attackModel = activateAttackModel.attacks[0] =
                Game.instance.model.GetTower(TowerType.SniperMonkey,0 ,1, 0).GetAttackModel().Duplicate();
                activateAttackModel.AddChildDependant(attackModel);
                attackModel.behaviors = attackModel.behaviors
                .RemoveItemOfType<Model, TargetFirstModel>()
                .RemoveItemOfType<Model, TargetLastModel>()
                .RemoveItemOfType<Model, TargetCloseModel>();
                var targetFirstModel = attackModel.GetBehavior<TargetStrongPrioCamoModel>();
                attackModel.targetProvider = targetFirstModel;
                attackModel.range = 2000;
                attackModel.attackThroughWalls = true;
                var weapon = attackModel.weapons[0];
                weapon.emission.AddBehavior(
                    new EmissionRotationOffBloonDirectionModel("EmissionRotationOffBloonDirectionModel", false, false));
                abilityModel.dontShowStacked = true;
                var projectileModel = weapon.projectile;
                projectileModel.GetDamageModel().damage = 9999999f;
                projectileModel.GetDamageModel().immuneBloonProperties = BloonProperties.None;
                projectileModel.AddBehavior(Game.instance.model.GetTowerFromId("BombShooter-100").GetWeapon().projectile.GetBehavior<CreateProjectileOnContactModel>().Duplicate());
                projectileModel.GetBehavior<CreateProjectileOnContactModel>().projectile.GetDamageModel().damage = 0;
                projectileModel.AddBehavior(Game.instance.model.GetTowerFromId("BombShooter-100").GetWeapon().projectile.GetBehavior<CreateSoundOnProjectileCollisionModel>().Duplicate());
                projectileModel.AddBehavior(Game.instance.model.GetTowerFromId("BombShooter-100").GetWeapon().projectile.GetBehavior<CreateEffectOnContactModel>().Duplicate());
                var bma = Game.instance.model.GetTower(TowerType.Alchemist, 0, 0, 5);
                var techTerror = Game.instance.model.GetTower(TowerType.SuperMonkey, 0, 4);
                var shrink = bma.GetAttackModel(2).Duplicate();
                var filterModels = shrink.GetBehavior<AttackFilterModel>().filters.ToList();
                var soundModel = Game.instance.model.GetTower(TowerType.BoomerangMonkey, 0, 4, 0).GetAbility().GetBehavior<CreateSoundOnAbilityModel>().Duplicate();
                soundModel.sound.assetId = GetAudioSourceReference<BedwarsTowers>("Hannah_Execute");
                abilityModel.AddBehavior(soundModel);
                filterModels.Add(new FilterOutTagModel("FilterOutTagModel_5", "Moab", new Il2CppStringArray(0)));
                filterModels.Add(new FilterOutTagModel("FilterOutTagModel_6", "Ddt", new Il2CppStringArray(0)));
                filterModels.Add(new FilterOutTagModel("FilterOutTagModel_7", "Zomg", new Il2CppStringArray(0)));
                filterModels.Add(new FilterOutTagModel("FilterOutTagModel_8", "Bfb", new Il2CppStringArray(0)));
                filterModels.Add(new FilterOutTagModel("FilterOutTagModel_9", "Ceramic", new Il2CppStringArray(0)));
                filterModels.Add(new FilterOutTagModel("FilterOutTagModel_10", "Zomg", new Il2CppStringArray(0)));
                abilityModel.GetDescendant<AttackFilterModel>().filters = filterModels.ToIl2CppReferenceArray();
            }
            public override string Get2DTexture(int[] tiers)
            {
                return "Hannah-Icon";
            }
            public class ghostCutlass : ModUpgrade<Hannah>
            {
                public override int Path => MIDDLE;
                public override int Tier => 1;
                public override int Cost => 350;
                public override string Icon => "ghostCutlass";
                public override string DisplayName => "Ghost Cutlass";

                public override string Description => "Ghost cutlass slashes faster.";

                public override void ApplyUpgrade(TowerModel towerModel)
                {
                    towerModel.GetAttackModel().weapons[0].rate *= 0.7f;
                }
            }
            public class improvedExecute : ModUpgrade<Hannah>
            {
                public override int Path => MIDDLE;
                public override int Tier => 2;
                public override int Cost => 750;
                public override string Icon => "executeImage";
                public override string DisplayName => "Improved Execution";

                public override string Description => "Can now execute ceramics and execute faster";

                public override void ApplyUpgrade(TowerModel towerModel)
                {
                    var attackFilterModel = towerModel.GetAbility().GetDescendant<AttackFilterModel>();
                    var filterModels = attackFilterModel.filters.ToList();
                    filterModels.RemoveAll(model => model.IsType<FilterOutTagModel>(out var tag) && tag.tag == "Ceramic");
                    attackFilterModel.filters = filterModels.ToIl2CppReferenceArray();
                    towerModel.GetAbility().cooldown *= 0.7f;
                }
            }
            public class MOABexecute : ModUpgrade<Hannah>
            {
                public override int Path => MIDDLE;
                public override int Tier => 3;
                public override int Cost => 2500;
                public override string Icon => "maimMoab";
                public override string DisplayName => "MOAB Execution";

                public override string Description => "Can now execute MOABS and do more damage to them.";

                public override void ApplyUpgrade(TowerModel towerModel)
                {
                    towerModel.GetAttackModel().weapons[0].projectile.hasDamageModifiers = true;
                    towerModel.GetAttackModel().weapons[0].projectile.AddBehavior(new DamageModifierForTagModel("damageModifier_","Moabs",2f,5f,false,false));
                    var attackFilterModel = towerModel.GetAbility().GetDescendant<AttackFilterModel>();
                    var filterModels = attackFilterModel.filters.ToList();
                    filterModels.RemoveAll(model => model.IsType<FilterOutTagModel>(out var tag) && tag.tag == "Moab");
                    attackFilterModel.filters = filterModels.ToIl2CppReferenceArray();
                }
            }
            public class BlunderBuss : ModUpgrade<Hannah>
            {
                public override int Path => MIDDLE;
                public override int Tier => 4;
                public override int Cost => 7500;
                public override string Icon => "blunderBuss";
                public override string DisplayName => "Blunderbuss";

                public override string Description => "Can now shoot stunning bullets at bloons. Can also execute a BFB or DDT (if has external camo detection).";

                public override void ApplyUpgrade(TowerModel towerModel)
                {
                    var sniperAttack = Game.instance.model.GetTowerFromId("SniperMonkey-110").GetAttackModel().Duplicate();
                    var bloonImpact = Game.instance.model.GetTower(TowerType.BombShooter, 4);
                    var slowModel = bloonImpact.GetDescendant<SlowModel>().Duplicate();
                    var slowModifierForTagModel = bloonImpact.GetDescendant<SlowModifierForTagModel>().Duplicate();
                    sniperAttack.weapons[0].projectile.AddBehavior(slowModel);
                    sniperAttack.weapons[0].projectile.AddBehavior(slowModifierForTagModel);
                    sniperAttack.weapons[0].projectile.collisionPasses = new[] { -1, 0 };
                    towerModel.AddBehavior(sniperAttack);
                    var attackFilterModel = towerModel.GetAbility().GetDescendant<AttackFilterModel>();
                    var filterModels = attackFilterModel.filters.ToList();
                    filterModels.RemoveAll(model => model.IsType<FilterOutTagModel>(out var tag) && tag.tag == "Ddt");
                    filterModels.RemoveAll(model => model.IsType<FilterOutTagModel>(out var tag) && tag.tag == "Bfb");
                    attackFilterModel.filters = filterModels.ToIl2CppReferenceArray();
                }
            }
            public class BloodForBlood : ModUpgrade<Hannah>
            {
                public override int Path => MIDDLE;
                public override int Tier => 5;
                public override int Cost => 70000;
                public override string Icon => "skull";
                public override string DisplayName => "Blood For Blood";

                public override string Description => "Regularly executes a ZOMG, and has a shorter cooldown. Adds a secret ability that can takedown the biggest of bloons.";

                public override void ApplyUpgrade(TowerModel towerModel)
                {
                    towerModel.GetAbility().cooldown *= 0.7f;
                    var attackFilterModel = towerModel.GetAbility().GetDescendant<AttackFilterModel>();
                    var filterModels = attackFilterModel.filters.ToList();
                    filterModels.RemoveAll(model => model.IsType<FilterOutTagModel>(out var tag) && tag.tag == "Zomg");
                    attackFilterModel.filters = filterModels.ToIl2CppReferenceArray();
                    var newAbility = towerModel.GetAbility().Duplicate();
                    newAbility.GetBehavior<ActivateAttackModel>().attacks[0].RemoveBehavior<AttackFilterModel>();
                    newAbility.icon = GetSpriteReference("ghostCutlass");
                    newAbility.maxActivationsPerRound = 1;
                    newAbility.cooldown *= 2;
                    newAbility.dontShowStacked = true;
                    newAbility.resetCooldownOnTierUpgrade = true;
                    towerModel.AddBehavior(newAbility);
                }
            }
        }
    }
    public class KaliyahMod
    {
        public class Kaliyah : ModTower<BedwarsSet>
        {
            public override bool Use2DModel => true;
            public override string BaseTower => TowerType.DartMonkey;
            public override int Cost => 800;

            public override int TopPathUpgrades => 0;
            public override int MiddlePathUpgrades => 5;
            public override int BottomPathUpgrades => 0;
            public override string Description => "A fearsome warrior with a fiery punch!";
            public override string DisplayName => "Kaliyah";
            public override void ModifyBaseTowerModel(TowerModel towerModel)
            {
                towerModel.RemoveBehavior<AttackModel>();
                towerModel.displayScale *= 0.45f;
                var newAttackModel = Game.instance.model.GetTowerFromId("Sauda").GetAttackModel().Duplicate();
                newAttackModel.weapons[0].RemoveBehavior<CreateSoundOnProjectileCreatedModel>();
                towerModel.range = Game.instance.model.GetTowerFromId("Sauda").range;
                towerModel.AddBehavior(newAttackModel);
                var abilityModel = new AbilityModel("AbilityModel_Punch", "Punch",
                "Punches a nearby bloon.", 1, 0,
                GetSpriteReference("punchIcon"), 7f, null, false, false, null,
                0, 0, 9999999, false, false);
                towerModel.AddBehavior(abilityModel);
                var activateAttackModel = new ActivateAttackModel("ActivateAttackModel_Punch", 0, true,
                new Il2CppReferenceArray<AttackModel>(1), true, false, false, false, false);
                abilityModel.AddBehavior(activateAttackModel);
                var attackModel = activateAttackModel.attacks[0] =
                Game.instance.model.GetTower(TowerType.DartMonkey,0,0,2).GetAttackModel().Duplicate();
                activateAttackModel.AddChildDependant(attackModel);
                attackModel.behaviors = attackModel.behaviors
                .RemoveItemOfType<Model, TargetFirstModel>()
                .RemoveItemOfType<Model, TargetLastModel>()
                .RemoveItemOfType<Model, TargetCloseModel>();
                var targetFirstModel = attackModel.GetBehavior<TargetStrongPrioCamoModel>();
                attackModel.targetProvider = targetFirstModel;
                attackModel.range = towerModel.range * 2;
                attackModel.attackThroughWalls = true;
                var weapon = attackModel.weapons[0];
                weapon.emission.AddBehavior(
                    new EmissionRotationOffBloonDirectionModel("EmissionRotationOffBloonDirectionModel", false, false));

                
                var projectileModel = weapon.projectile;
                projectileModel.pierce = 1;
                projectileModel.scale *= 0.0001f;
                projectileModel.GetDamageModel().damage = 4f;
                projectileModel.GetDamageModel().immuneBloonProperties = BloonProperties.None;
                projectileModel.collisionPasses = new int[] { -1, 0 };
                projectileModel.AddBehavior(new WindModel("Punch",30f,50f,10000f,false,null,1,null,1.5f));
                var LavaBehavior = Game.instance.model.GetTowerFromId("Alchemist").GetDescendant<AddBehaviorToBloonModel>().Duplicate();
                LavaBehavior.GetBehavior<DamageOverTimeModel>().interval = 1 / 3f;
                LavaBehavior.lifespan = 10;
                LavaBehavior.lifespanFrames = 600;
                LavaBehavior.overlayType = "Fire";
                projectileModel.AddBehavior(LavaBehavior);
                projectileModel.GetDescendant<DamageOverTimeModel>().damage += 2f;
                projectileModel.AddBehavior(Game.instance.model.GetTowerFromId("BombShooter").GetWeapon().projectile.GetBehavior<CreateProjectileOnContactModel>().Duplicate());
                projectileModel.GetBehavior<CreateProjectileOnContactModel>().projectile.GetDamageModel().damage = 0;
                projectileModel.AddBehavior(Game.instance.model.GetTowerFromId("BombShooter").GetWeapon().projectile.GetBehavior<CreateSoundOnProjectileCollisionModel>().Duplicate());
                projectileModel.AddBehavior(Game.instance.model.GetTowerFromId("BombShooter").GetWeapon().projectile.GetBehavior<CreateEffectOnContactModel>().Duplicate());
                var soundModel = Game.instance.model.GetTower(TowerType.BoomerangMonkey, 0, 4, 0).GetAbility().GetBehavior<CreateSoundOnAbilityModel>().Duplicate();
                soundModel.sound.assetId = GetAudioSourceReference<BedwarsTowers>("kaliyahPunch");
                towerModel.GetDescendants<FilterInvisibleModel>().ForEach(model => model.isActive = false);
                abilityModel.AddBehavior(soundModel);
            }
            public override string Get2DTexture(int[] tiers)
            {
                return "Kaliyah-Icon";
            }
            public class fireEnchant : ModUpgrade<Kaliyah>
            {
                public override int Path => MIDDLE;
                public override int Tier => 1;
                public override int Cost => 550;
                public override string Icon => "fireEnchant";
                public override string DisplayName => "Fire I";

                public override string Description => "Fire enchantment enhances the damage over time effect of the ability.";

                public override void ApplyUpgrade(TowerModel towerModel)
                {
                    var ability = towerModel.GetAbility();
                    var abilityAttack = ability.GetBehavior<ActivateAttackModel>().attacks[0];
                    var proj = abilityAttack.weapons[0].projectile;
                    proj.GetDescendant<DamageOverTimeModel>().damage += 2f;
                    proj.GetDescendant<DamageOverTimeModel>().interval = 1 / 6f;
                }
            }
            public class betterKnockback : ModUpgrade<Kaliyah>
            {
                public override int Path => MIDDLE;
                public override int Tier => 2;
                public override int Cost => 650;
                public override string Icon => "slap";
                public override string DisplayName => "Enhanced Knockback";

                public override string Description => "Knocks bloons further with the ability.";

                public override void ApplyUpgrade(TowerModel towerModel)
                {
                    var ability = towerModel.GetAbility();
                    var abilityAttack = ability.GetBehavior<ActivateAttackModel>().attacks[0];
                    var proj = abilityAttack.weapons[0].projectile;
                    proj.GetBehavior<WindModel>().distanceMin *= 1.4f;
                    proj.GetBehavior<WindModel>().distanceMax *= 1.4f;
                }
            }
            public class wind : ModUpgrade<Kaliyah>
            {
                public override int Path => MIDDLE;
                public override int Tier => 3;
                public override int Cost => 2500;
                public override string Icon => "wind";
                public override string DisplayName => "Wind III";

                public override string Description => "Very fast attack speed. Fiery Punch cooldown reduced.";

                public override void ApplyUpgrade(TowerModel towerModel)
                {
                    var attackModel = towerModel.GetAttackModel();
                    attackModel.weapons[0].rate *= 0.2f;
                    var ability = towerModel.GetAbility();
                    ability.cooldown *= 0.6f;

                }
            }
            public class betterPunch : ModUpgrade<Kaliyah>
            {
                public override int Path => MIDDLE;
                public override int Tier => 4;
                public override int Cost => 12500;
                public override string Icon => "punchIcon5";
                public override string DisplayName => "Stronger Force";

                public override string Description => "Increased punch force knocks bloons even further, including MOABs. Fiery punch does triple damage.";

                public override void ApplyUpgrade(TowerModel towerModel)
                {
                    var ability = towerModel.GetAbility();
                    var abilityAttack = ability.GetBehavior<ActivateAttackModel>().attacks[0];
                    var proj = abilityAttack.weapons[0].projectile;
                    proj.GetBehavior<WindModel>().distanceMin *= 1.12f;
                    proj.GetBehavior<WindModel>().distanceMax *= 1.12f;
                    proj.GetBehavior<WindModel>().affectMoab = true;
                    proj.GetDamageModel().damage *= 4;
                }
            }
            public class megaPunch : ModUpgrade<Kaliyah>
            {
                public override int Path => MIDDLE;
                public override int Tier => 5;
                public override int Cost => 85000;
                public override string Icon => "flamingFist";
                public override string DisplayName => "Erupting Flaming Fist";

                public override string Description => "Her flaming fist is able to really hurt bloons, fire, and one off damage.";

                public override void ApplyUpgrade(TowerModel towerModel)
                {
                    var attackModel = towerModel.GetAttackModel();
                    attackModel.weapons[0].projectile.GetDamageModel().damage = 50f;
                    var ability = towerModel.GetAbility();
                    var abilityAttack = ability.GetBehavior<ActivateAttackModel>().attacks[0];
                    var proj = abilityAttack.weapons[0].projectile;
                    proj.GetDamageModel().damage = 1500f;
                    proj.GetDescendant<DamageOverTimeModel>().damage = 150f;
                }
            }
        }
    }
    public class VanessaMod
    {
        public class Vanessa : ModTower<BedwarsSet>
        {
            public override bool Use2DModel => true;
            public override string BaseTower => "DartMonkey-003";
            public override int Cost => 540;

            public override int TopPathUpgrades => 0;
            public override int MiddlePathUpgrades => 5;
            public override int BottomPathUpgrades => 0;
            public override string Description => "Super-charges her bow to perform a triple shot.";
            public override string DisplayName => "Vanessa";
            public override void ModifyBaseTowerModel(TowerModel towerModel)
            {
                var attackModel = towerModel.GetAttackModel();
                towerModel.range += 20;
                attackModel.range += 20;
                var openSound = Game.instance.model.GetTowerFromId("Sauda").GetAttackModel().weapons[0].GetBehavior<CreateSoundOnProjectileCreatedModel>().Duplicate();
                openSound.sound1.assetId = GetAudioSourceReference<BedwarsTowers>("vanessaCharge");
                openSound.sound2.assetId = GetAudioSourceReference<BedwarsTowers>("vanessaCharge");
                openSound.sound3.assetId = GetAudioSourceReference<BedwarsTowers>("vanessaCharge");
                openSound.sound4.assetId = GetAudioSourceReference<BedwarsTowers>("vanessaCharge");
                openSound.sound5.assetId = GetAudioSourceReference<BedwarsTowers>("vanessaCharge");
                attackModel.weapons[0].AddBehavior(openSound);
                attackModel.weapons[0].rate *= 2.3f;
                attackModel.weapons[0].projectile.pierce = 1;
                var collideSound = Game.instance.model.GetTowerFromId("GlueGunner").GetWeapon().projectile.GetBehavior<CreateSoundOnProjectileCollisionModel>().Duplicate();
                collideSound.sound1.assetId = GetAudioSourceReference<BedwarsTowers>("vanessaHit");
                collideSound.sound2.assetId = GetAudioSourceReference<BedwarsTowers>("vanessaHit");
                collideSound.sound3.assetId = GetAudioSourceReference<BedwarsTowers>("vanessaHit");
                collideSound.sound4.assetId = GetAudioSourceReference<BedwarsTowers>("vanessaHit");
                collideSound.sound5.assetId = GetAudioSourceReference<BedwarsTowers>("vanessaHit");
                attackModel.weapons[0].projectile.AddBehavior(collideSound);
                attackModel.weapons[0].emission = new EmissionOverTimeModel("ArcEmissionModel_", 3, 0.15f, null);
            }
            public override string Get2DTexture(int[] tiers)
            {
                return "Vanessa-Icon";
            }
            public class accuracy : ModUpgrade<Vanessa>
            {
                public override int Path => MIDDLE;
                public override int Tier => 1;
                public override int Cost => 250;
                public override string Icon => "crosshair";
                public override string DisplayName => "Increased Accuracy";

                public override string Description => "Her shots are slightly more accurate";

                public override void ApplyUpgrade(TowerModel towerModel)
                {
                    var attackModel = towerModel.GetAttackModel();
                    attackModel.weapons[0].projectile.GetBehavior<TravelStraitModel>().speed *= 5f;
                    attackModel.weapons[0].emission = new EmissionOverTimeModel("ArcEmissionModel_", 3, 0.07f, null);
                    attackModel.weapons[0].projectile.radius *= 2;
                }
            }
            public class FasterFiring : ModUpgrade<Vanessa>
            {
                public override int Path => MIDDLE;
                public override int Tier => 2;
                public override int Cost => 550;
                public override string Icon => "bowIcon";
                public override string DisplayName => "Faster Firing";

                public override string Description => "Activates her triple shot more often";

                public override void ApplyUpgrade(TowerModel towerModel)
                {
                    var attackModel = towerModel.GetAttackModel();
                    attackModel.weapons[0].rate *= 0.7f;
                }
            }
            public class Knockback : ModUpgrade<Vanessa>
            {
                public override int Path => MIDDLE;
                public override int Tier => 3;
                public override int Cost => 2500;
                public override string Icon => "knockback_van";
                public override string DisplayName => "Knockback";

                public override string Description => "Each arrow can knockback bloons.";

                public override void ApplyUpgrade(TowerModel towerModel)
                {
                    var attackModel = towerModel.GetAttackModel();
                    attackModel.weapons[0].projectile.AddBehavior(new WindModel("wind",10f,20f,1000f,false,null,1,null,3));
                }
            }
            public class Crossbow : ModUpgrade<Vanessa>
            {
                public override int Path => MIDDLE;
                public override int Tier => 4;
                public override int Cost => 7500;
                public override string Icon => "crossbow";
                public override string DisplayName => "Crossbow";

                public override string Description => "Each arrow deals increased damage to Ceramic and Moab Class.";

                public override void ApplyUpgrade(TowerModel towerModel)
                {
                    var attackModel = towerModel.GetAttackModel();
                    attackModel.weapons[0].projectile.hasDamageModifiers = true;
                    attackModel.weapons[0].projectile.AddBehavior(new DamageModifierForTagModel("damageModifier_", "Moabs", 4f, 24f, false, false));
                    attackModel.weapons[0].projectile.AddBehavior(new DamageModifierForTagModel("damageModifier_2", "Ceramic", 3f, 4f, false, false));
                }
            }
            public class Headhunter : ModUpgrade<Vanessa>
            {
                public override int Path => MIDDLE;
                public override int Tier => 5;
                public override int Cost => 65000;
                public override string Icon => "headHunter";
                public override string DisplayName => "Headhunter";

                public override string Description => "Headhunter allows arrows to crit sometimes, and increases damage heavily around BADs.";

                public override void ApplyUpgrade(TowerModel towerModel)
                {
                    var attackModel = towerModel.GetAttackModel();
                    attackModel.weapons[0] = Game.instance.model.GetTowerFromId("DartMonkey-004").GetWeapon().Duplicate();
                    attackModel.weapons[0].projectile.hasDamageModifiers = true;
                    attackModel.weapons[0].projectile.AddBehavior(new DamageModifierForTagModel("damageModifier_5", "Moabs", 10f, 750f, false, false));
                    attackModel.weapons[0].projectile.AddBehavior(new DamageModifierForTagModel("damageModifier_23", "Bad", 10f, 1250f, false, false));
                    attackModel.weapons[0].projectile.AddBehavior(new DamageModifierForTagModel("damageModifier_4", "Ceramic", 2f, 10f, false, false));
                    attackModel.weapons[0].projectile.AddBehavior(new WindModel("wind", 5f, 10f, 1000f, true, null, 1, null, 5));
                    attackModel.weapons[0].emission = new EmissionOverTimeModel("ArcEmissionModel_", 3, 0.05f, null);
                    var collideSound = Game.instance.model.GetTowerFromId("GlueGunner").GetWeapon().projectile.GetBehavior<CreateSoundOnProjectileCollisionModel>().Duplicate();
                    collideSound.sound1.assetId = GetAudioSourceReference<BedwarsTowers>("vanessaHit");
                    collideSound.sound2.assetId = GetAudioSourceReference<BedwarsTowers>("vanessaHit");
                    collideSound.sound3.assetId = GetAudioSourceReference<BedwarsTowers>("vanessaHit");
                    collideSound.sound4.assetId = GetAudioSourceReference<BedwarsTowers>("vanessaHit");
                    collideSound.sound5.assetId = GetAudioSourceReference<BedwarsTowers>("vanessaHit");
                    attackModel.weapons[0].projectile.AddBehavior(collideSound);
                    var openSound = Game.instance.model.GetTowerFromId("Sauda").GetAttackModel().weapons[0].GetBehavior<CreateSoundOnProjectileCreatedModel>().Duplicate();
                    openSound.sound1.assetId = GetAudioSourceReference<BedwarsTowers>("vanessaCharge");
                    openSound.sound2.assetId = GetAudioSourceReference<BedwarsTowers>("vanessaCharge");
                    openSound.sound3.assetId = GetAudioSourceReference<BedwarsTowers>("vanessaCharge");
                    openSound.sound4.assetId = GetAudioSourceReference<BedwarsTowers>("vanessaCharge");
                    openSound.sound5.assetId = GetAudioSourceReference<BedwarsTowers>("vanessaCharge");
                    attackModel.weapons[0].AddBehavior(openSound);
                    attackModel.weapons[0].projectile.pierce = 1;
                }
            }
        }
    }
}