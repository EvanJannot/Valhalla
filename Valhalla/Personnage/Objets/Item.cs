using RLNET;
using RogueSharp;
using Valhalla.Interfaces;
using System.Text;

namespace Valhalla.Core
{
    //Classe définissant les objets qui sont des trésors et dessinables
    public class Item : IItem, ITreasure, IDrawable
    {
        StringBuilder espace = new StringBuilder();
        public Item() 
        {
            Symbol = (char)15; //On définit le symbole qui est une fiole marron
            Color = RLColor.Brown;
        }

        public string Name { get; protected set; }
        public int RemainingUses { get; protected set; }

        public bool Use() //Fonction permettant de vérifier si un objet est utilisé en appuyant sur une touche dans la classe des controles 
        {
            return UseItem(); //On retourne si l'objet est utilisé ou non
        }

        //Fonction d'utilisation de l'objet 
        protected virtual bool UseItem() //De base la valeur est fale mais cette fonction est re écrite dans chaque classe d'objet 
        {
            return false;
        }

        //Fonction permettant à un acteur de récupérer un objet
        public bool PickUp(IActor actor)
        {
            Player player = actor as Player;

            if (player != null) //Si c'est le joueur 
            {
                if (player.AddItem(this)) //Si l'objet peut être récupéré 
                {
                    Game.MessageLog.Add($"{actor.Name} a ramasse {Name}"); //On affiche un message de confirmation
                    Game.MessageLog.Add(espace.ToString());
                    return true; //On retourne true pour confirmer que le joueur l'a pris
                }
            }

            return false; //Sinon on retourne false 
        }

        public RLColor Color { get; set; }

        public char Symbol { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        //Fonction pour dessiner l'objet
        public void Draw(RLConsole console, IMap map, int lvl)
        {
            if (!map.IsExplored(X, Y)) //Si la case n'est pas explorée on retourne false
            {
                return;
            }

            if (map.IsInFov(X, Y)) //Si elle est dans le champ de vision on affiche l'objet sinon il est flouté 
            {
                console.Set(X, Y, Color, Colors.FloorBackgroundFov, Symbol);
            }
            else
            {
                console.Set(X, Y, RLColor.Blend(Color, RLColor.Gray, 0.5f), Colors.FloorBackground, Symbol);
            }
        }
    }
}
