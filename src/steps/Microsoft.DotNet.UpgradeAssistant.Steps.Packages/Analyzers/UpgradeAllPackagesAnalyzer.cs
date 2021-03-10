// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NuGet.Versioning;

namespace Microsoft.DotNet.UpgradeAssistant.Steps.Packages.Analyzers
{
    public class UpgradeAllPackagesAnalyzer : IPackageReferencesAnalyzer
    {
        private readonly IPackageLoader _packageLoader;
        private readonly ILogger<UpgradeAllPackagesAnalyzer> _logger;
        private readonly string? _analyzerPackageVersion;

        public string Name => "Upgrade assistant reference analyzer";

        public UpgradeAllPackagesAnalyzer(IOptions<PackageUpdaterOptions> updaterOptions, IPackageLoader packageLoader, ILogger<UpgradeAllPackagesAnalyzer> logger)
        {
            if (updaterOptions is null)
            {
                throw new ArgumentNullException(nameof(updaterOptions));
            }

            _packageLoader = packageLoader ?? throw new ArgumentNullException(nameof(packageLoader));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _analyzerPackageVersion = updaterOptions.Value.UpgradeAnalyzersPackageVersion;
        }

        public async Task<PackageAnalysisState> AnalyzeAsync(IProject project, PackageAnalysisState state, CancellationToken token)
        {
            // project.RanToCompletionOnce = true;

            if (state is null)
            {
                throw new ArgumentNullException(nameof(state));
            }

            var packageReferences = project.Required().PackageReferences.Where(r => !state.PackagesToRemove.Contains(r));

            foreach (var pkg in packageReferences)
            {
                var old = new NuGetReference(pkg.Name, pkg.Version);

                var analyzerPackage = _analyzerPackageVersion is not null
                    ? new NuGetReference(pkg.Name, _analyzerPackageVersion)
                    : await _packageLoader.GetLatestVersionAsync(pkg.Name, false, null, token).ConfigureAwait(false);

                if (analyzerPackage is not null)
                {
                    if (old == analyzerPackage)
                    {
                        _logger.LogInformation("Package {name} [{version}] is latest", pkg.Name, pkg.Version);
                        continue;
                    }

                    _logger.LogInformation("Package {name} [{version}] needs upgraded to [{latest}]", pkg.Name, pkg.Version, analyzerPackage.Version);
                    state.PackagesToRemove.Add(old);
                    state.PackagesToAdd.Add(analyzerPackage);
                }
            }

            return state;
        }
    }
}