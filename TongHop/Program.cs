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

            var targetSelectorMenu = new Menu("Target Selector", "Target Selector");
            SimpleTs.AddToMenu(targetSelectorMenu);

            //Combo
            Config.AddSubMenu(new Menu("KillSteal", "Ks"));
            Config.SubMenu("Ks").AddItem(new MenuItem("UseIgnite", "Use Ignite")).SetValue(true);
            Config.AddSubMenu(new Menu("UseItems", "UseItems"));
            Config.SubMenu("UseItems").AddItem(new MenuItem("UseItems", "Use Items")).SetValue(true);
            Config.SubMenu("UseItems").AddItem(new MenuItem("ActiveCombo", "UseItems").SetValue(new KeyBind(32, KeyBindType.Press)));
     
            Config.AddToMainMenu();

            Game.OnGameUpdate += OnGameUpdate;

            Game.PrintChat("<font color='#1d87f2'>TongHop Loaded!</font>");
        }

        private static void OnGameUpdate(EventArgs args) 
        {
           tuong = ObjectManager.Player;
            if (Config.Item("ActiveCombo").GetValue<KeyBind>().Active) 
            {
                Combo();
            }
            if (Config.Item("UseIgnite").GetValue<bool>()) {
                KillSteal();
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
        private static void KillSteal() {
            var target = SimpleTs.GetTarget(650, SimpleTs.DamageType.Physical);
            var igniteDmg = DamageLib.getDmg(target, DamageLib.SpellType.IGNITE);
            if (target != null && tuong.SummonerSpellbook.CanUseSpell(IgniteSlot) == SpellState.Ready)
            {
                if (igniteDmg > target.Health)
                {
                    tuong.SummonerSpellbook.CastSpell(IgniteSlot, target);
                }
            }
        }

    }
}