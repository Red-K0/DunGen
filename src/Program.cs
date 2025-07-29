using DunGen.Values;
using DunGen.Generation;

Console.SetWindowSize(150, 50);
Console.SetBufferSize(150, 50);
Console.CursorVisible = false;

Map map = new(MapSize.Large);

Console.Clear();

map.Draw();

Thread.Sleep(-1);