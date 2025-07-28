using DunGen;

Console.SetWindowSize(150, 50);
Console.SetBufferSize(150, 50);
Console.CursorVisible = false;

while (true)
{
	Map map = new(MapSize.Small);

	Console.Clear();

	map.Draw();

	Thread.Sleep(1000);
}

