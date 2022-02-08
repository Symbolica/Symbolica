using System.Collections.Generic;
using System.Linq;
using Symbolica.Computation.Values.Constants;
using Xunit;

namespace Symbolica.Computation.Values.TestData;

internal sealed class DoublePowerTestData : TheoryData<
    IConstantValue, IConstantValue,
    SymbolicFloat, SymbolicFloat>
{
    public DoublePowerTestData()
    {
        foreach (var left in Values())
        foreach (var right in Values())
            Add(new ConstantDouble(left), new ConstantDouble(right),
                SymbolicFloat.Create(left), SymbolicFloat.Create(right));
    }

    private static IEnumerable<double> Values()
    {
        return Enumerable.Range(1, 10).Select(i => (double) i);
    }
}
