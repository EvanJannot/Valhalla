using RLNET;
using RogueSharp;
using Valhalla.Equipment;
using Valhalla.Interfaces;
using System.Text;

namespace Valhalla.Core
{
    //Classe de gestion de l'equipement qui est aussi un trésor qui est dessiné au sol par un casque 
    public class Equipment : IEquipment, ITreasure, IDrawable
    {
        StringBuilder espace = new StringBuilder();
        public Equipment()
        {
            Symbol = (char)16; //Représentation de l'equipement par un casque doré
            Color = RLColor.Yellow;

        }

        //Getter et setter des caractéristiques de l'interface IEquipement qui caractérisent les équipements
        public int Attack { get; set; }
        public int AttackChance { get; set; }
        public int Awareness { get; set; }
        public int Defense { get; set; }
        public int DefenseChance { get; set; }
        public int Gold { get; set; }
        public int Health { get; set; }
        public int MaxHealth { get; set; }
        public string Name { get; set; }
        public int Speed { get; set; }


        //Permet de vérifier si un équipement est égale à un autre 
        protected bool Equals(Equipment other)
        {
            return Attack == other.Attack && AttackChance == other.AttackChance && Awareness == other.Awareness && Defense == other.Defense && DefenseChance == other.DefenseChance && Gold == other.Gold && Health == other.Health && MaxHealth == other.MaxHealth && string.Equals(Name, other.Name) && Speed == other.Speed;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != this.GetType())
            {
                return false;
            }
            return Equals((Equipment)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Attack;
                hashCode = (hashCode * 397) ^ AttackChance;
                hashCode = (hashCode * 397) ^ Awareness;
                hashCode = (hashCode * 397) ^ Defense;
                hashCode = (hashCode * 397) ^ DefenseChance;
                hashCode = (hashCode * 397) ^ Gold;
                hashCode = (hashCode * 397) ^ Health;
                hashCode = (hashCode * 397) ^ MaxHealth;
                hashCode = (hashCode * 397) ^ (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Speed;
                return hashCode;
            }
        }

        //Opérateur qui permet de comparer deux équipements (utilisé pour vérifier qu'il n'y a pas d'équipement)
        public static bool operator ==(Equipment left, Equipment right)
        {
            return Equals(left, right);
        }

        //Opérateur qui permet de comparer deux équipements (utilisé pour vérifier qu'il y a bien un équipement)
        public static bool operator !=(Equipment left, Equipment right)
        {
            return !Equals(left, right);
        }

        //Permet à un acteur de ramasser de l'équipement
        public bool PickUp(IActor actor)
        {
            if (this is HeadEquipment) //Si c'est un casque 
            {
                actor.Head = this as HeadEquipment; //On définit le casque de l'acteur 
                Game.MessageLog.Add($"{actor.Name} a ramasse un casque en {Name}");  //On affiche un message de confirmation 
                Game.MessageLog.Add(espace.ToString());
                return true;
            }

            if (this is BodyEquipment) //Si c'est un plastron
            {
                actor.Body = this as BodyEquipment; //On définit le plastron de l'acteur 
                Game.MessageLog.Add($"{actor.Name} a ramasse un plastron en {Name}"); //On affiche un message de confirmation
                Game.MessageLog.Add(espace.ToString());
                return true;
            }

            if (this is HandEquipment) //Si c'est une arme 
            {
                actor.Hand = this as HandEquipment; //On définit l'arme de l'acteur 
                Game.MessageLog.Add($"{actor.Name} a ramasse une {Name}"); //On affiche un message de confirmation
                Game.MessageLog.Add(espace.ToString());
                return true;
            }

            if (this is FeetEquipment) //Si ce sont des jambières 
            {
                actor.Feet = this as FeetEquipment; //On définit les nouvelles jambières de l'acteur 
                Game.MessageLog.Add($"{actor.Name} a ramasse des bottes en {Name}"); //On affiche un message de confirmation 
                Game.MessageLog.Add(espace.ToString());
                return true;
            }

            return false;
        }

        public RLColor Color { get; set; }
        public char Symbol { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        //Fonction pour dessiner l'equipement au sol
        public void Draw(RLConsole console, IMap map, int lvl)
        {
            if (this is BodyEquipment)
            {
                Symbol = (char)162;
            }
            if (this is FeetEquipment)
            {
                Symbol = (char)163;
            }
            if (this is HandEquipment)
            {
                Symbol = (char)12;
            }
            if (!map.IsExplored(X, Y)) //Si la case n'est pas explorée on affiche rien 
            {
                return;
            }

            if (map.IsInFov(X, Y)) //Si elle est dans le champ de vision on l'affiche de manière claire et sinon on le floute  
            {
                if (Name == "Cuir") //On différencie les types d'équipements par couleur (jaune = cuir, bleu = maille, rouge = plaque et gris = arme)
                {
                    Color = RLColor.Yellow;
                }
                else if (Name == "Maille")
                {
                    Color = RLColor.Blue;
                }
                else if (Name == "Plaque")
                {
                    Color = RLColor.Red;
                }
                else
                {
                    Color = RLColor.Gray;
                }
                console.Set(X, Y, Color, Colors.FloorBackgroundFov, Symbol);
            }
            else
            {
                console.Set(X, Y, RLColor.Blend(Color, RLColor.Gray, 0.5f), Colors.FloorBackground, Symbol);
            }
        }
    }
}