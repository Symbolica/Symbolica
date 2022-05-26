﻿using System;
using System.Collections.Generic;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Implementation.System;

internal sealed class InputDescription : IPersistentDescription
{
    private readonly IExpressionFactory _exprFactory;

    public InputDescription(IExpressionFactory exprFactory)
    {
        _exprFactory = exprFactory;
    }

    public (long, IPersistentDescription) Seek(long offset, uint whence)
    {
        return (-1L, this);
    }

    public int Read(ISpace space, IMemory memory, IExpression address, int count)
    {
        return -1;
    }

    public IExpression ReadDirectory(ISpace space, IMemory memory, IStruct entry, IExpression address, int tell)
    {
        return _exprFactory.CreateZero(_exprFactory.PointerSize);
    }

    public int GetStatus(ISpace space, IMemory memory, IStruct stat, IExpression address)
    {
        var type = Convert.ToInt32("0020000", 8);
        var mode = Convert.ToInt32("00444", 8);

        memory.Write(space, address, stat
            .Write(space, 3, type | mode)
            .Write(space, 8, 0L)
            .Expression);

        return 0;
    }

    public (HashSet<ExpressionSubs> subs, bool) IsEquivalentTo(IPersistentDescription other)
    {
        return (new(), other is InputDescription);
    }

    public object ToJson()
    {
        return GetType().Name;
    }

    public int GetEquivalencyHash(bool includeSubs)
    {
        return GetType().Name.GetHashCode();
    }
}
