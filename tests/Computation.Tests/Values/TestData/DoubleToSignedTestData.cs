using System.Collections.Generic;
using System.Linq;
using Symbolica.Computation.Values.Constants;
using Symbolica.Expression;
using Xunit;

namespace Symbolica.Computation.Values.TestData;

internal sealed class DoubleToSignedTestData : TheoryData<Bits,
    IConstantValue,
    SymbolicFloat>
{
    public DoubleToSignedTestData()
    {
        foreach (var size in Sizes())
        foreach (var value in Values())
            Add(size,
                new ConstantDouble(value),
                SymbolicFloat.Create(value));
    }

    private static IEnumerable<Bits> Sizes()
    {
        yield return (Bits) 16U;
        yield return (Bits) 32U;
        yield return (Bits) 64U;
        yield return (Bits) 80U;
        yield return (Bits) 128U;
    }

    private static IEnumerable<double> Values()
    {
        yield return 0d;
        yield return double.Epsilon;

        foreach (var i in Enumerable.Range(-10, 10))
        {
            yield return i / 3d;
            yield return 3d / i;
        }

        foreach (var i in Enumerable.Range(1, 10))
        {
            yield return i / 3d;
            yield return 3d / i;
        }
    }
}
