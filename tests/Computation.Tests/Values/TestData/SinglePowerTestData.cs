using System.Collections.Generic;
using System.Linq;
using Symbolica.Computation.Values.Constants;
using Xunit;

namespace Symbolica.Computation.Values.TestData;

internal sealed class SinglePowerTestData : TheoryData<
    IConstantValue, IConstantValue,
    SymbolicFloat, SymbolicFloat>
{
    public SinglePowerTestData()
    {
        foreach (var left in Values())
        foreach (var right in Values())
            Add(new ConstantSingle(left), new ConstantSingle(right),
                SymbolicFloat.Create(left), SymbolicFloat.Create(right));
    }

    private static IEnumerable<float> Values()
    {
        return Enumerable.Range(1, 5).Select(i => (float) i);
    }
}
