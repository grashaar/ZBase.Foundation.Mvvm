﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using ZBase.Foundation.SourceGen;

namespace ZBase.Foundation.Mvvm.GenericUnionSourceGen
{
    [Generator]
    public class GenericUnionGenerator : IIncrementalGenerator
    {
        public const string IUNION_T = "global::ZBase.Foundation.Mvvm.Unions.IUnion<";

        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var projectPathProvider = SourceGenHelpers.GetSourceGenConfigProvider(context);

            var candidateProvider = context.SyntaxProvider.CreateSyntaxProvider(
                predicate: static (node, token) => GeneratorHelpers.IsStructSyntaxMatch(node, token),
                transform: static (syntaxContext, token) => GetStructSemanticMatch(syntaxContext, token)
            ).Where(static t => t is { });

            var combined = candidateProvider.Collect()
                .Combine(context.CompilationProvider)
                .Combine(projectPathProvider);

            context.RegisterSourceOutput(combined, static (sourceProductionContext, source) => {
                GenerateOutput(
                    sourceProductionContext
                    , source.Left.Right
                    , source.Left.Left
                    , source.Right.projectPath
                    , source.Right.outputSourceGenFiles
                );
            });
        }

        public static StructRef GetStructSemanticMatch(
              GeneratorSyntaxContext context
            , CancellationToken token
        )
        {
            if (context.SemanticModel.Compilation.IsValidCompilation() == false
                || context.Node is not StructDeclarationSyntax structSyntax
                || structSyntax.BaseList == null
            )
            {
                return null;
            }

            var semanticModel = context.SemanticModel;

            foreach (var baseType in structSyntax.BaseList.Types)
            {
                var typeInfo = semanticModel.GetTypeInfo(baseType.Type, token);

                if (typeInfo.Type is INamedTypeSymbol interfaceSymbol)
                {
                    if (interfaceSymbol.IsGenericType
                       && interfaceSymbol.TypeParameters.Length == 1
                       && interfaceSymbol.ToFullName().StartsWith(IUNION_T)
                    )
                    {
                        return new StructRef {
                            Syntax = structSyntax,
                            TypeArgument = interfaceSymbol.TypeArguments[0],
                        };
                    }
                }

                if (TryGetMatchTypeArgument(typeInfo.Type.Interfaces, out var type)
                    || TryGetMatchTypeArgument(typeInfo.Type.AllInterfaces, out type)
                )
                {
                    return new StructRef {
                        Syntax = structSyntax,
                        TypeArgument = type,
                    };
                }
            }

            return null;

            static bool TryGetMatchTypeArgument(
                  ImmutableArray<INamedTypeSymbol> interfaces
                , out ITypeSymbol result
            )
            {
                foreach (var interfaceSymbol in interfaces)
                {
                    if (interfaceSymbol.IsGenericType
                        && interfaceSymbol.TypeParameters.Length == 1
                        && interfaceSymbol.ToFullName().StartsWith(IUNION_T)
                    )
                    {
                        result = interfaceSymbol.TypeArguments[0];
                        return true;
                    }
                }

                result = default;
                return false;
            }
        }

        private static void GenerateOutput(
              SourceProductionContext context
            , Compilation compilation
            , ImmutableArray<StructRef> candidates
            , string projectPath
            , bool outputSourceGenFiles
        )
        {
            if (candidates.Length < 1)
            {
                return;
            }

            context.CancellationToken.ThrowIfCancellationRequested();

            try
            {
                SourceGenHelpers.ProjectPath = projectPath;

                var declaration = new GenericUnionDeclaration(candidates, compilation, context.CancellationToken);

                declaration.GenerateUnionForValueTypes(
                      context
                    , compilation
                    , outputSourceGenFiles
                    , s_errorDescriptor
                );

                declaration.GenerateUnionForRefTypes(
                      context
                    , compilation
                    , outputSourceGenFiles
                    , s_errorDescriptor
                );

                declaration.GenerateStaticClass(
                      context
                    , compilation
                    , outputSourceGenFiles
                    , s_errorDescriptor
                );

                declaration.GenerateRedundantTypes(
                      context
                    , compilation
                    , outputSourceGenFiles
                    , s_errorDescriptor
                );
            }
            catch (Exception e)
            {
                context.ReportDiagnostic(Diagnostic.Create(
                      s_errorDescriptor
                    , null
                    , e.ToUnityPrintableString()
                ));
            }
        }

        private static readonly DiagnosticDescriptor s_errorDescriptor
            = new("SG_GENERIC_UNIONS_01"
                , "Generic Union Generator Error"
                , "This error indicates a bug in the Generic Union source generators. Error message: '{0}'."
                , "ZBase.Foundation.Mvvm.Unions.IUnion<T>"
                , DiagnosticSeverity.Error
                , isEnabledByDefault: true
                , description: ""
            );
    }
}
