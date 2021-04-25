using System;
using System.Collections.Generic;
using System.Linq;
using RogueSharp;
using RogueSharp.DiceNotation;
using Valhalla.Core;

namespace Valhalla.Systems
{
    public class MapGenerator
    {
        //La map est défini par sa largeur, sa hauteur, le nombre maximum de salles, leur taille max et min, le niveau et le monde 
        private readonly int _width;
        private readonly int _height;
        private readonly int _maxRooms;
        private readonly int _roomMaxSize;
        private readonly int _roomMinSize;
        private readonly int _level;
        private readonly int _monde;
        private readonly DungeonMap _map;
        private readonly EquipmentGenerator _equipmentGenerator;

        //Construit la map en fonction des dimensions, du nombre de salles et du niveau
        public MapGenerator(int width, int height, int maxRooms, int roomMaxSize, int roomMinSize, int level, int monde)
        {

            _width = width;
            _height = height;
            _maxRooms = maxRooms;
            _roomMaxSize = roomMaxSize;
            _roomMinSize = roomMinSize;
            _level = level;
            _monde = monde;
            _map = new DungeonMap(_level, _monde);
            _equipmentGenerator = new EquipmentGenerator(monde);


        }

        //Fonction qui retourne une map créée à partir des différentes caractéristiques de cette dernière 
        public DungeonMap CreateMap(int monde, int lvl, int atkX, int defX, int awaX, int hpX, string name, char symbol)
        {
            _map.Initialize(_width, _height);

            for (int r = 0; r < _maxRooms; r++)
            {
                if (lvl == 6) //Pour le niveau du boss, on veut deux salles distinctes
                {
                    int roomWidth = Game.Random.Next(_roomMinSize, _roomMaxSize);
                    int roomHeight = Game.Random.Next(_roomMinSize, _roomMaxSize);
                    int roomXPosition = Game.Random.Next(0, _width - roomWidth - 1);
                    int roomYPosition = Game.Random.Next(0, _height - roomHeight - 1);
                    var newRoom = new Rectangle(roomXPosition, roomYPosition, roomWidth, roomHeight);
                    bool newRoomIntersects = _map.Rooms.Any(room => newRoom.Intersects(room));
                    while (newRoomIntersects) //Tant que les deux salles se coupent ont refait une nouvelle salle pour en avoir deux distinctes
                    {
                        roomXPosition = Game.Random.Next(0, _width - roomWidth - 1);
                        roomYPosition = Game.Random.Next(0, _height - roomHeight - 1);
                        newRoom = new Rectangle(roomXPosition, roomYPosition, roomWidth, roomHeight);
                        newRoomIntersects = _map.Rooms.Any(room => newRoom.Intersects(room));

                    }
                    _map.Rooms.Add(newRoom); //On ajoute la nouvelle salle 
                }

                else //Pour les autres niveau pas besoin de gérer cela, les salles peuvent se couper ou non
                {
                    int roomWidth = Game.Random.Next(_roomMinSize, _roomMaxSize);
                    int roomHeight = Game.Random.Next(_roomMinSize, _roomMaxSize);
                    int roomXPosition = Game.Random.Next(0, _width - roomWidth - 1);
                    int roomYPosition = Game.Random.Next(0, _height - roomHeight - 1);

                    var newRoom = new Rectangle(roomXPosition, roomYPosition, roomWidth, roomHeight);
                    bool newRoomIntersects = _map.Rooms.Any(room => newRoom.Intersects(room));
                    if (!newRoomIntersects)
                    {
                        _map.Rooms.Add(newRoom);
                    }
                }

            }

            foreach (Rectangle room in _map.Rooms) //Pour chaque salle on créé l'intérieur 
            {
                CreateMap(room);
            }

            for (int r = 0; r < _map.Rooms.Count; r++) //Permet de créer les tunnels entre les salles 
            {
                if (r == 0) 
                {
                    continue;
                }

                int previousRoomCenterX = _map.Rooms[r - 1].Center.X; //On récupère les coordonnées (x,y) de la salle d'avant
                int previousRoomCenterY = _map.Rooms[r - 1].Center.Y;
                int currentRoomCenterX = _map.Rooms[r].Center.X; //On récupère les coordonnées de cette salle 
                int currentRoomCenterY = _map.Rooms[r].Center.Y;

                if (Game.Random.Next(0, 2) == 0) // De manière aléatoire, on créé soit un tunnel horizontal puis vertical soit vertical puis horizontal 
                {
                    CreateHorizontalTunnel(previousRoomCenterX, currentRoomCenterX, previousRoomCenterY);
                    CreateVerticalTunnel(previousRoomCenterY, currentRoomCenterY, currentRoomCenterX);
                }
                else
                {
                    CreateVerticalTunnel(previousRoomCenterY, currentRoomCenterY, previousRoomCenterX);
                    CreateHorizontalTunnel(previousRoomCenterX, currentRoomCenterX, currentRoomCenterY);
                }
            }

            foreach (Rectangle room in _map.Rooms) //Pour chaque salle, on créé les portes 
            {
                CreateDoors(room);
            }

            CreateStairs(); //On créé les escaliers 


            PlacePlayer(atkX, defX, awaX, hpX, name, symbol); //On place le joueur 


            if (monde == 0 | monde == 4) //Pour le monde de début et de fin on s'arrête là
            {
                return _map;
            }

            else if (lvl == 5) //Pour le niveau du marchand on place de l'equipement, des objets et des capacités
            {

                PlaceEquipment(lvl);

                PlaceItems(lvl);

                PlaceAbility();
                return _map;
            }

            else if (lvl == 6) //Pour le niveau du boss on place juste un monstre 
            {
                PlaceMonsters(lvl);
                return _map;
            }

            else //Pour les autres niveaux on place tout (monstres, equipements, objets, capacités)
            {
                PlaceMonsters(lvl);

                PlaceEquipment(lvl);

                PlaceItems(lvl);

                PlaceAbility();
                return _map;
            }


        }

        //Fonction qui permet de créer les salles 
        private void CreateMap(Rectangle room)
        {
            for (int x = room.Left + 1; x < room.Right; x++)
            {
                for (int y = room.Top + 1; y < room.Bottom; y++)
                {
                    if (_monde == 0 | _monde == 4 | Game.LEVEL == 5)
                    {
                        if (x == _map.Rooms.Last().Center.X && y == _map.Rooms.Last().Center.Y - 2) //Pour le monde de début, de fin et des marchands on affiche un pnj 
                        {
                            _map.SetCellProperties(x, y, true, false);
                        }
                        else
                        {
                            _map.SetCellProperties(x, y, true, true);
                        }
                    }
                    else
                    {
                        _map.SetCellProperties(x, y, true, true);
                    }

                }
            }
        }


        //Les deux fonctions ci dessous permettent de créer des tunnels reliant deux salles à l'aide des coordonnées obtenues précédemment
        private void CreateHorizontalTunnel(int xStart, int xEnd, int yPosition) //Tunnel horizontale
        {
            for (int x = Math.Min(xStart, xEnd); x <= Math.Max(xStart, xEnd); x++)
            {
                _map.SetCellProperties(x, yPosition, true, true);
            }
        }

        private void CreateVerticalTunnel(int yStart, int yEnd, int xPosition) //Tunnel verticale 
        {
            for (int y = Math.Min(yStart, yEnd); y <= Math.Max(yStart, yEnd); y++)
            {
                _map.SetCellProperties(xPosition, y, true, true);
            }
        }

        //Fonction pour créer les portes 
        private void CreateDoors(Rectangle room)
        {
            //On récupère les positions des murs verticaux et horizontaux
            int xMin = room.Left;
            int xMax = room.Right;
            int yMin = room.Top;
            int yMax = room.Bottom;

            //On récupère l'ensemble des murs
            List<Cell> borderCells = _map.GetCellsAlongLine(xMin, yMin, xMax, yMin).ToList();
            borderCells.AddRange(_map.GetCellsAlongLine(xMin, yMin, xMin, yMax));
            borderCells.AddRange(_map.GetCellsAlongLine(xMin, yMax, xMax, yMax));
            borderCells.AddRange(_map.GetCellsAlongLine(xMax, yMin, xMax, yMax));

            foreach (Cell cell in borderCells) //Pour chaque mur 
            {
                if (IsPotentialDoor(cell)) //Si c'est potentiellement une porte 
                {
                    if (_level == 6) //Si on est dans la salle du boss
                    {
                        _map.SetCellProperties(cell.X, cell.Y, false, false); //On ne peut pas passer la porte 
                        _map.Doors.Add(new Door
                        {
                            X = cell.X,
                            Y = cell.Y,
                            IsOpen = false
                        });
                    }
                    else //Sinon on peut la passer 
                    {
                        _map.SetCellProperties(cell.X, cell.Y, false, true);
                        _map.Doors.Add(new Door
                        {
                            X = cell.X,
                            Y = cell.Y,
                            IsOpen = false
                        });
                    }
                }
            }
        }

        //Fonction pour vérifier si une case peut être une porte 
        private bool IsPotentialDoor(Cell cell)
        {
            if (!cell.IsWalkable) //Si on ne peut pas marcher dessus ce n'est pas possible 
            {
                return false;
            }

            //Sinon c'est que c'est l'entrée d'un tunnel 
            //On récupère les cases autour 
            Cell right = _map.GetCell(cell.X + 1, cell.Y);
            Cell left = _map.GetCell(cell.X - 1, cell.Y);
            Cell top = _map.GetCell(cell.X, cell.Y - 1);
            Cell bottom = _map.GetCell(cell.X, cell.Y + 1);

            //Si il y a une porte autour on ne fait rien 
            if (_map.GetDoor(cell.X, cell.Y) != null || 
                 _map.GetDoor(right.X, right.Y) != null ||
                 _map.GetDoor(left.X, left.Y) != null ||
                 _map.GetDoor(top.X, top.Y) != null ||
                 _map.GetDoor(bottom.X, bottom.Y) != null)
            {
                return false;
            }

            //Si il est possible de marcher à gauche et à droite ou au dessus et en dessous on fait une porte 
            if (right.IsWalkable && left.IsWalkable && !top.IsWalkable && !bottom.IsWalkable)
            {
                return true;
            }
            if (!right.IsWalkable && !left.IsWalkable && top.IsWalkable && bottom.IsWalkable)
            {
                return true;
            }
            return false;
        }

        //Fonction pour créer les escaliers 
        private void CreateStairs()
        {
            if (_monde == 0 | _monde == 4) //Pour les mondes de début et fin on ne fait pas d'escalier qui monte 
            {
                _map.StairsDown = new Stairs
                {
                    X = _map.Rooms.Last().Center.X,
                    Y = _map.Rooms.Last().Center.Y - 7,
                    IsUp = false
                };
            }
            else //Pour les autres on a un escalier qui monte et un qui descend 
            {
                if (Game.LEVEL == 5) //Pour le niveau du marchand 
                {
                    _map.StairsUp = new Stairs //Les escaliers sont au fond de la salle 
                    {
                        X = _map.Rooms.First().Center.X,
                        Y = _map.Rooms.First().Center.Y+7,
                        IsUp = true
                    };
                    _map.StairsDown = new Stairs
                    {
                        X = _map.Rooms.Last().Center.X,
                        Y = _map.Rooms.Last().Center.Y - 6,
                        IsUp = false
                    };
                }
                else //Sinon il est au centre de la salle 
                {
                    _map.StairsUp = new Stairs //Celui qui monte est dans la salle où apparait le joueur 
                    {
                        X = _map.Rooms.First().Center.X + 1,
                        Y = _map.Rooms.First().Center.Y,
                        IsUp = true
                    };
                    _map.StairsDown = new Stairs //Celui qui descend est dans la dernière salle 
                    {
                        X = _map.Rooms.Last().Center.X,
                        Y = _map.Rooms.Last().Center.Y,
                        IsUp = false
                    };
                }
            }

        }

        //Fonction pour créer et placer des monstres sur la map
        private void PlaceMonsters(int lvl)
        {
            if (lvl == 6) //Pour le niveau du boss
            {
                foreach (var room in _map.Rooms)
                {
                    if (room == _map.Rooms[0]) //On place le monstre dans la première salle 
                    {
                        if (_map.DoesRoomHaveWalkableSpace(room)) 
                        {
                            Point randomRoomLocation = _map.GetRandomLocationInRoom(room);
                            if (randomRoomLocation != null)
                            {
                                _map.AddMonster(ActorGenerator.CreateMonster(_level, _map.GetRandomLocationInRoom(room), _monde));
                            }
                        }

                    }
                }
            }
            else //Sinon 
            {
                foreach (var room in _map.Rooms)  //Pour chaque salle 
                {
                    if (Dice.Roll("1D10") < 7) //On a 6 chances sur 10 d'avoir un monstre ou plusieurs dedans 
                    {
                        var numberOfMonsters = Dice.Roll("1D4"); //On peut avoir jusqu'à 4 monstres dedans 
                        for (int i = 0; i < numberOfMonsters; i++) //Pour chaque monstre on l'ajoute à la map
                        {
                            if (_map.DoesRoomHaveWalkableSpace(room))
                            {
                                Point randomRoomLocation = _map.GetRandomLocationInRoom(room);
                                if (randomRoomLocation != null)
                                {
                                    _map.AddMonster(ActorGenerator.CreateMonster(_level, _map.GetRandomLocationInRoom(room), _monde));
                                }
                            }
                        }
                    }
                }
            }

        }

        //Fonction pour créer et placer des équipements sur la map
        private void PlaceEquipment(int lvl)
        {
            if (lvl == 5) //Si on est au niveau du marchand on aura forcément un équipement 
            {
                foreach (var room in _map.Rooms)
                {
                    if (_map.DoesRoomHaveWalkableSpace(room))
                    {

                        int _xequ = room.Center.X; //On place l'equipement au centre de la salle face au marchand 
                        int _yequ = room.Center.Y - 1;

                        Core.Equipment equipment;
                        try
                        {
                            equipment = _equipmentGenerator.CreateEquipment();
                        }
                        catch (InvalidOperationException)
                        {
                            return;
                        }
                        _map.AddTreasure(_xequ, _yequ, equipment); //On ajoute l'equipement à la map

                    }

                }
            }
            else //Pour les autres salles 
            {
                foreach (var room in _map.Rooms) //pour chaque salle 
                {
                    if (Dice.Roll("1D10") < 3) //On a 2 chances sur 10 d'avoir un equipement dedans 
                    {
                        if (_map.DoesRoomHaveWalkableSpace(room))
                        {
                            Point randomRoomLocation = _map.GetRandomLocationInRoom(room); //On place l'équipement quelque part au hasard 
                            if (randomRoomLocation != null)
                            {
                                Core.Equipment equipment;
                                try
                                {
                                    equipment = _equipmentGenerator.CreateEquipment();
                                }
                                catch (InvalidOperationException)
                                {
                                    return;
                                }
                                Point location = _map.GetRandomLocationInRoom(room);
                                _map.AddTreasure(location.X, location.Y, equipment); //On ajoute l'équipement à la map
                            }
                        }
                    }
                }
            }
        }

        //Fonction pour créer et places des objets sur la map le fonctionnement est le même que pour l'équipement
        private void PlaceItems(int lvl)
        {
            if (lvl == 5) 
            {
                foreach (var room in _map.Rooms)
                {

                    if (_map.DoesRoomHaveWalkableSpace(room))
                    {
                        int _xobj = room.Center.X - 1;
                        int _yobj = room.Center.Y - 1;
                        Item item = ItemGenerator.CreateItem();
                        _map.AddTreasure(_xobj, _yobj, item);

                    }

                }
            }
            else
            {
                foreach (var room in _map.Rooms)
                {
                    if (Dice.Roll("1D10") < 3)
                    {
                        if (_map.DoesRoomHaveWalkableSpace(room))
                        {
                            Point randomRoomLocation = _map.GetRandomLocationInRoom(room);
                            if (randomRoomLocation != null)
                            {
                                Item item = ItemGenerator.CreateItem();
                                Point location = _map.GetRandomLocationInRoom(room);
                                _map.AddTreasure(location.X, location.Y, item);
                            }
                        }
                    }
                }
            }
        }

        //Fonction pour créer et placer le joueur sur la map 
        private void PlacePlayer(int atkX, int defX, int awaX, int hpX, string name, char symbol)
        {
            if (_level == 5) //Au niveau du marchand on place le joueur tout en bas de la salle
            {
                Player player = ActorGenerator.CreatePlayer(atkX, defX, awaX, hpX, name, symbol);

                player.X = _map.Rooms[0].Center.X;
                player.Y = _map.Rooms[0].Center.Y+7;

                _map.AddPlayer(player);
            }
            else //Pour les autres niveaux le joueur est placé au milieu
            {
                Player player = ActorGenerator.CreatePlayer(atkX, defX, awaX, hpX, name, symbol);

                player.X = _map.Rooms[0].Center.X;
                player.Y = _map.Rooms[0].Center.Y;

                _map.AddPlayer(player);
            }
        }

        //Pour les capacités c'est la même chose que les objets et l'équipement 
        private void PlaceAbility()
        {
            if (_level == 5)
            {
                foreach (var room in _map.Rooms)
                {

                    if (_map.DoesRoomHaveWalkableSpace(room))
                    {
                        int _xabi = room.Center.X + 1;
                        int _yabi = room.Center.Y - 1;
                        Ability ability = AbilityGenerator.CreateAbility();
                        _map.AddTreasure(_xabi, _yabi, ability);
                    }
                }
            }
            else
            {
                foreach (var room in _map.Rooms)
                {
                    if (Dice.Roll("1D10") < 2)
                    {
                        if (_map.DoesRoomHaveWalkableSpace(room))
                        {
                            Point randomRoomLocation = _map.GetRandomLocationInRoom(room);
                            if (randomRoomLocation != null)
                            {
                                Ability ability = AbilityGenerator.CreateAbility();
                                Point location = _map.GetRandomLocationInRoom(room);
                                _map.AddTreasure(location.X, location.Y, ability);
                            }
                        }
                    }
                }
            }
            
        }
    }
}