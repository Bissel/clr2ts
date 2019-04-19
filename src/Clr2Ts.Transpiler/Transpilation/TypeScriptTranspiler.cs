﻿using Clr2Ts.Transpiler.Extensions;
using Clr2Ts.Transpiler.Transpilation.Templating;
using Clr2Ts.Transpiler.Transpilation.TypeReferenceTranslation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Clr2Ts.Transpiler.Transpilation.TypeScript
{
    /// <summary>
    /// Allows transpiling .NET type definitions to TypeScript source code.
    /// </summary>
    public sealed class TypeScriptTranspiler
    {
        private readonly ITypeReferenceTranslator _typeReferenceTranslator = new DefaultTypeReferenceTranslator();
        private readonly ITemplatingEngine _templatingEngine;
        private readonly IDocumentationSource _documentationSource;

        /// <summary>
        /// Creates a <see cref="TypeScriptTranspiler"/>.
        /// </summary>
        /// <param name="templatingEngine">Engine to use for loading templates.</param>
        /// <param name="documentationSource">Source for looking up documentation comments for members.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="templatingEngine"/> or <paramref name="documentationSource"/> is null.</exception>
        public TypeScriptTranspiler(ITemplatingEngine templatingEngine, IDocumentationSource documentationSource)
        {
            _templatingEngine = templatingEngine ?? throw new ArgumentNullException(nameof(templatingEngine));
            _documentationSource = documentationSource ?? throw new ArgumentNullException(nameof(documentationSource));
        }

        /// <summary>
        /// Transpiles the specified .NET type definitions to new source code.
        /// </summary>
        /// <param name="types">Types that should be transpiled.</param>
        /// <returns>The result of the transpilation process.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="types"/> is null.</exception>
        public TranspilationResult Transpile(IEnumerable<Type> types)
        {
            if (types == null) throw new ArgumentNullException(nameof(types));

            return new TranspilationResult(types.Select(GenerateClassDefinition));
        }

        private CodeFragment GenerateClassDefinition(Type type)
        {
            var code = _templatingEngine.UseTemplate("ClassDefinition", new Dictionary<string, string>
            {
                { "ClassDeclaration", type.Name },
                { "Documentation", GenerateDocumentationComment(type) },
                { "Properties", GeneratePropertyDefinitions(type, out var dependencies).AddIndentation() }
            });

            return new CodeFragment(
                CodeFragmentId.ForClrType(type),
                dependencies,
                code);
        }

        private string GeneratePropertyDefinitions(Type type, out IEnumerable<CodeFragmentId> dependencies)
        {
            var propertyCodeSnippets = new List<string>();
            var deps = new List<CodeFragmentId>();

            foreach(var property in type.GetProperties())
            {
                var typeReferenceTranslation = _typeReferenceTranslator.Translate(property.PropertyType);
                deps.AddRange(typeReferenceTranslation.Dependencies);
                propertyCodeSnippets.Add(_templatingEngine.UseTemplate("PropertyDefinition", new Dictionary<string, string>
                {
                    { "PropertyName", property.Name },
                    { "Documentation", GenerateDocumentationComment(property) },
                    { "PropertyType", typeReferenceTranslation.ReferencedTypeName }
                }));
            }

            dependencies = deps.Distinct().ToList();
            return string.Join(Environment.NewLine, propertyCodeSnippets);
        }
        
        private string GenerateDocumentationComment(MemberInfo member)
        {
            var documentation = _documentationSource.GetDocumentationText(member);
            if (string.IsNullOrWhiteSpace(documentation)) return null;

            return $@"/**
 * {documentation}
 */
";
        }
    }
}