using RLNET;
using RogueSharp;
using Valhalla.Equipment;
using Valhalla.Interfaces;

namespace Valhalla.Core
{
    //Classe Acteur qui est la classe mère de la classe du joueur et des monstres 
    public class Actor : IActor, IDrawable, IScheduleable
    {
        public Actor() //Un acteur est caractérisé par son equipement, de base il n'en a pas 
        {
            Head = HeadEquipment.None();
            Body = BodyEquipment.None();
            Hand = HandEquipment.None();
            Feet = FeetEquipment.None();
        }

        //Ci dessous, l'ensemble des getter et setter
        public HeadEquipment Head { get; set; }
        public BodyEquipment Body { get; set; }
        public HandEquipment Hand { get; set; }
        public FeetEquipment Feet { get; set; }

        private int _attack; //Caractéristiques pouvant définir un acteur
        private int _attackChance;
        private int _awareness;
        private int _defense;
        private int _defenseChance;
        private int _gold;
        private int _health;
        private int _maxHealth;
        private string _name;
        private int _speed;
        private int _xp;
        private int _lvl;

        public int Attack
        {
            get
            {
                return _attack + Head.Attack + Body.Attack + Hand.Attack + Feet.Attack;
            }
            set
            {
                _attack = value;
            }
        }

        public int AttackChance
        {
            get
            {
                return _attackChance + Head.AttackChance + Body.AttackChance + Hand.AttackChance + Feet.AttackChance;
            }
            set
            {
                _attackChance = value;
            }
        }

        public int Awareness
        {
            get
            {
                return _awareness + Head.Awareness + Body.Awareness + Hand.Awareness + Feet.Awareness;
            }
            set
            {
                _awareness = value;
            }
        }

        public int Defense
        {
            get
            {
                return _defense + Head.Defense + Body.Defense + Hand.Defense + Feet.Defense;
            }
            set
            {
                _defense = value;
            }
        }

        public int DefenseChance
        {
            get
            {
                return _defenseChance + Head.DefenseChance + Body.DefenseChance + Hand.DefenseChance + Feet.DefenseChance;
            }
            set
            {
                _defenseChance = value;
            }
        }

        public int Gold
        {
            get
            {
                return _gold + Head.Gold + Body.Gold + Hand.Gold + Feet.Gold;
            }
            set
            {
                _gold = value;
            }
        }

        public int Health
        {
            get
            {
                return _health;
            }
            set
            {
                _health = value;
            }
        }

        public int MaxHealth
        {
            get
            {
                return _maxHealth + Head.MaxHealth + Body.MaxHealth + Hand.MaxHealth + Feet.MaxHealth;
            }
            set
            {
                _maxHealth = value;
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        public int Speed
        {
            get
            {
                return _speed + Head.Speed + Body.Speed + Hand.Speed + Feet.Speed;
            }
            set
            {
                _speed = value;
            }
        }

        public int Xp
        {
            get
            {
                return _xp;
            }
            set
            {
                _xp = value;
            }
        }
        public int Lvl
        {
            get
            {
                return _lvl;
            }
            set
            {
                _lvl = value;
            }
        }

        public RLColor Color { get; set; }
        public char Symbol { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        //Fonction afin de dessiner l'acteur 
        public void Draw(RLConsole mapConsole, IMap map, int lvl)
        {
            if (!map.GetCell(X, Y).IsExplored) //Si la case n'est pas explorée, l'acteur n'est pas dessiné (permet de ne pas voir les monstres des zones non explorées)
            {
                return;
            }

            // Si la case est visible, on dessine l'acteur avec sa couleur et son symbole
            if (map.IsInFov(X, Y))
            {
                mapConsole.Set(X, Y, Color, Colors.FloorBackgroundFov, Symbol);
            }
            else //Sinon on dessine un sol normal
            {
                if (lvl == 1)
                {
                    mapConsole.Set(X, Y, Colors.Floor, Colors.FloorBackground, (char)8);
                }
                else if (lvl == 2)
                {
                    mapConsole.Set(X, Y, Colors.Floor, Colors.FloorBackground, (char)95);
                }
                else if (lvl == 3)
                {
                    mapConsole.Set(X, Y, Colors.Floor, Colors.FloorBackground, (char)64);
                }
                else
                {
                    mapConsole.Set(X, Y, Colors.Floor, Colors.FloorBackground, (char)154);
                }
            }
        }

        //Permet de récupérer la vitesse d'un acteur pour le système de gestion du temps 
        public int Time
        {
            get
            {
                return Speed;
            }
        }
    }
}