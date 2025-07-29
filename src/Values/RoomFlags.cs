using DunGen.Generation;

namespace DunGen.Values;

/// <summary>
/// Represents the properties and characteristics of a <see cref="Room"/>.
/// </summary>
[Flags]
internal enum RoomFlags : int
{
	/// <summary> Indicates that no room is present. </summary>
	NoRoom = 0x0000,

	#region Doors

	/// <summary> Indicates the presence of a northern door. </summary>
	NorthDoor = 0x0001,

	/// <summary> Indicates the presence of an eastern door. </summary>
	EastDoor = 0x0002,

	/// <summary> Indicates the presence of a southern door. </summary>
	SouthDoor = 0x0004,

	/// <summary> Indicates the presence of a western door. </summary>
	WestDoor = 0x0008,

	#endregion

	/// <summary> Indicates that the room is of standard shape. </summary>
	Standard = 0x0010,

	/// <summary> Indicates that the room is a corridor. </summary>
	Corridor = 0x0020,

	/// <summary> Indicates that the room is a hall, meaning it can combine with adjacent halls, if any. </summary>
	Hall = 0x0040,

	/// <summary> Indicates that the room acts as an entrance to the dungeon / floor. </summary>
	Entrance = 0x0080,

	/// <summary> Indicates the presence of a trap. </summary>
	Trap = 0x0100,

	/// <summary> Indicates the presence of treasure. </summary>
	Treasure = 0x0200,

	/// <summary> Indicates the presence of a shrine. </summary>
	Shrine = 0x0400,

	/// <summary> Undefined value. </summary>
	Filler = 0x0800,
}