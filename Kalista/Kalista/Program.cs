using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
namespace ParaKalista
{
    class Program
    {
        public static void Main(string[]args){Loading.OnLoadingComplete+=Loading_OnLoadingComplete;}
        static Spell.Skillshot Q;
        static Spell.Active E;
        static float lastaa;
        static Menu Kalistamenu;
        static readonly Item botrk = new Item((int)ItemId.Blade_of_the_Ruined_King, 550f);
        static readonly Item bc = new Item((int)ItemId.Bilgewater_Cutlass, 550f);
        static void Loading_OnLoadingComplete(EventArgs args)
        {
            Q=new Spell.Skillshot(SpellSlot.Q,1200,SkillShotType.Linear,250,1700,40);
            Q.AllowedCollisionCount=0;
            Q.MinimumHitChance=HitChance.High;
            E=new Spell.Active(SpellSlot.E);
            Kalistamenu=MainMenu.AddMenu("Kalista","kalista");
            Kalistamenu.Add("combo",new KeyBind("Combo",false,KeyBind.BindTypes.HoldActive,' '));
            Kalistamenu.AddSeparator();
            Kalistamenu.Add("useq",new CheckBox("Use Q In Combo"));
            Kalistamenu.AddSeparator();
            Kalistamenu.Add("combo2",new CheckBox("Orb Mode 2 - Fly hack | exploit if > 2.0 attack speed [ care ez ban ]", false));
            Kalistamenu.AddSeparator();
            Kalistamenu.Add("aad",new CheckBox("Attack Range"));
            Obj_AI_Base.OnBasicAttack += Obj_AI_Base_OnBasicAttack;
            Game.OnTick += On_Tick;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        static void Obj_AI_Base_OnBasicAttack(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                lastaa=Game.Time*1000;
            }
        }

        static void On_Tick(EventArgs args)
        {
            Killsteal();
            MinionEKill();
            if(Kalistamenu["combo"].Cast<KeyBind>().CurrentValue)
            {
                Items();
                UseQ();
                if(Kalistamenu["combo2"].Cast<CheckBox>().CurrentValue)
                {
                    var target = TargetSelector.GetTarget(ObjectManager.Player.AttackRange+ObjectManager.Player.BoundingRadius+65,DamageType.Physical);
                    if (target.IsValidTarget())
                    {
                        if (Game.Time*1000 >= lastaa + 1)
                        {
                            Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                        }
                        if (Game.Time*1000 > lastaa + ObjectManager.Player.AttackDelay*1000-180)
                        {
                            Player.IssueOrder(GameObjectOrder.AttackUnit, target);
                        }
                    }
                    else
                    {
                        Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                    }
                }
                else
                {
                    var target = TargetSelector.GetTarget(ObjectManager.Player.AttackRange+ObjectManager.Player.BoundingRadius+65,DamageType.Physical);
                    if (target.IsValidTarget())
                    {
                        if (Game.Time*1000 > lastaa + ObjectManager.Player.AttackDelay*1000-180)
                        {
                            Player.IssueOrder(GameObjectOrder.AttackUnit, target);
                        }
                        else if (Game.Time*1000 >= lastaa + 1)
                        {
                            Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                        }
                    }
                    else
                    {
                        Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                    }
                }
            }
        }
        static void Killsteal()
        {
            foreach(var hero in HeroManager.Enemies.Where(x => !x.IsDead && x.GetBuffCount("kalistaexpungemarker")>0))
            {
                if (hero.GetBuffCount("kalistaexpungemarker")==1)
                {
                    if (hero.Health>10 && hero.Health < ObjectManager.Player.CalculateDamageOnUnit(hero, DamageType.Physical,new float[] { 20, 30, 40, 50, 60 }[E.Level-1] + ObjectManager.Player.TotalAttackDamage*60/100))
                    {
                        E.Cast();
                    }
                }
                if (hero.GetBuffCount("kalistaexpungemarker")>1)
                {
                    if (hero.Health>10 && hero.Health < ObjectManager.Player.CalculateDamageOnUnit(hero, DamageType.Physical,new float[] { 20, 30, 40, 50, 60 }[E.Level-1]+ObjectManager.Player.TotalAttackDamage*60/100+(hero.GetBuffCount("kalistaexpungemarker")-1)*(new float[] { 10, 14, 19, 25, 32 }[E.Level-1]+(ObjectManager.Player.TotalAttackDamage*new float[] { 200, 225, 250, 275, 300 }[E.Level-1]/1000))))
                    {
                        E.Cast();
                    }
                }
            }
        }
        static void UseQ()
        {
            var qtarget = TargetSelector.GetTarget(Q.Range,DamageType.Physical);
            if(qtarget.IsValidTarget() && Q.IsReady() && Kalistamenu["useq"].Cast<CheckBox>().CurrentValue)
            {
                Q.Cast(qtarget);
            }
        }
        static void MinionEKill()
        {
            foreach(var minion in ObjectManager.Get<Obj_AI_Minion>().Where(x=>!x.IsDead && x.GetBuffCount("kalistaexpungemarker")>3))
            {
                if (minion.Health>10 && minion.Health+25 < ObjectManager.Player.CalculateDamageOnUnit(minion, DamageType.Physical,new float[] { 20, 30, 40, 50, 60 }[E.Level-1]+ObjectManager.Player.TotalAttackDamage*60/100+(minion.GetBuffCount("kalistaexpungemarker")-1)*(new float[] { 10, 14, 19, 25, 32 }[E.Level-1]+(ObjectManager.Player.TotalAttackDamage*new float[] { 200, 225, 250, 275, 300 }[E.Level-1]/1000))))
                {
                    E.Cast();
                }
            }
        }
        static void Drawing_OnDraw(EventArgs args)
        {
            if (Kalistamenu["aad"].Cast<CheckBox>().CurrentValue)
            {
                var range = ObjectManager.Player.AttackRange+ObjectManager.Player.BoundingRadius+65;
                Drawing.DrawCircle(ObjectManager.Player.Position,range,System.Drawing.Color.DarkRed);
            }
            foreach(var hero in HeroManager.Enemies.Where(x => !x.IsDead && x.GetBuffCount("kalistaexpungemarker")>1))
            {
                if (hero.Health>10)
                {
                    if (hero.GetBuffCount("kalistaexpungemarker")==1)
                    {
                        Drawing.DrawText(Drawing.WorldToScreen(hero.Position)[0],Drawing.WorldToScreen(hero.Position)[1],System.Drawing.Color.AliceBlue,(ushort)(100*ObjectManager.Player.CalculateDamageOnUnit(hero, DamageType.Physical,new float[] { 20, 30, 40, 50, 60 }[E.Level-1] + ObjectManager.Player.TotalAttackDamage*60/100)/hero.Health)+" %",3);
                    }
                    if (hero.GetBuffCount("kalistaexpungemarker")>1)
                    {
                        Drawing.DrawText(Drawing.WorldToScreen(hero.Position)[0],Drawing.WorldToScreen(hero.Position)[1],System.Drawing.Color.AliceBlue,(ushort)(100*ObjectManager.Player.CalculateDamageOnUnit(hero, DamageType.Physical,new float[] { 20, 30, 40, 50, 60 }[E.Level-1]+ObjectManager.Player.TotalAttackDamage*60/100+(hero.GetBuffCount("kalistaexpungemarker")-1)*(new float[] { 10, 14, 19, 25, 32 }[E.Level-1]+(ObjectManager.Player.TotalAttackDamage*new float[] { 200, 225, 250, 275, 300 }[E.Level-1]/1000)))/hero.Health)+" %",3);
                    }
                }
            }
            foreach (var minion in ObjectManager.Get<Obj_AI_Minion>().Where(x=>!x.IsDead && x.GetBuffCount("kalistaexpungemarker")>3))
            {
                if (minion.Health>10)
                {
                    Drawing.DrawText(Drawing.WorldToScreen(minion.Position)[0],Drawing.WorldToScreen(minion.Position)[1],System.Drawing.Color.AliceBlue,(ushort)(100*ObjectManager.Player.CalculateDamageOnUnit(minion, DamageType.Physical,new float[] { 20, 30, 40, 50, 60 }[E.Level-1]+ObjectManager.Player.TotalAttackDamage*60/100+(minion.GetBuffCount("kalistaexpungemarker")-1)*(new float[] { 10, 14, 19, 25, 32 }[E.Level-1]+(ObjectManager.Player.TotalAttackDamage*new float[] { 200, 225, 250, 275, 300 }[E.Level-1]/1000)))/minion.Health)+" %",3);
                }
            }
        }
        static void Items()
        {
            var botrktarget = TargetSelector.GetTarget(ObjectManager.Player.AttackRange+ObjectManager.Player.BoundingRadius+65,DamageType.Physical);
            if (botrktarget.IsValidTarget() && botrktarget.Distance(ObjectManager.Player)<550)
            {
                if (botrk.Slots.Any())
                {
                    if (botrk.IsReady())
                    {
                        botrk.Cast(botrktarget);
                    }
                }
                if (bc.Slots.Any())
                {
                    if (bc.IsReady())
                    {
                        bc.Cast(botrktarget);
                    }
                }
            }
        }
    }
}