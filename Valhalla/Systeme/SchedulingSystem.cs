using System.Collections.Generic;
using System.Linq;
using Valhalla.Interfaces;

namespace Valhalla.Systems
{
    //Classe permettant de gérer le système de temps
    public class SchedulingSystem
    {
        private int _time;
        private readonly SortedDictionary<int, List<IScheduleable>> _scheduleables;

        public SchedulingSystem()
        {
            _time = 0;
            _scheduleables = new SortedDictionary<int, List<IScheduleable>>();
        }

        //Fonction qui ajoute un acteur au système de gestion du temps
        public void Add(IScheduleable actor)
        {
            int key = _time + actor.Time; //On créé une clé à partir du temps et de la vitesse de l'acteur 
            if (!_scheduleables.ContainsKey(key)) //Si le dictionnaire ne contient pas déjà cette clé 
            {
                _scheduleables.Add(key, new List<IScheduleable>()); //On ajoute la clé et on créé une liste d'acteurs associés 
            }
            _scheduleables[key].Add(actor); //Sinon on ajoute l'acteur à la liste associé à la clé 
        }

        //Fonction pour supprimer un acteur du système de gestion du temps
        public void Remove(IScheduleable scheduleable)
        {
            KeyValuePair<int, List<IScheduleable>> scheduleableListFound = new KeyValuePair<int, List<IScheduleable>>(-1, null); //On défini la clé associée à scheduleableListFound comme étant null

            foreach (var scheduleablesList in _scheduleables) //Pour chaque élément du dictionnaire 
            {
                if (scheduleablesList.Value.Contains(scheduleable))  //Si on trouve l'élément 
                {
                    scheduleableListFound = scheduleablesList; //scheduleableListFound devient l'élément que l'on cherche et n'est donc plus null
                    break;
                }
            }
            if (scheduleableListFound.Value != null) //Si l'élément a été trouvé donc scheduleableListFound n'est plus null
            {
                scheduleableListFound.Value.Remove(scheduleable); //On l'enlève 
                if (scheduleableListFound.Value.Count <= 0) //Si il est encore présent on répète l'opération jusqu'à ce qu'il soit entièrement supprimé 
                {
                    _scheduleables.Remove(scheduleableListFound.Key);
                }
            }
        }

        //Fonction pour récupérer l'acteur qui doit jouer 
        public IScheduleable Get()
        {
            var firstScheduleableGroup = _scheduleables.First(); //On récupère le premier dans le dictionnaire 
            var firstScheduleable = firstScheduleableGroup.Value.First(); //On récupère sa valeur 
            Remove(firstScheduleable); //On le supprime du dicctionnaire 
            _time = firstScheduleableGroup.Key; //Le temps avance jusqu'à son tour (exemple : si on est au temps 0 et que le premier qui va jouer joue au bout de 2 temps on passe au temps 2)
            return firstScheduleable; //On retourne l'élément qui a joué 
        }

        //Fonction pour récupérer le temps
        public int GetTime()
        {
            return _time;
        }

        //Fonction pour remettre à 0 le système de temps
        public void Clear()
        {
            _time = 0;
            _scheduleables.Clear();
        }
    }
}
