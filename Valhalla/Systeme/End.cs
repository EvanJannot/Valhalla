using System.Collections.Generic;
using System.Linq;
using RLNET;

namespace Valhalla.Systems
{
    //Classe de l'écran de fin
    public class End
    {
        private readonly Queue<string> _lines;
        public End() //On créé une variable qui contiendra le texte 
        {
            _lines = new Queue<string>();
        }

        //Fonction pour ajouter du texte 
        public void Add(string message)
        {
            _lines.Enqueue(message);
            if (_lines.Count > 9)
            {
                _lines.Dequeue();
            }
        }

        //Fonction pour dessiner le texte 
        public void Draw(RLConsole console)
        {
            console.Clear(); //On vide la console
            string[] lines = _lines.ToArray(); //On créé un tableau de texte, chaque case correspond à une ligne
            for (int i = 0; i < lines.Count(); i++) //On affiche les lignes les unes après les autres 
            {
                console.Print(1, i + 1, lines[i], RLColor.White);
            }
        }
    }
}
