﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Clr2Ts.Transpiler.Transpilation
{
    /// <summary>
    /// Represents the result of a transpilation process.
    /// </summary>
    public sealed class TranspilationResult
    {
        /// <summary>
        /// Creates a <see cref="TranspilationResult"/>.
        /// </summary>
        /// <param name="codeFragments">Code fragments that were generated by the transpilation process.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="codeFragments"/> is null.</exception>
        public TranspilationResult(IEnumerable<CodeFragment> codeFragments)
        {
            if (codeFragments == null) throw new ArgumentNullException(nameof(codeFragments));

            CodeFragments = codeFragments.ToList();
        }

        /// <summary>
        /// Gets the code fragments that were generated by the transpilation process.
        /// </summary>
        public IEnumerable<CodeFragment> CodeFragments { get; }
    }
}