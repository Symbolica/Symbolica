namespace Symbolica.Implementation.System;

internal interface IDescriptionFactory
{
    IPersistentDescription? Create(string path);
    IPersistentDescription CreateInput();
    IPersistentDescription CreateOutput();
    IPersistentDescription CreateInvalid();
}
