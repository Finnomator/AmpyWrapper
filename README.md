# Ampy Wrapper

A c# implementation of my version of [ampy](https://github.com/Finnomator/ampy).

## Documentation

You can find the documentation [here](https://github.com/Finnomator/AmpyWrapper/wiki/AmpyWrapper.Ampy).

## Installation

```shell
dotnet add package AmpyWrapper --version 1.0.0
```

## Example

Replace `boardCOMPortNum` with the COM port number your board is connected to.

```csharp
using AmpyWrapper;

Ampy ampy = new(boardCOMPortNum);

AmpyOutput res = await ampy.ListDirectory();

Console.WriteLine("-------------OUTPUT-------------");
Console.WriteLine(res.Output);

if (res.Error != "") {
    Console.WriteLine("-------------ERROR-------------");
    Console.WriteLine(res.Error);
}
```
