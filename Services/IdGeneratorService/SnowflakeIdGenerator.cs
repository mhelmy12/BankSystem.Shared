using System;
using IdGen;

namespace Shared.Services.IdGeneratorService;

public class SnowflakeIdGenerator : IIdGeneratorService
{
    private readonly IIdGenerator<long> generator;

    public SnowflakeIdGenerator(IIdGenerator<long> Generator)
    {
        generator = Generator;
    }
    public string Generate()
    {
        var id = generator.CreateId();
        return id.ToString();
    }
}
