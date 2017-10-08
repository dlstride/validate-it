using System.Collections.Generic;

namespace ValidationServer.Bootstrap
{
    public interface IValidatorModuleConfig
    {
        List<string> ModuleNames { get; }
    }
}