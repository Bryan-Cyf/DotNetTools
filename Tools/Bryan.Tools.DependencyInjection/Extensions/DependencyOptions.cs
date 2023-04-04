using System;
using System.Collections.Generic;
using System.Text;

namespace Tools.Dependency
{

    /// <summary>
    /// options.
    /// </summary>
    public class DependencyOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:EasyCaching.Core.EasyCachingOptions"/> class.
        /// </summary>
        public DependencyOptions()
        {
            Extensions = new List<IDependencyOptionsExtension>();
        }

        /// <summary>
        /// Gets the extensions.
        /// </summary>
        /// <value>The extensions.</value>
        public IList<IDependencyOptionsExtension> Extensions { get; }

        /// <summary>
        /// Registers the extension.
        /// </summary>
        /// <param name="extension">Extension.</param>
        public void RegisterExtension(IDependencyOptionsExtension extension)
        {
            Extensions.Add(extension);
        }
    }
}
