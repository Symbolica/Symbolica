using System.Collections.Generic;
using System.Linq;
using Symbolica.Computation.Values.Constants;
using Symbolica.Expression;
using Xunit;

namespace Symbolica.Computation.Values.TestData;

internal sealed class UnaryTestData : TheoryData<IConstantValue, SymbolicUnsigned>
{
    public UnaryTestData()
    {
        foreach (var x in Values())
            Add(x, new SymbolicUnsigned(x));
    }

    public static IEnumerable<ConstantUnsigned> Values()
    {
        return Enumerable.Range(-8, 24).Select(v => ConstantUnsigned.Create((Bits) 4U, v));
    }
}
