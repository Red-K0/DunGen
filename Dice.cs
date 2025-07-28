using static System.Random;

namespace DunGen;
internal class Dice
{
	public static bool  D2 => Shared.Next(0, 2) == 1;
	public static int   D4 => Shared.Next(1,   5);
	public static int   D6 => Shared.Next(1,   7);
	public static int   D8 => Shared.Next(1,   9);
	public static int  D10 => Shared.Next(1,  11);
	public static int  D12 => Shared.Next(1,  13);
	public static int  D20 => Shared.Next(1,  21);
	public static int D100 => Shared.Next(1, 101);
}
