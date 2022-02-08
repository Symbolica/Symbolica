using System.Collections.Generic;
using System.Linq;
using Symbolica.Computation.Values.Constants;
using Xunit;

namespace Symbolica.Computation.Values.TestData;

internal sealed class DoubleBinaryTestData : TheoryData<
    IConstantValue, IConstantValue,
    SymbolicFloat, SymbolicFloat>
{
    public DoubleBinaryTestData()
    {
        foreach (var left in Values())
        foreach (var right in Values())
            Add(new ConstantDouble(left), new ConstantDouble(right),
                SymbolicFloat.Create(left), SymbolicFloat.Create(right));
    }

    private static IEnumerable<double> Values()
    {
        yield return 0d;
        yield return double.Epsilon;
        yield return double.NaN;
        yield return double.NegativeInfinity;
        yield return double.MinValue;
        yield return double.MaxValue;
        yield return double.PositiveInfinity;

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
