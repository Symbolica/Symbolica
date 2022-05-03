using System.Linq;
using Symbolica.Abstraction;
using Symbolica.Expression;
using Symbolica.Representation.Types;

namespace Symbolica.Representation.Functions;

internal sealed class Symbolize : IFunction
{
    public Symbolize(FunctionId id, IParameters parameters)
    {
        Id = id;
        Parameters = parameters;
    }

    public FunctionId Id { get; }
    public IParameters Parameters { get; }

    public void Call(IState state, ICaller caller, IArguments arguments)
    {
        static Address BitCastAddress(IAddress address, Bytes size)
        {
            return Address.Create(new ArrayType(1U, new SingleValueType(size)), address.BaseAddress, address.Offsets);
        }

        var address = arguments.Get(0);
        var size = (Bytes) (uint) arguments.Get(1).GetSingleValue(state.Space);
        var name = state.ReadString(arguments.Get(2));
        address = address is IAddress a
            ? name switch
            {
                "addr" => BitCastAddress(a, (Bytes) 4),
                "wdata" => BitCastAddress(a, (Bytes) 4),
                "wstrb" => BitCastAddress(a, (Bytes) 1),
                "valid" => BitCastAddress(a, (Bytes) 1),
                _ => address
            }
            : address;

        state.Memory.Write(address, state.Space.CreateSymbolic(size.ToBits(), name));
    }
}
