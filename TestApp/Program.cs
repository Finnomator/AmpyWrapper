using AmpyWrapper;

Ampy ampy = new(9);

AmpyOutput res = await ampy.ListDirectory();

Console.WriteLine("-------------OUTPUT-------------");
Console.WriteLine(res.Output);

if (res.Error != "") {
    Console.WriteLine("-------------ERROR-------------");
    Console.WriteLine(res.Error);
}
