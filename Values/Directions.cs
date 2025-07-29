namespace DunGen.Values;

/// <summary>
/// Represents the cardinal directions as a set of values, compatible with <see cref="RoomFlags"/>.
/// </summary>
internal enum Directions : int
{
	/// <summary> Represents cardinal north. </summary>
	North = 0x0001,

	/// <summary> Represents cardinal east. </summary>
	East = 0x0002,

	/// <summary> Represents cardinal south. </summary>
	South = 0x0004,

	/// <summary> Represents cardinal west. </summary>
	West = 0x0008,
}