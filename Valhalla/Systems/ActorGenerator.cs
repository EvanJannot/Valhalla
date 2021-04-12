using RogueSharp;
using RogueSharpRLNetSamples.Core;
using RogueSharpRLNetSamples.Monsters;

namespace RogueSharpRLNetSamples.Systems
{
   public static class ActorGenerator
   {
      private static Player _player = null;

      public static Monster CreateMonster( int level, Point location, int monde )
      {
            Pool<Monster> monsterPool = new Pool<Monster>();
            if (monde == 1)
            {
                if (level == 6)
                {
                    monsterPool.Add(Ours.Create(monde), 100);
                }
                else
                {
                    monsterPool.Add(Araignee.Create(monde), 25);
                    monsterPool.Add(Slime.Create(monde), 25);
                    monsterPool.Add(Abeille.Create(monde), 50);
                }
            }
            else if (monde == 2)
            {
                if (level == 6)
                {
                    monsterPool.Add(Golem.Create(monde), 100);
                }
                else
                {
                    monsterPool.Add(Squelette.Create(monde), 25);
                    monsterPool.Add(Fantome.Create(monde), 25);
                    monsterPool.Add(Chauve_souris.Create(monde), 50);
                }
            }
            else if (monde == 3)
            {
                if (level == 6)
                {
                    monsterPool.Add(Roi.Create(monde), 100);
                }
                else
                {
                    monsterPool.Add(Demon.Create(monde), 25);
                    monsterPool.Add(Garde.Create(monde), 25);
                    monsterPool.Add(Rat.Create(monde), 50);
                }
            }
            else  if (monde != 1 & monde != 2 & monde !=3)
            {
                if (level == 6)
                {
                    monsterPool.Add(Roi.Create(monde), 33);
                    monsterPool.Add(Ours.Create(monde), 33);
                    monsterPool.Add(Golem.Create(monde), 34);
                }
                else
                {
                    monsterPool.Add(Demon.Create(monde), 11);
                    monsterPool.Add(Garde.Create(monde), 12);
                    monsterPool.Add(Rat.Create(monde), 11);
                    monsterPool.Add(Araignee.Create(monde), 11);
                    monsterPool.Add(Slime.Create(monde), 11);
                    monsterPool.Add(Abeille.Create(monde), 11);
                    monsterPool.Add(Squelette.Create(monde), 11);
                    monsterPool.Add(Fantome.Create(monde), 11);
                    monsterPool.Add(Chauve_souris.Create(monde), 11);
                }
            }




         Monster monster = monsterPool.Get();
         monster.X = location.X;
         monster.Y = location.Y;

         return monster;
      }

        public static void ResetPlayer()
        {
            _player = null;
        }


      public static Player CreatePlayer(int atkX, int defX, int awaX, int hpX, string name, char symbol)
      {
         if ( _player == null )
         {
            _player = new Player {
               Attack = 2 +atkX,
               AttackChance = 60,
               Awareness = 15 + awaX,
               Color = Colors.Player,
               Defense = 2 + defX,
               DefenseChance = 40,
               Gold = 0,
               Health = 100 + hpX,
               MaxHealth = 100 + hpX,
               Xp = 0,
               Lvl = 1,
               Name = name,
               Speed = 10,
               Symbol = symbol
            };
         }

         return _player;
      }
   }
}