﻿using Microsoft.CodeAnalysis;
using SharpGen.Generator.Marshallers;
using SharpGen.Logging;
using SharpGen.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpGen.Generator
{
    public class MarshallingRegistry
    {
        private readonly Logger logger;

        public MarshallingRegistry(GlobalNamespaceProvider globalNamespace, Logger logger)
        {
            Marshallers = new List<IMarshaller>
            {
                new InterfaceArrayMarshaller(globalNamespace),
                new ArrayOfInterfaceMarshaller(globalNamespace),
                new BoolToIntArrayMarshaller(globalNamespace),
                new BoolToIntMarshaller(globalNamespace),
                new EnumMarshaller(globalNamespace),
                new InterfaceMarshaller(globalNamespace),
                new PointerSizeMarshaller(globalNamespace),
                new StringMarshaller(globalNamespace),
                new RemappedTypeMarshaller(globalNamespace),
                new StructWithNativeTypeMarshaller(globalNamespace),
                new StructWithNativeTypeArrayMarshaller(globalNamespace),
                new NullableInstanceMarshaller(globalNamespace),
                new BitfieldMarshaller(globalNamespace),
                new ValueTypeArrayFieldMarshaller(globalNamespace),
                new FallbackFieldMarshaller(globalNamespace),
                new ValueTypeArrayMarshaller(globalNamespace),
                new ValueTypeMarshaller(globalNamespace)
            };
            this.logger = logger;
        }

        private IReadOnlyList<IMarshaller> Marshallers { get; }

        public IMarshaller GetMarshaller(CsMarshalBase csElement)
        {
            var marshaller = Marshallers.FirstOrDefault(m => m.CanMarshal(csElement));
            if (marshaller == null)
            {
                if (csElement.PublicType is CsUndefinedType || csElement.MarshalType is CsUndefinedType)
                {
                    logger.Error(LoggingCodes.CannotMarshalUnknownType,
                        $"The element '{csElement}' has an unknown type and cannot be marshalled accurately. Maybe you used a <bind> directive and didn't use a <define> to define the type for SharpGen?");
                    logger.Exit("Unable to generate code with unknown marshal type.");
                }
                else
                {
                    throw new InvalidOperationException($"No marshaller found for {csElement}");
                }
            }
            return marshaller;
        }

        public SyntaxToken GetMarshalStorageLocationIdentifier(CsMarshalBase csElement)
        {
            return MarshallerBase.GetMarshalStorageLocationIdentifier(csElement);
        }
    }
}
