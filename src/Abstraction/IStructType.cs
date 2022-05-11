﻿using System;
using Symbolica.Expression;

namespace Symbolica.Abstraction;

public interface IStructType : IType
{
    IStruct CreateStruct(IExpressionFactory exprFactory, Func<Bits, IExpression> initializer);
}
