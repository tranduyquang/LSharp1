using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Runtime.CompilerServices;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Igniter;
using System.ComponentModel;
using System.Reflection;

namespace Tonghop
{
    class Program
    {

        private static Menu Config;
        
        public static TargetSelector TS;

        private static Items.Item riumx;

        private static Items.Item guomvd;

        private static Items.Item sunghex;

        private static Items.Item kiemma;
        
        private static Items.Item khien;
        
        private static Items.Item ythien;

        private static Items.Item guomht;
        
        private static Items.Item buadl;
        
        private static Items.Item tiamat;
        
        private static Obj_AI_Hero tuong;
        private static SpellSlot IgniteSlot;

        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += OnGameLoad;
        }

        private static void OnGameLoad(EventArgs args) 
        {
            tuong = ObjectManager.Player;
            
            TS = new TargetSelector(1000, TargetSelector.TargetingMode.AutoPriority);
            
            IgniteSlot = tuong.GetSpellSlot("SummonerDot");

            riumx = new Items.Item(3074, 175f);
            tiamat = new Items.Item(3077, 175f);
            guomvd = new Items.Item(3153, 450f);
            guomht = new Items.Item(3144, 450f);
            khien = new Items.Item(3143, 500f);
            sunghex = new Items.Item(3146, 700f);
            buadl = new Items.Item(3128, 750f);
            kiemma = new Items.Item(3142, 185f);
            ythien = new Items.Item(3131, 185f);

            Config = new Menu("TongHop", "TongHop", true);

            Config.AddSubMenu(new Menu("Target Selector", "TargetSelector"));
            Config.SubMenu("TargetSelector").AddItem(new MenuItem("TargetingMode", "Mode")).SetValue(new StringList(new[] { "AutoPriority", "Closest", "LessAttack", "LessCast", "LowHP", "MostAD", "MostAP", "NearMouse" }, 1));
            Config.SubMenu("TargetSelector").AddItem(new MenuItem("TargetingRange", "Range")).SetValue(new Slider(750, 2000, 100));

            //Combo
            Config.AddSubMenu(new Menu("Smart", "Ks"));
            Config.SubMenu("Ks").AddItem(new MenuItem("UseIgnite", "Use Ignite")).SetValue(true);
            Config.SubMenu("Ks").AddItem(new MenuItem("UseExhaust", "Use Exhaust")).SetValue(true);
            Config.AddSubMenu(new Menu("UseItems", "UseItems"));
            Config.SubMenu("UseItems").AddItem(new MenuItem("UseItems", "Use Items")).SetValue(true);
            Config.SubMenu("UseItems").AddItem(new MenuItem("ActiveCombo", "UseItems").SetValue(new KeyBind(32, KeyBindType.Press)));
            Config.AddSubMenu(new Menu("AutoPotion", "AutoPotion"));
            Config.SubMenu("AutoPotion").AddItem(new MenuItem("HealthPercent", "HP Trigger Percent").SetValue(new Slider(50, 100, 0)));
            Config.SubMenu("AutoPotion").AddItem(new MenuItem("ManaPercent", "MP Trigger Percent").SetValue(new Slider(50, 100, 0)));  
            Config.AddToMainMenu();

            Game.OnGameUpdate += OnGameUpdate;

            Game.PrintChat("<font color='#1d87f2'>TongHop Loaded!</font>");
        }

        private static void OnGameUpdate(EventArgs args) 
        {
           tuong = ObjectManager.Player;
           //auto potion
            if (GetPlayerHealthPercent() < Config.Item("HealthPercent").GetValue<Slider>().Value)
                if (!ObjectManager.Player.Buffs.Any(buff => buff.Name == "RegenerationPotion" || buff.Name == "ItemCrystalFlask" || buff.Name == "ItemMiniRegenPotion"))
                    GetHealthPotionSlot().UseItem();
            
            if (GetPlayerManaPercent() < Config.Item("ManaPercent").GetValue<Slider>().Value)
                if (!ObjectManager.Player.Buffs.Any(buff => buff.Name == "ItemCrystalFlask" || buff.Name == "FlaskOfCrystalWater"))
                    GetManaPotionSlot().UseItem();
            
            //use item
            if (Config.Item("ActiveCombo").GetValue<KeyBind>().Active) 
            {
                Combo();
            }
            //ignite
            if (Config.Item("UseIgnite").GetValue<bool>()) {
                Smart();
            }
            //Exhaust
            if (Config.Item("UseExhaust").GetValue<bool>()) {
                Exhaust();
            }
        }

        private static void InternalSetTargetingMode()
        {
            float TSRange = Config.Item("TargetingRange").GetValue<Slider>().Value;
            TS.SetRange(TSRange);
            var mode = Config.Item("TargetingMode").GetValue<StringList>().SelectedIndex;

            switch (mode)
            {
                case 0:
                    TS.SetTargetingMode(TargetSelector.TargetingMode.AutoPriority);

                    break;
                case 1:
                    TS.SetTargetingMode(TargetSelector.TargetingMode.Closest);
                    break;
                case 2:
                    TS.SetTargetingMode(TargetSelector.TargetingMode.LessAttack);
                    break;
                case 3:
                    TS.SetTargetingMode(TargetSelector.TargetingMode.LessCast);
                    break;
                case 4:
                    TS.SetTargetingMode(TargetSelector.TargetingMode.LowHP);
                    break;
                case 5:
                    TS.SetTargetingMode(TargetSelector.TargetingMode.MostAD);
                    break;
                case 6:
                    TS.SetTargetingMode(TargetSelector.TargetingMode.MostAP);
                    break;
                case 7:
                    TS.SetTargetingMode(TargetSelector.TargetingMode.NearMouse);
                    break;
            }
        }
        private static void Combo() 
        {
            var target = SimpleTs.GetTarget(500, SimpleTs.DamageType.Physical);
            var target1 = SimpleTs.GetTarget(750, SimpleTs.DamageType.Magical);
            if (target != null) 
            {
                    if (Config.Item("UseItems").GetValue<bool>()) {
                    guomvd.Cast(target);
                    kiemma.Cast();
                    ythien.Cast();
                    guomht.Cast(target);
                    khien.Cast();
                    if ((tuong.Distance(target) <= riumx.Range) || (tuong.Distance(target) <= tiamat.Range))
                    {
                        riumx.Cast(target);
                        tiamat.Cast(target);
                    }
                }
            }
            if ( target1 != null)
            {
                if (Config.Item("UseItems").GetValue<bool>()) 
                {
                    sunghex.Cast(target1);
                    buadl.Cast(target1);
                }
            }
        }
        private static void Smart() {
            foreach (Obj_AI_Hero hero in ObjectManager.Get<Obj_AI_Hero>())
            {
                if (hero != null)
                {
                    if ((hero.IsEnemy) && (hero.IsValidTarget(GetSummoner("SummonerDot").SData.CastRange[0])))
                    {
                        if (hero.Health < DamageLib.getDmg(hero, DamageLib.SpellType.IGNITE))
                        {
                            tuong.SummonerSpellbook.CastSpell(GetSummoner("SummonerDot").Slot, hero);
                        }
                    }
                }
            }
            
        }
        private static void Exhaust() {
            foreach (Obj_AI_Hero hero in ObjectManager.Get<Obj_AI_Hero>())
            {
                if (hero != null)
                {
                    if ((hero.IsEnemy) && (hero.IsValidTarget(GetSummoner("SummonerExhaust").SData.CastRange[0])))
                    {
                        if (hero.Health < hero.MaxHealth / 3)
                        {
                            tuong.SummonerSpellbook.CastSpell(GetSummoner("SummonerExhaust").Slot, hero);
                        }
                    }
                }

            }
            
        }
        
        private static SpellDataInst GetSummoner(string p)
        {
            var spells = tuong.SummonerSpellbook.Spells;
            return spells.FirstOrDefault(spell => spell.Name == p);
        }
        private static float GetPlayerHealthPercent()
        {
            return ObjectManager.Player.Health * 100 / ObjectManager.Player.MaxHealth;
        }
        
        private static InventorySlot GetHealthPotionSlot()
        {
            return ObjectManager.Player.InventoryItems.First(item => (item.Id == (ItemId)2003 && item.Stacks >= 1 )|| (item.Id == (ItemId)2009 && item.Stacks >= 1) || (item.Id == (ItemId)2010 && item.Stacks >= 1) || (item.Id == (ItemId)2041) && item.Charges >=1 );
        }

        private static float GetPlayerManaPercent()
        {
            return ObjectManager.Player.Mana * 100 / ObjectManager.Player.MaxMana;
        }

        private static InventorySlot GetManaPotionSlot()
        {
            return ObjectManager.Player.InventoryItems.First(item => (item.Id == (ItemId)2004 && item.Stacks >= 1) || (item.Id == (ItemId)2041 && item.Charges >=1 ));
        }

    }
}
