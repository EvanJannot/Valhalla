using System.Collections.Generic;
using System.Linq;
using RLNET;
using RogueSharp;
using Valhalla.Core;
using Valhalla.Interfaces;

namespace Valhalla.Systems
{
    //Classe permettant de gérer le système de visée/Sélection de zone
    public class TargetingSystem
    {
        private enum SelectionType //On défini les différentes possibilités (aucune, une cible simple, une zone, une ligne)
        {
            None = 0,
            Target = 1,
            Area = 2,
            Line = 3
        }

        public bool IsPlayerTargeting { get; private set; }

        private Point _cursorPosition;
        private List<Point> _selectableTargets = new List<Point>();
        private int _currentTargetIndex;
        private ITargetable _targetable;
        private int _area;
        private SelectionType _selectionType;

        //Fonction qui permet de cibler un monstre 
        public bool SelectMonster(ITargetable targetable)
        {
            Initialize(); //On réinitialise les paramètres 
            _selectionType = SelectionType.Target; //On précise que c'est une cible 
            DungeonMap map = Game.DungeonMap;
            _selectableTargets = map.GetMonsterLocationsInFieldOfView().ToList(); //On ajoute à la liste des cibles possibles les monstres dans le champ de vision
            _targetable = targetable; //On définit la cible à partir du paramètre
            _cursorPosition = _selectableTargets.FirstOrDefault(); //On place le curseur sur la première cible de la liste
            if (_cursorPosition == null) //Si le curseur est sur aucun ennnemi c'est qu'il n'y en a pas dans la liste et que c'est impossible d'en cibler
            {
                StopTargeting(); //On arrête de cibler et on retourne false
                return false;
            }

            IsPlayerTargeting = true; //Sinon le joueur cible et on retourne true 
            return true;
        }

        //Fonction permettant de sélectionner une zone 
        public bool SelectArea(ITargetable targetable, int area = 0)
        {
            Initialize(); //On réinitialise les paramètres 
            _selectionType = SelectionType.Area; //On précise que c'est une zone 
            Player player = Game.Player;
            _cursorPosition = new Point { X = player.X, Y = player.Y }; //Le curseur commence sur le joueur 
            _targetable = targetable;
            _area = area; //La zone est définie 

            IsPlayerTargeting = true; //Le joueur cible forcément une zone
            return true;
        }

        //Fonction permettant de sélectionner une ligne 
        public bool SelectLine(ITargetable targetable)
        {
            Initialize();//On réinitialise les paramètres 
            _selectionType = SelectionType.Line; //On précise que c'est une ligne 
            Player player = Game.Player;
            _cursorPosition = new Point { X = player.X, Y = player.Y }; //Le curseur commence sur le joueur 
            _targetable = targetable;

            IsPlayerTargeting = true; //Le joueur cible forcément 
            return true;
        }

        //Fonction pour arrêter la visée 
        private void StopTargeting()
        {
            IsPlayerTargeting = false; //On passe à false le paramètre indiquant que le joueur vise
            Initialize(); //On réinitialise les paramètres 
        }

        //Fonction appeler à chaque début de visée pour réinitialiser les paramètres et partir sur de bonnes bases 
        private void Initialize()
        {
            _cursorPosition = null; //On définit comme null la position du curseur de sélection
            _selectableTargets = new List<Point>(); //On initialise la liste des points 
            _currentTargetIndex = 0;
            _area = 0; //On initialise la zone 
            _targetable = null; //On initialise la cible 
            _selectionType = SelectionType.None;
        }

        //Fonction gérant les touches sur lesquelles le joueur appuie 
        public bool HandleKey(RLKey key)
        {
            if (_selectionType == SelectionType.Target) //Si c'est une cible 
            {
                HandleSelectableTargeting(key);
            }
            else if (_selectionType == SelectionType.Area) //Si c'est une zone 
            {
                HandleLocationTargeting(key);
            }
            else if (_selectionType == SelectionType.Line) //Si c'est une ligne 
            {
                HandleLocationTargeting(key);
            }

            if (key == RLKey.Enter) //Si il valide 
            {
                _targetable.SelectTarget(_cursorPosition); //On valide la zone la zone 
                StopTargeting(); //On arrete de viser
                return true;
            }

            return false;
        }

        //Si on a une cible de sélectionnée
        private void HandleSelectableTargeting(RLKey key)
        {
            if (key == RLKey.Right || key == RLKey.Down) //Si on se déplace à droite ou en bas
            {
                _currentTargetIndex++; //On passe l'indice de la cible à celui d'après 
                if (_currentTargetIndex >= _selectableTargets.Count) //Si on dépasse le nombre total de cibles possible
                {
                    _currentTargetIndex = 0; //On resélectionne la première
                }
                _cursorPosition = _selectableTargets[_currentTargetIndex]; //Le curseur passe sur la cible d'après 
            }
            else if (key == RLKey.Left || key == RLKey.Up) //Si on se déplace à gauche ou en haut 
            {
                _currentTargetIndex--; //On passe l'indice de la cible à celui d'avant
                if (_currentTargetIndex < 0) //Si on veut sélectionner la cible avant la première 
                {
                    _currentTargetIndex = _selectableTargets.Count - 1; //On prend la dernière 
                }
                _cursorPosition = _selectableTargets[_currentTargetIndex]; //Le curseur passe sur la cible d'avant 
            }
        }

        //Si on sélectionne une zone ou une ligne 
        private void HandleLocationTargeting(RLKey key)
        {
            int x = _cursorPosition.X; //On stocke dans des variables la position du curseur 
            int y = _cursorPosition.Y;
            DungeonMap map = Game.DungeonMap;

            //Pour les commandes gauche et droite on déplace juste la variable de position du curseur à gauche ou à droite 
            if (key == RLKey.Right)
            {
                x++;
            }
            else if (key == RLKey.Left)
            {
                x--;
            }
            //Pour les commandes haut et bas on déplace juste la variable de position du curseur en haut ou en bas
            else if (key == RLKey.Up)
            {
                y--;
            }
            else if (key == RLKey.Down)
            {
                y++;
            }
            //Si la sélection est bien dans le champ de vision on applique aux coordonnées du curseur la valeur des variables de stockage modifiées 
            if (map.IsInFov(x, y))
            {
                _cursorPosition.X = x;
                _cursorPosition.Y = y;
            }
        }

        //Fonction pour dessiner le curseur 
        public void Draw(RLConsole mapConsole)
        {
            if (IsPlayerTargeting) //Si le joueur vise 
            {
                DungeonMap map = Game.DungeonMap;
                Player player = Game.Player;
                if (_selectionType == SelectionType.Area) //Si c'est une zone 
                {
                    foreach (Cell cell in map.GetCellsInArea(_cursorPosition.X, _cursorPosition.Y, _area)) //Pour chaque case dans la zone on change la couleur 
                    {
                        mapConsole.SetBackColor(cell.X, cell.Y, Swatch.DbSun);
                    }
                }
                else if (_selectionType == SelectionType.Line) //Si c'est une ligne 
                {
                    foreach (Cell cell in map.GetCellsAlongLine(player.X, player.Y, _cursorPosition.X, _cursorPosition.Y)) //Pour chaque case le long de la lgne on change la couleur
                    {
                        mapConsole.SetBackColor(cell.X, cell.Y, Swatch.DbSun);
                    }
                }

                mapConsole.SetBackColor(_cursorPosition.X, _cursorPosition.Y, Swatch.DbLight); //On change la couleur du curseur (centre de la zone, début de la ligne, cible simple)
            }
        }
    }
}
