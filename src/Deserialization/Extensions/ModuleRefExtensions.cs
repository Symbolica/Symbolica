using System.Collections.Generic;
using LLVMSharp.Interop;

namespace Symbolica.Deserialization.Extensions;

internal static class ModuleRefExtensions
{
    public static IEnumerable<LLVMValueRef> GetFunctions(this LLVMModuleRef self)
    {
        var function = self.FirstFunction;

        while (function != self.LastFunction)
        {
            yield return function;
            function = function.NextFunction;
        }

        yield return function;
    }

    public static IEnumerable<LLVMValueRef> GetGlobals(this LLVMModuleRef self)
    {
        var global = self.FirstGlobal;

        while (global != self.LastGlobal)
        {
            yield return global;
            global = global.NextGlobal;
        }

        yield return global;
    }
}
