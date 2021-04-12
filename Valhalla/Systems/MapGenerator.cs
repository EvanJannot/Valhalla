using System;
using System.Collections.Generic;
using System.Linq;
using RogueSharp;
using RogueSharp.DiceNotation;
using RogueSharpRLNetSamples.Core;

namespace RogueSharpRLNetSamples.Systems
{
    public class MapGenerator
    {
        private readonly int _width;
        private readonly int _height;
        private readonly int _maxRooms;
        private readonly int _roomMaxSize;
        private readonly int _roomMinSize;
        private readonly int _level;
        private readonly int _monde;
        private readonly DungeonMap _map;
        private readonly EquipmentGenerator _equipmentGenerator;

        public MapGenerator(int width, int height, int maxRooms, int roomMaxSize, int roomMinSize, int level, int monde)
        {

            _width = width;
            _height = height;
            _maxRooms = maxRooms;
            _roomMaxSize = roomMaxSize;
            _roomMinSize = roomMinSize;
            _level = level;
            _monde = monde;
            _map = new DungeonMap(_level,_monde);
            _equipmentGenerator = new EquipmentGenerator(level);


        }

        public DungeonMap CreateMap(int monde, int lvl)
        {
            _map.Initialize(_width, _height);

            for (int r = 0; r < _maxRooms; r++)
            {
                if(lvl == 6)
                {
                    int roomWidth = Game.Random.Next(_roomMinSize, _roomMaxSize);
                    int roomHeight = Game.Random.Next(_roomMinSize, _roomMaxSize);
                    int roomXPosition = Game.Random.Next(0, _width - roomWidth - 1);
                    int roomYPosition = Game.Random.Next(0, _height - roomHeight - 1);
                    var newRoom = new Rectangle(roomXPosition, roomYPosition, roomWidth, roomHeight);
                    bool newRoomIntersects = _map.Rooms.Any(room => newRoom.Intersects(room));
                    while (newRoomIntersects)
                    {
                        roomXPosition = Game.Random.Next(0, _width - roomWidth - 1);
                        roomYPosition = Game.Random.Next(0, _height - roomHeight - 1);
                        newRoom = new Rectangle(roomXPosition, roomYPosition, roomWidth, roomHeight);
                        newRoomIntersects = _map.Rooms.Any(room => newRoom.Intersects(room));

                    }
                    _map.Rooms.Add(newRoom);
                }

                else 
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

            foreach (Rectangle room in _map.Rooms)
            {
                CreateMap(room);
            }

            for (int r = 0; r < _map.Rooms.Count; r++)
            {
                if (r == 0)
                {
                    continue;
                }

                int previousRoomCenterX = _map.Rooms[r - 1].Center.X;
                int previousRoomCenterY = _map.Rooms[r - 1].Center.Y;
                int currentRoomCenterX = _map.Rooms[r].Center.X;
                int currentRoomCenterY = _map.Rooms[r].Center.Y;

                if (Game.Random.Next(0, 2) == 0)
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

            foreach (Rectangle room in _map.Rooms)
            {
                CreateDoors(room);
            }

            CreateStairs();

            PlacePlayer();


            if (monde == 0 | monde==4)
            {
                return _map;
            }

            else if (lvl == 5)
            {

                PlaceEquipment(lvl);

                PlaceItems(lvl);

                PlaceAbility(lvl);
                return _map;
            }

            else if (lvl == 6)
            {
                PlaceMonsters(lvl);
                return _map;
            }

            else
            {
                PlaceMonsters(lvl);

                PlaceEquipment(lvl);

                PlaceItems(lvl);

                PlaceAbility(lvl);
                return _map;
            }


        }

        private void CreateMap(Rectangle room)
        {
            for (int x = room.Left + 1; x < room.Right; x++)
            {
                for (int y = room.Top + 1; y < room.Bottom; y++)
                {
                    if (_monde == 0)
                    {
                        if (x == _map.Rooms.Last().Center.X && y == _map.Rooms.Last().Center.Y - 2)
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

        private void CreateHorizontalTunnel(int xStart, int xEnd, int yPosition)
        {
            for (int x = Math.Min(xStart, xEnd); x <= Math.Max(xStart, xEnd); x++)
            {
                _map.SetCellProperties(x, yPosition, true, true);
            }
        }

        private void CreateVerticalTunnel(int yStart, int yEnd, int xPosition)
        {
            for (int y = Math.Min(yStart, yEnd); y <= Math.Max(yStart, yEnd); y++)
            {
                _map.SetCellProperties(xPosition, y, true, true);
            }
        }

        private void CreateDoors(Rectangle room)
        {
            int xMin = room.Left;
            int xMax = room.Right;
            int yMin = room.Top;
            int yMax = room.Bottom;

            List<Cell> borderCells = _map.GetCellsAlongLine(xMin, yMin, xMax, yMin).ToList();
            borderCells.AddRange(_map.GetCellsAlongLine(xMin, yMin, xMin, yMax));
            borderCells.AddRange(_map.GetCellsAlongLine(xMin, yMax, xMax, yMax));
            borderCells.AddRange(_map.GetCellsAlongLine(xMax, yMin, xMax, yMax));

            foreach (Cell cell in borderCells)
            {
                if (IsPotentialDoor(cell))
                {
                    if (_level == 6)
                    {
                        _map.SetCellProperties(cell.X, cell.Y, false, false);
                        _map.Doors.Add(new Door
                        {
                            X = cell.X,
                            Y = cell.Y,
                            IsOpen = false
                        });
                    }
                    else
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

        private bool IsPotentialDoor(Cell cell)
        {
            if (!cell.IsWalkable)
            {
                return false;
            }

            Cell right = _map.GetCell(cell.X + 1, cell.Y);
            Cell left = _map.GetCell(cell.X - 1, cell.Y);
            Cell top = _map.GetCell(cell.X, cell.Y - 1);
            Cell bottom = _map.GetCell(cell.X, cell.Y + 1);

            if (_map.GetDoor(cell.X, cell.Y) != null ||
                 _map.GetDoor(right.X, right.Y) != null ||
                 _map.GetDoor(left.X, left.Y) != null ||
                 _map.GetDoor(top.X, top.Y) != null ||
                 _map.GetDoor(bottom.X, bottom.Y) != null)
            {
                return false;
            }

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

        private void CreateStairs()
        {
            if (_monde == 0)
            {
                _map.StairsDown = new Stairs
                {
                    X = _map.Rooms.Last().Center.X,
                    Y = _map.Rooms.Last().Center.Y-7,
                    IsUp = false
                };
            }
            else
            {
                _map.StairsUp = new Stairs
                {
                    X = _map.Rooms.First().Center.X + 1,
                    Y = _map.Rooms.First().Center.Y,
                    IsUp = true
                };
                _map.StairsDown = new Stairs
                {
                    X = _map.Rooms.Last().Center.X,
                    Y = _map.Rooms.Last().Center.Y,
                    IsUp = false
                };
            }

        }

        private void PlaceMonsters(int lvl)
        {
            if (lvl == 6)
            {
                foreach (var room in _map.Rooms)
                {
                    if (room == _map.Rooms[0])
                    {
                        if (_map.DoesRoomHaveWalkableSpace(room))
                        {
                            Point randomRoomLocation = _map.GetRandomLocationInRoom(room);
                            if (randomRoomLocation != null)
                            {
                                _map.AddMonster(ActorGenerator.CreateMonster(_level, _map.GetRandomLocationInRoom(room),_monde));
                            }
                        }

                    }
                }
            }
            else
            {
                foreach (var room in _map.Rooms)
                {
                    if (Dice.Roll("1D10") < 7)
                    {
                        var numberOfMonsters = Dice.Roll("1D4");
                        for (int i = 0; i < numberOfMonsters; i++)
                        {
                            if (_map.DoesRoomHaveWalkableSpace(room))
                            {
                                Point randomRoomLocation = _map.GetRandomLocationInRoom(room);
                                if (randomRoomLocation != null)
                                {
                                    _map.AddMonster(ActorGenerator.CreateMonster(_level, _map.GetRandomLocationInRoom(room),_monde));
                                }
                            }
                        }
                    }
                }
            }
            
        }

        private void PlaceEquipment(int lvl)
        {
            if (lvl == 5)
            {
                foreach (var room in _map.Rooms)
                {
                    if (_map.DoesRoomHaveWalkableSpace(room))
                    {
                        Point randomRoomLocation = _map.GetRandomLocationInRoom(room);
                        if (randomRoomLocation != null)
                        {
                            Core.Equipment equipment;
                            try
                            {
                                equipment = _equipmentGenerator.CreateEquipment();
                            }
                            catch (InvalidOperationException)
                            {
                                // no more equipment to generate so just quit adding to this level
                                return;
                            }
                            Point location = _map.GetRandomLocationInRoom(room);
                            _map.AddTreasure(location.X, location.Y, equipment);
                        }
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
                                Core.Equipment equipment;
                                try
                                {
                                    equipment = _equipmentGenerator.CreateEquipment();
                                }
                                catch (InvalidOperationException)
                                {
                                    // no more equipment to generate so just quit adding to this level
                                    return;
                                }
                                Point location = _map.GetRandomLocationInRoom(room);
                                _map.AddTreasure(location.X, location.Y, equipment);
                            }
                        }
                    }
                }
            }
        }

        private void PlaceItems(int lvl)
        {
            if (lvl == 5)
            {
                foreach (var room in _map.Rooms)
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

        private void PlacePlayer()
        {
            Player player = ActorGenerator.CreatePlayer();

            player.X = _map.Rooms[0].Center.X;
            player.Y = _map.Rooms[0].Center.Y;

            _map.AddPlayer(player);
        }

        private void PlaceAbility(int lvl)
        {
            if (_level == 1 || _level % 3 == 0 || _level == 5)
            {
                try
                {
                    var ability = AbilityGenerator.CreateAbility();
                    int roomIndex = Game.Random.Next(0, _map.Rooms.Count - 1);
                    Point location = _map.GetRandomLocationInRoom(_map.Rooms[roomIndex]);
                    _map.AddTreasure(location.X, location.Y, ability);
                }
                catch (InvalidOperationException)
                {
                }
            }
        }
    }
}