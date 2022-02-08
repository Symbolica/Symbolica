using System.Collections.Generic;
using System.Linq;
using Symbolica.Computation.Values.Constants;
using Xunit;

namespace Symbolica.Computation.Values.TestData;

internal sealed class DoubleUnaryTestData : TheoryData<
    IConstantValue,
    SymbolicFloat>
{
    public DoubleUnaryTestData()
    {
        foreach (var value in Values())
            Add(new ConstantDouble(value),
                SymbolicFloat.Create(value));
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
