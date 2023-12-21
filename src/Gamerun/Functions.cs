namespace Gamerun;

public static class Functions
{
    public static IEnumerable<int> ParseCoresSpecification(string? coreSpecification)
    {
        if (string.IsNullOrWhiteSpace(coreSpecification))
            yield break;

        if (coreSpecification == "none")
        {
            foreach (var core in Sysfs.GetCpuCores())
                yield return core;
            
            yield break;
        }
    
        var coreStrings = coreSpecification
            .Split(",", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        foreach (var coreString in coreStrings)
        {
            var coreNumbers = coreString
                .Split('-', 2, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse)
                .ToArray();

            if (coreNumbers.Length == 1)
                yield return coreNumbers[0];
            else
            {
                foreach (var core in Enumerable.Range(coreNumbers[0], coreNumbers[1] - coreNumbers[0] + 1))
                    yield return core;
            }
        }
    }
}