using static DunGen.Dice;
using static DunGen.Rooms.Direction;

namespace DunGen.Rooms;
internal static class Generation
{
	public static Direction GetRandomDirection() => D4 switch { 1 => North, 2 => East, 3 => South, _ => West };
	public static Direction GetOppositeDirection(Direction direction) => direction switch  { North => South, East => West, South => North, _ => East, };
}

internal static class Ratios
{
}