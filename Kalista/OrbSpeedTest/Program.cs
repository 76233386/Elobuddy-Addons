using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
namespace OrbSpeedTest
{
    class Program
    {
        static int count;
        
        static float lastaa, compare;
        
        static Menu OrbSpeedTestmenu;
        
        public static void Main(string[]args)
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }
        
        static void Loading_OnLoadingComplete(EventArgs args)
        {
            OrbSpeedTestmenu=MainMenu.AddMenu("Orb Speed Test","orbspeedtest");
            OrbSpeedTestmenu.Add("aac", new Slider("Time of x attacks", 5, 1, 10));
            Obj_AI_Base.OnBasicAttack += Obj_AI_Base_OnBasicAttack;
        }

        static void Obj_AI_Base_OnBasicAttack(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                lastaa = Game.Time * 1000;
                count = count + 1;
                if ( count == 1 )
                {
                    compare = lastaa;
                }
                if ( count == OrbSpeedTestmenu["aac"].Cast<Slider>().CurrentValue )
                {
                    Chat.Print ( OrbSpeedTestmenu["aac"].Cast<Slider>().CurrentValue + " attacks in: " + (lastaa - compare) + " milliseconds. Next Test F5 " );
                }
            }
        }
    }
}