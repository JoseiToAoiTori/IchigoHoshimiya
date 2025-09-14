using IchigoHoshimiya.Interfaces;

namespace IchigoHoshimiya.Services;

public class ChooseService : IChooseService
{
    private readonly Random _random = new();

    public string GetRandomChoice(string[] choices)
    {
        var index = _random.Next(choices.Length);

        return choices[index];
    }
}