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
            var coreIds = coreString
                .Split('-', 2, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse)
                .ToArray();

            if (coreIds.Length == 1)
                yield return coreIds[0];
            else
            {
                foreach (var core in Enumerable.Range(coreIds[0], coreIds[1] - coreIds[0] + 1))
                    yield return core;
            }
        }
    }
}