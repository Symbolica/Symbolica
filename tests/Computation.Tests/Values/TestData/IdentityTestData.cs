using System.Collections.Generic;
using System.Linq;
using Symbolica.Computation.Values.Constants;
using Symbolica.Expression;
using Xunit;

namespace Symbolica.Computation.Values.TestData;

internal sealed class IdentityTestData : TheoryData<IValue>
{
    public IdentityTestData()
    {
        foreach (var value in Values())
        {
            Add(value);
            Add(new SymbolicUnsigned(value));
        }
    }

    private static IEnumerable<ConstantUnsigned> Values()
    {
        return Enumerable.Range(-8, 24).Select(v => ConstantUnsigned.Create(Size.FromBits(4U), v));
    }
}
