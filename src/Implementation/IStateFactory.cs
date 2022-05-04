namespace Symbolica.Implementation;

public interface IStateFactory
{
    IExecutableState Create(IStatePool statePool);
}
