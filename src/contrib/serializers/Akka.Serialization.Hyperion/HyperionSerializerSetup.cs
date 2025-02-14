﻿//-----------------------------------------------------------------------
// <copyright file="HyperionSerializerSetup.cs" company="Akka.NET Project">
//     Copyright (C) 2009-2023 Lightbend Inc. <http://www.lightbend.com>
//     Copyright (C) 2013-2023 .NET Foundation <https://github.com/akkadotnet/akka.net>
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Akka.Actor;
using Akka.Actor.Setup;
using Akka.Util;
using Hyperion;

namespace Akka.Serialization.Hyperion
{
    public class HyperionSerializerSetup : Setup
    {
        public static readonly HyperionSerializerSetup Empty =
            new HyperionSerializerSetup(null, null, null, null, null, null, null);

        public static HyperionSerializerSetup Create(
            bool preserveObjectReferences, 
            bool versionTolerance, 
            Type knownTypesProvider)
            => new HyperionSerializerSetup(preserveObjectReferences, versionTolerance, knownTypesProvider, null, null, null, DisabledTypeFilter.Instance);

        public static HyperionSerializerSetup Create(
            bool preserveObjectReferences, 
            bool versionTolerance, 
            Type knownTypesProvider, 
            IEnumerable<Func<string, string>> packageNameOverrides)
            => new HyperionSerializerSetup(preserveObjectReferences, versionTolerance, knownTypesProvider, packageNameOverrides, null, null, DisabledTypeFilter.Instance);

        public static HyperionSerializerSetup Create(
            bool preserveObjectReferences, 
            bool versionTolerance, 
            Type knownTypesProvider, 
            IEnumerable<Func<string, string>> packageNameOverrides,
            IEnumerable<Surrogate> surrogates)
            => new HyperionSerializerSetup(preserveObjectReferences, versionTolerance, knownTypesProvider, packageNameOverrides, surrogates, null, DisabledTypeFilter.Instance);
        
        public static HyperionSerializerSetup Create(
            bool preserveObjectReferences, 
            bool versionTolerance, 
            Type knownTypesProvider, 
            IEnumerable<Func<string, string>> packageNameOverrides,
            IEnumerable<Surrogate> surrogates,
            bool disallowUnsafeType)
            => new HyperionSerializerSetup(preserveObjectReferences, versionTolerance, knownTypesProvider, packageNameOverrides, surrogates, disallowUnsafeType, DisabledTypeFilter.Instance);

        public static HyperionSerializerSetup Create(
            bool preserveObjectReferences, 
            bool versionTolerance, 
            Type knownTypesProvider, 
            IEnumerable<Func<string, string>> packageNameOverrides,
            IEnumerable<Surrogate> surrogates,
            bool disallowUnsafeType,
            ITypeFilter typeFilter)
            => new HyperionSerializerSetup(preserveObjectReferences, versionTolerance, knownTypesProvider, packageNameOverrides, surrogates, disallowUnsafeType, typeFilter);
        
        private HyperionSerializerSetup(
            bool? preserveObjectReferences, 
            bool? versionTolerance, 
            Type knownTypesProvider, 
            IEnumerable<Func<string, string>> packageNameOverrides,
            IEnumerable<Surrogate> surrogates,
            bool? disallowUnsafeType,
            ITypeFilter typeFilter)
        {
            PreserveObjectReferences = preserveObjectReferences;
            VersionTolerance = versionTolerance;
            KnownTypesProvider = knownTypesProvider;
            PackageNameOverrides = packageNameOverrides;
            Surrogates = surrogates;
            DisallowUnsafeType = disallowUnsafeType;
            TypeFilter = typeFilter;
        }

        /// <summary>
        /// When true, it tells <see cref="HyperionSerializer"/> to keep
        /// track of references in serialized/deserialized object graph.
        /// </summary>
        public bool? PreserveObjectReferences { get; }

        /// <summary>
        /// When true, it tells <see cref="HyperionSerializer"/> to encode
        /// a list of currently serialized fields into type manifest.
        /// </summary>
        public bool? VersionTolerance { get; }

        /// <summary>
        /// A type implementing <see cref="IKnownTypesProvider"/>, that will
        /// be used when <see cref="HyperionSerializer"/> is being constructed
        /// to provide a list of message types that are supposed to be known
        /// implicitly by all communicating parties. Implementing class must
        /// provide either a default constructor or a constructor taking
        /// <see cref="ExtendedActorSystem"/> as its only parameter.
        /// </summary>
        public Type KnownTypesProvider { get; }

        /// <summary>
        /// A list of lambda functions, used to transform incoming deserialized
        /// package names before they are instantiated.
        /// Used to provide cross-platform compatibility.
        /// </summary>
        public IEnumerable<Func<string, string>> PackageNameOverrides { get; }
        
        /// <summary>
        /// A list of Surrogate instances that are used to de/serialize complex objects
        /// into a much simpler serialized objects.
        /// </summary>
        public IEnumerable<Surrogate> Surrogates { get; }
        
        /// <summary>
        /// If set, will cause the Hyperion serializer to block potentially dangerous and unsafe types
        /// from being deserialized during run-time. Defaults to true.
        /// </summary>
        public bool? DisallowUnsafeType { get; }
        
        public ITypeFilter TypeFilter { get; }

        internal HyperionSerializerSettings ApplySettings(HyperionSerializerSettings settings)
        {
            if(PreserveObjectReferences is { })
                settings = settings.WithPreserveObjectReference(PreserveObjectReferences.Value);

            if (VersionTolerance is { })
                settings = settings.WithVersionTolerance(VersionTolerance.Value);

            if (KnownTypesProvider is { })
                settings = settings.WithKnownTypesProvider(KnownTypesProvider);

            if (PackageNameOverrides is { })
                settings = settings.WithPackageNameOverrides(PackageNameOverrides);

            if (Surrogates is { })
                settings = settings.WithSurrogates(Surrogates);

            if (DisallowUnsafeType is { })
                settings = settings.WithDisallowUnsafeType(DisallowUnsafeType.Value);

            if (TypeFilter is { })
                settings = settings.WithTypeFilter(TypeFilter);
            
            return settings;
        }

        public HyperionSerializerSetup WithPreserveObjectReference(bool preserveObjectReference)
            => Copy(preserveObjectReferences: preserveObjectReference);

        public HyperionSerializerSetup WithVersionTolerance(bool versionTolerance)
            => Copy(versionTolerance: versionTolerance);

        public HyperionSerializerSetup WithKnownTypeProvider<T>()
            => Copy(knownTypesProvider: typeof(T));

        public HyperionSerializerSetup WithKnownTypeProvider(Type knownTypeProvider)
            => Copy(knownTypesProvider: knownTypeProvider);

        public HyperionSerializerSetup WithPackageNameOverrides(IEnumerable<Func<string, string>> packageNameOverrides)
            => Copy(packageNameOverrides: packageNameOverrides);

        public HyperionSerializerSetup WithSurrogates(IEnumerable<Surrogate> surrogates)
            => Copy(surrogates: surrogates);

        public HyperionSerializerSetup WithDisallowUnsafeType(bool disallowUnsafeType)
            => Copy(disallowUnsafeType: disallowUnsafeType);

        public HyperionSerializerSetup WithTypeFilter(ITypeFilter typeFilter)
            => Copy(typeFilter: typeFilter);
        
        private HyperionSerializerSetup Copy(
            bool? preserveObjectReferences = null,
            bool? versionTolerance = null,
            Type knownTypesProvider = null,
            IEnumerable<Func<string, string>> packageNameOverrides = null,
            IEnumerable<Surrogate> surrogates = null,
            bool? disallowUnsafeType = null,
            ITypeFilter typeFilter = null
            )
            => new HyperionSerializerSetup(
                preserveObjectReferences ?? PreserveObjectReferences,
                versionTolerance ?? VersionTolerance,
                knownTypesProvider ?? KnownTypesProvider,
                packageNameOverrides ?? PackageNameOverrides,
                surrogates ?? Surrogates,
                disallowUnsafeType ?? DisallowUnsafeType,
                typeFilter ?? TypeFilter);
    }
}
