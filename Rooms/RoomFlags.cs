namespace DunGen.Rooms;

[Flags]
internal enum RoomFlags : int
{
	   NoRoom = 0x0000,

	NorthDoor = 0x0001, EastDoor = 0x0002,  SouthDoor = 0x0004, WestDoor = 0x0008, // Doors
	 Standard = 0x0010, Corridor = 0x0020,  LargeRoom = 0x0040, Entrance = 0x0080, // Structure Types
	     Trap = 0x0100, Treasure = 0x0200, Decoration = 0x0400,   Shrine = 0x0800, // Room Modifiers
}

internal enum Direction : int
{
	North = 0x0001, East = 0x0002, South = 0x0004, West = 0x0008,
}