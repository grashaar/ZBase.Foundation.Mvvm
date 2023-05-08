﻿using System;
using ZBase.Foundation.Mvvm.Unions;

namespace ZBase.Foundation.Mvvm.ViewBinding.Adapters
{
    [Serializable]
    [Label("Object ⇒ String", "Default")]
    [Adapter(typeof(object), typeof(string))]
    public sealed class ObjectToStringAdapter : IAdapter
    {
        public Union Convert(in Union union)
        {
            if (union.TypeKind == UnionTypeKind.Object)
            {
                if (union.TryGetValue(out object result))
                {
                    return result.ToString();
                }
            }

            return union;
        }
    }
}
