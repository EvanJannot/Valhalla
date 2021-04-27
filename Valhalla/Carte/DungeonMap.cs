using System.Collections.Generic;
using System.Linq;
using System;
using RLNET;
using RogueSharp;
using Valhalla.Interfaces;

namespace Valhalla.Core
{
    public class DungeonMap : Map
    {
        private readonly List<Monster> _monsters;
        private readonly List<TreasurePile> _treasurePiles;
        private readonly int _level;
        private readonly int _monde;

        public List<Rectangle> Rooms;
        public List<Door> Doors;
        public Stairs StairsUp;
        public Stairs StairsDown;

        //La map dépend du niveau et du monde
        public DungeonMap(int lvl, int monde)
        {
            _monsters = new List<Monster>();
            _treasurePiles = new List<TreasurePile>();
            _level = lvl;
            _monde = monde;
            Game.SchedulingSystem.Clear();

            Rooms = new List<Rectangle>();
            Doors = new List<Door>();
        }

        //Fonction pour ajouter des monstres 
        public void AddMonster(Monster monster)
        {
            //On ajoute le monstre à la liste des monstres
            _monsters.Add(monster);
            //Il n'est pas possible de marcher sur la case où il y a un monstre
            SetIsWalkable(monster.X, monster.Y, false);
            //On ajoute le monstre au système de gestion du temps
            Game.SchedulingSystem.Add(monster);
        }

        //Fonction pour supprimer des monstres 
        public void RemoveMonster(Monster monster)
        {
            //On l'enlève de la liste
            _monsters.Remove(monster);
            //La case sur laquelle il était est maintenant accessible
            SetIsWalkable(monster.X, monster.Y, true);
            //On retire le monstre du système de gestion du temps
            Game.SchedulingSystem.Remove(monster);
            if (_level == 6) //Au niveau 6 (niveau du boss)
            {
                if (_monsters.Count() == 0) //Si il n'y a plus de monstre
                {
                    foreach (Door d in Doors) //On ouvre la porte 
                    {
                        this.SetCellProperties(d.X, d.Y, false, true);
                    }
                }
            }
        }

        //Fonction qui retourne un monstre se trouvant si il se trouve à des coordonnées (x,y)
        public Monster GetMonsterAt(int x, int y)
        {
            return _monsters.FirstOrDefault(m => m.X == x && m.Y == y);
        }

        //Retourne un point composé d'une coordonnée x et y sur lequel se trouve un monstre
        public IEnumerable<Point> GetMonsterLocations()
        {
            return _monsters.Select(m => new Point
            {
                X = m.X,
                Y = m.Y
            });
        }

        //Retourne un point sur lequel se trouve un monstre dans le champ de vision
        public IEnumerable<Point> GetMonsterLocationsInFieldOfView()
        {
            return _monsters.Where(monster => IsInFov(monster.X, monster.Y))
               .Select(m => new Point { X = m.X, Y = m.Y });
        }

        //Ajoute un trésor (equipement, objet, capacité) sur la map
        public void AddTreasure(int x, int y, ITreasure treasure)
        {
            _treasurePiles.Add(new TreasurePile(x, y, treasure));
        }

        //Ajoute le joueur sur la carte 
        public void AddPlayer(Player player)
        {
            Game.Player = player;
            SetIsWalkable(player.X, player.Y, false); //On ne peut pas marcher sur la case ou il se trouve (pour ne pas que les monstres soient sur le joueur)
            UpdatePlayerFieldOfView(); //On met à jour le champ de vision
            Game.SchedulingSystem.Add(player); //On ajoute le joueur au système de gestion du temps
        }

        //Fonction qui met à jour le champ de vision
        public void UpdatePlayerFieldOfView()
        {
            Player player = Game.Player;
            ComputeFov(player.X, player.Y, player.Awareness, true); //A partir des coordonnées et des stats du perso on calcul le nouveau champ de vision
            foreach (Cell cell in GetAllCells())
            {
                if (IsInFov(cell.X, cell.Y)) //Pour chaque case, si elle est dans le champ de vision on la passe en "explorée"
                {
                    SetCellProperties(cell.X, cell.Y, cell.IsTransparent, cell.IsWalkable, true);
                }
            }
        }

        //Fonction qui définit la position d'un acteur et effectue les actions liées à l'endroit où il se trouve
        public bool SetActorPosition(Actor actor, int x, int y)
        {
            if (GetCell(x, y).IsWalkable) //Si il est possible de marcher sur la case
            {
                PickUpTreasure(actor, x, y); //L'acteur récupère les trésors sur la case si il y en a
                SetIsWalkable(actor.X, actor.Y, true); //La case sur laquelle il se trouvait est maintenant accessible
                actor.X = x; //On définit sa nouvelle position
                actor.Y = y;
                SetIsWalkable(actor.X, actor.Y, false); //Sa nouvelle position n'est plus accessible 
                OpenDoor(actor, x, y); //Il ouvre une porte si il y en a une
                if (actor is Player) //Si c'est le joueur, on met à jour son champ de vision
                {
                    UpdatePlayerFieldOfView();
                }
                return true;
            }
            return false; //Si on ne peut pas marcher sur la case, on ne met pas à jour la position et les infos
        }

        //On récupère la porte aux coordonnées (x,y)
        public Door GetDoor(int x, int y)
        {
            return Doors.SingleOrDefault(d => d.X == x && d.Y == y);
        }

        //Fonction qui permet à un acteur d'ouvrir la porte aux coordonnées (x,y)
        private void OpenDoor(Actor actor, int x, int y)
        {
            Door door = GetDoor(x, y); //On récupère la porte 
            if (door != null && !door.IsOpen) //Si il n'y en a une et qu'elle n'est pas déjà ouverte
            {
                door.IsOpen = true; //On ouvre la porte 
                var cell = GetCell(x, y); //On récupère la case de la porte et on la passe en explorée et marchable
                SetCellProperties(x, y, true, true, cell.IsExplored);
            }
        }

        //Fonction pour ajouter de l'or à un emplacement (x,y)
        public void AddGold(int x, int y, int amount)
        {
            if (amount > 0)
            {
                AddTreasure(x, y, new Gold(amount));
            }
        }

        //Fonction permettant à un acteur de récupérer un trésor en un point (x,y)
        private void PickUpTreasure(Actor actor, int x, int y)
        {
            //On récupère les trésors à l'emplacement (x,y)
            List<TreasurePile> treasureAtLocation = _treasurePiles.Where(g => g.X == x && g.Y == y).ToList();
            foreach (TreasurePile treasurePile in treasureAtLocation) //Pour chaque trésor de la liste 
            {
                if (Game.LEVEL==5) //Si on est au niveau du marchand, il faut payer une somme d'argent pour récupérer les trésors
                {
                    if (actor.Gold >= 100*_monde) //Cette somme vaut 100 multiplié par le monde 
                    {
                        if (treasurePile.Treasure.PickUp(actor)) //On regarde si il est possible de prendre l'objet 
                        {
                            actor.Gold -= 100; //On enlève l'or au joueur 
                            _treasurePiles.Remove(treasurePile); //On retire le trésor
                        }
                    }
                }
                else if (Game.LEVEL != 5) //Sinon il est possible de récupérer le trésor
                {
                    if (treasurePile.Treasure.PickUp(actor)) //On regarde si l'acteur peut prendre l'objet (les monstres ne peuvent pas prendre n'importe quel objet)
                    {
                        _treasurePiles.Remove(treasurePile); 
                    }
                }
            }
        }

        //Fonction pour vérifier si un joueur peut descendre au niveau suivant en vérifiant si le joueur est bien sur un escalier qui descend
        public bool CanMoveDownToNextLevel()
        {
            Player player = Game.Player; 

            return StairsDown.X == player.X && StairsDown.Y == player.Y;
        }

        //Fonction qui passe une case en accessible ou non
        public void SetIsWalkable(int x, int y, bool isWalkable)
        {
            Cell cell = GetCell(x, y);
            SetCellProperties(cell.X, cell.Y, cell.IsTransparent, isWalkable, cell.IsExplored);
        }

        //Permet de retourner un point aléatoire de la map
        public Point GetRandomLocation()
        {
            int roomNumber = Game.Random.Next(0, Rooms.Count - 1); //On prend une salle aléatoire
            Rectangle randomRoom = Rooms[roomNumber];

            if (!DoesRoomHaveWalkableSpace(randomRoom))
            {
                GetRandomLocation();
            }

            return GetRandomLocationInRoom(randomRoom); //On appelle la fonction qui retourne un point aléatoire de la salle 
        }

        //Permet de retourner un point aléatoire d'une salle 
        public Point GetRandomLocationInRoom(Rectangle room)
        {
            int x = Game.Random.Next(1, room.Width - 2) + room.X;
            int y = Game.Random.Next(1, room.Height - 2) + room.Y;
            if (!IsWalkable(x, y)) //Si on ne peut pas marcher sur la case on cherche un autre point 
            {
                GetRandomLocationInRoom(room);
            }
            return new Point(x, y);
        }

        //Fonction qui vérifie si la salle a une case accessible 
        public bool DoesRoomHaveWalkableSpace(Rectangle room)
        {
            for (int x = 1; x <= room.Width - 2; x++)
            {
                for (int y = 1; y <= room.Height - 2; y++)
                {
                    if (IsWalkable(x + room.X, y + room.Y))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        //Permet de dessiner la map à chaque fois que la console est mise à jour
        public void Draw(RLConsole mapConsole, RLConsole statConsole, RLConsole inventoryConsole, int monde)
        {
            mapConsole.Clear();
            foreach (Cell cell in GetAllCells()) //Pour chaque case de notre map on défini son symbole 
            {
                SetConsoleSymbolForCell(mapConsole, cell, monde);
            }

            foreach (Door door in Doors) //On dessine chaque porte
            {
                door.Draw(mapConsole, this, monde);
            }
            if (_monde != 0 & _monde !=4) //Si on est pas au monde 0 on dessine les escaliers montant et descendant
            {
                StairsUp.Draw(mapConsole, this, monde);
                StairsDown.Draw(mapConsole, this, monde);
            }
            else //Si on est au monde 0, il n'y a pas d'escalier montant
            {
                StairsDown.Draw(mapConsole, this, monde);
            }


            foreach (TreasurePile treasurePile in _treasurePiles) //Pour chaque item présent sur la map, on les dessine
            {
                IDrawable drawableTreasure = treasurePile.Treasure as IDrawable;
                drawableTreasure?.Draw(mapConsole, this, monde);
            }

            statConsole.Clear();

            //On dessine chacun des monstres sur la carte et si il est visible par le joueur, on l'affiche dans l'écran des stats 
            //L'indice i permet de déterminer la position du monstre dans l'écran des stats.
            int i = 0;
            foreach (Monster monster in _monsters)
            {
                monster.Draw(mapConsole, this, monde);
                if (IsInFov(monster.X, monster.Y))
                {
                    monster.DrawStats(statConsole, i);
                    i++;
                }
            }

            //On affiche le joueur sur la map ainsi que ses stats et son inventaire
            Player player = Game.Player;

            player.Draw(mapConsole, this, monde);
            player.DrawStats(statConsole);
            player.DrawInventory(inventoryConsole);
        }

        //Permet d'associer à chaque tuile son apparence
        private void SetConsoleSymbolForCell(RLConsole console, Cell cell, int monde)
        {
            if (!cell.IsExplored) //Si la tuile n'est pas explorée, elle n'a pas d'apparence
            {
                return;
            }

            // Quand une tuile est dans le champ de vision on l'affiche de manière plus clair que lorsqu'elle ne l'est pas 
            if (IsInFov(cell.X, cell.Y))
            {
                if (monde == 0 | monde == 4) //Pour le monde 0 et 4, le sol est en bois et les murs aussi
                {
                    if (cell.IsWalkable)
                    {
                        console.Set(cell.X, cell.Y, Colors.FloorFov, RLColor.Black, (char)154);
                    }
                    else
                    {
                        if (cell.X == this.Rooms.Last().Center.X && cell.Y == this.Rooms.Last().Center.Y - 2) //On affiche au centre de la salle un personnage d'un vieu sage 
                        {
                            console.Set(cell.X, cell.Y, Colors.Player, RLColor.Black, (char)94);
                        }
                        else
                        {
                            console.Set(cell.X, cell.Y, RLColor.Brown, RLColor.Black, (char)132);
                        }

                    }
                }
                if (monde == 1)
                {
                    // Pour le monde 1, c'est une foret avec le sol composé d'herbe et les murs d'arbres
                    if (cell.IsWalkable)
                    {
                        console.Set(cell.X, cell.Y, Colors.FloorFov, RLColor.Black, (char)8);
                    }
                    else
                    {
                        if (Game.LEVEL==5) //Au niveau 5 on affiche un marchand
                        {
                            if (cell.X == this.Rooms.Last().Center.X && cell.Y == this.Rooms.Last().Center.Y - 2) 
                            {
                                console.Set(cell.X, cell.Y, Colors.Player, RLColor.Black, (char)94);
                            }
                            else
                            {
                                console.Set(cell.X, cell.Y, Colors.WallFov, RLColor.Black, (char)47);
                            }
                        }
                        else
                        {
                            console.Set(cell.X, cell.Y, Colors.WallFov, RLColor.Black, (char)47);
                        }
                    }
                }
                else if (monde == 2)
                {
                    //Pour le monde 2, c'est une caverne avec le sol composé de plusieurs cailloux et les murs de gros rochers
                    if (cell.IsWalkable)
                    {
                        console.Set(cell.X, cell.Y, Colors.FloorFov, RLColor.Black, (char)95);
                    }
                    else
                    {
                        if (Game.LEVEL == 5) //Au niveau 5 on affiche un marchand
                        {
                            if (cell.X == this.Rooms.Last().Center.X && cell.Y == this.Rooms.Last().Center.Y - 2)
                            {
                                console.Set(cell.X, cell.Y, Colors.Player, RLColor.Black, (char)94);
                            }
                            else
                            {
                                console.Set(cell.X, cell.Y, Colors.WallFov, RLColor.Black, (char)30);
                            }
                        }
                        else
                        {
                            console.Set(cell.X, cell.Y, Colors.WallFov, RLColor.Black, (char)30);
                        }
                    }
                }
                else if (monde == 3)
                {
                    //Pour le monde 3, c'est un chateau avec l'intérieur en pierres taillées, les murs possèdent des colonnes en leur centre
                    if (cell.IsWalkable)
                    {
                        console.Set(cell.X, cell.Y, Colors.FloorFov, RLColor.Black, (char)64);
                    }
                    else
                    {
                        if (Game.LEVEL == 5) //Au niveau 5 on affiche un marchand
                        {
                            if (cell.X == this.Rooms.Last().Center.X && cell.Y == this.Rooms.Last().Center.Y - 2)
                            {
                                console.Set(cell.X, cell.Y, Colors.Player, RLColor.Black, (char)94);
                            }
                            else
                            {
                                console.Set(cell.X, cell.Y, Colors.WallFov, RLColor.Black, (char)31);
                            }
                        }
                        else
                        {
                            console.Set(cell.X, cell.Y, Colors.WallFov, RLColor.Black, (char)31);
                        }
                    }
                }
                else if (monde!=0 && monde != 1 && monde != 2 && monde != 3 && monde != 4) //Les mondes d'après sont tous pareils, sol en bois et murs en bois
                {
                    if (cell.IsWalkable)
                    {
                        console.Set(cell.X, cell.Y, Colors.FloorFov, RLColor.Black, (char)154);
                    }
                    else
                    {
                        if (Game.LEVEL == 5) //Au niveau 5 on affiche un marchand
                        {
                            if (cell.X == this.Rooms.Last().Center.X && cell.Y == this.Rooms.Last().Center.Y - 2) //On affiche au centre de la salle un personnage d'un vieu sage 
                            {
                                console.Set(cell.X, cell.Y, Colors.Player, RLColor.Black, (char)94);
                            }
                            else
                            {
                                console.Set(cell.X, cell.Y, RLColor.Brown, RLColor.Black, (char)132);
                            }
                        }
                        else
                        {
                            console.Set(cell.X, cell.Y, RLColor.Brown, RLColor.Black, (char)132);
                        }

                    }
                }
            }
            // Idem pour les cases hors du champ de vision mais en plus sombre
            else
            {
                if (monde == 0 | monde == 4)
                {
                    if (cell.IsWalkable)
                    {
                        console.Set(cell.X, cell.Y, Colors.Floor, RLColor.Black, (char)154);
                    }
                    else
                    {
                        if (cell.X == this.Rooms.Last().Center.X && cell.Y == this.Rooms.Last().Center.Y - 2)
                        {
                            console.Set(cell.X, cell.Y, Colors.Player, RLColor.Black, (char)94);
                        }
                        else
                        {
                            console.Set(cell.X, cell.Y, Colors.Wall, RLColor.Black, (char)132);
                        }
                    }
                }
                if (monde == 1)
                {
                    if (cell.IsWalkable)
                    {
                        console.Set(cell.X, cell.Y, Colors.Floor, RLColor.Black, (char)8);
                    }
                    else
                    {
                        console.Set(cell.X, cell.Y, Colors.Wall, RLColor.Black, (char)47);
                    }
                }
                else if (monde == 2)
                {
                    if (cell.IsWalkable)
                    {
                        console.Set(cell.X, cell.Y, Colors.Floor, RLColor.Black, (char)95);
                    }
                    else
                    {
                        console.Set(cell.X, cell.Y, Colors.Wall, RLColor.Black, (char)30);
                    }
                }
                else if (monde == 3)
                {
                    if (cell.IsWalkable)
                    {
                        console.Set(cell.X, cell.Y, Colors.Floor, RLColor.Black, (char)64);
                    }
                    else
                    {
                        console.Set(cell.X, cell.Y, Colors.Wall, RLColor.Black, (char)31);
                    }
                }
                else if (monde != 0 && monde != 1 && monde != 2 && monde != 3 && monde != 4)
                {
                    if (cell.IsWalkable)
                    {
                        console.Set(cell.X, cell.Y, Colors.Floor, RLColor.Black, (char)132);
                    }
                    else
                    {
                        console.Set(cell.X, cell.Y, Colors.Wall, RLColor.Black, (char)132);
                    }
                }

            }
        }
    }
}