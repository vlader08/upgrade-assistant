﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Microsoft.DotNet.UpgradeAssistant.Steps.Solution
{
    public class EntrypointSelectionStep : UpgradeStep
    {
        private readonly IPackageRestorer _restorer;
        private readonly IUserInput _userInput;
        private readonly bool _isNonInteractiveMode;
        private readonly string _entryPoint;

        public EntrypointSelectionStep(
            UpgradeOptions options,
            IPackageRestorer restorer,
            IUserInput userInput,
            ILogger<EntrypointSelectionStep> logger)
            : base(logger)
        {
            _isNonInteractiveMode = options?.NonInteractive ?? throw new ArgumentNullException(nameof(options));
            _entryPoint = options?.EntryPoint ?? string.Empty;
            _restorer = restorer ?? throw new ArgumentNullException(nameof(restorer));
            _userInput = userInput ?? throw new ArgumentNullException(nameof(userInput));
        }

        public override string Title => "Select an entrypoint";

        public override string Description => "The entrypoint is the application you run or the library that is to be upgraded. Dependencies will then be analyzed and a recommended process will then be determined";

        protected override async Task<UpgradeStepApplyResult> ApplyImplAsync(IUpgradeContext context, CancellationToken token)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var selectedProject = await GetEntrypointAsync(context, token).ConfigureAwait(false);

            if (selectedProject is null)
            {
                return new UpgradeStepApplyResult(UpgradeStepStatus.Failed, "No project was selected.");
            }
            else
            {
                context.SetEntryPoint(selectedProject);
                await _restorer.RestorePackagesAsync(context, selectedProject, token).ConfigureAwait(false);

                return new UpgradeStepApplyResult(UpgradeStepStatus.Complete, $"Project {selectedProject.GetRoslynProject().Name} was selected.");
            }
        }

        protected override Task<UpgradeStepInitializeResult> InitializeImplAsync(IUpgradeContext context, CancellationToken token)
            => Task.FromResult(InitializeImpl(context));

        protected UpgradeStepInitializeResult InitializeImpl(IUpgradeContext context)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.EntryPoint is not null)
            {
                return new UpgradeStepInitializeResult(UpgradeStepStatus.Complete, "Project is already selected.", BuildBreakRisk.None);
            }

            var projects = context.Projects.ToList();

            if (projects.Count == 1)
            {
                var project = projects[0];
                context.SetEntryPoint(project);

                Logger.LogInformation("Setting entrypoint to only project in solution: {Project}", project.FilePath);

                return new UpgradeStepInitializeResult(UpgradeStepStatus.Complete, "Selected only project.", BuildBreakRisk.None);
            }

            // If the user has specified a particular project, that project will be considered the entry point even if
            // other dependencies were loaded along with it.
            if (!context.InputIsSolution)
            {
                var project = projects.First(i => Path.GetFileName(i.FilePath).Equals(Path.GetFileName(context.InputPath), StringComparison.OrdinalIgnoreCase));
                context.SetEntryPoint(project);

                Logger.LogInformation("Setting entrypoint to user selected project: {Project}", project.FilePath);

                return new UpgradeStepInitializeResult(UpgradeStepStatus.Complete, "Selected user's choice of entry point project.", BuildBreakRisk.None);
            }

            if (!string.IsNullOrEmpty(_entryPoint))
            {
                var entryPointProject = projects.FirstOrDefault(i => Path.GetFileName(i.FilePath).Equals(_entryPoint, StringComparison.OrdinalIgnoreCase));
                if (entryPointProject is not null)
                {
                    context.SetEntryPoint(entryPointProject);
                    Logger.LogInformation("Setting entrypoint to user selected project in solution: {Project}", _entryPoint);

                    return new UpgradeStepInitializeResult(UpgradeStepStatus.Complete, "Selected user's choice of entry point project.", BuildBreakRisk.None);
                }
                else
                {
                    Logger.LogInformation("Invalid Entry-Point Project specified : {Project}", _entryPoint);
                }
            }

#pragma warning disable CA1508 // Avoid dead conditional code
            if (context.EntryPoint is null && _isNonInteractiveMode)
#pragma warning restore CA1508 // Avoid dead conditional code
            {
                throw new UpgradeException("Entry Point needs to be provided when more than 1 projects present in a non-interative mode. There are 2 ways of providing an entry-point :\n" +
                    "a) Execute upgrade-assistant in interactive mode up until select entry point step and re-run upgrade-assistant in non-interactive mode \n" +
                    "b) Execute upgrade-assistant in non-interactive mode with valid value for --entry-point");
            }

            return new UpgradeStepInitializeResult(UpgradeStepStatus.Incomplete, "No entryproint is selected.", BuildBreakRisk.None);
        }

        private async ValueTask<IProject> GetEntrypointAsync(IUpgradeContext context, CancellationToken token)
        {
            const string EntrypointQuestion = "Please select the project you run. We will then analyze the dependencies and identify the recommended order to upgrade projects.";

            if (context.EntryPoint is not null)
            {
                return context.EntryPoint;
            }

            var allProjects = context.Projects.OrderBy(p => p.GetRoslynProject().Name).Select(ProjectCommand.Create).ToList();
            var result = await _userInput.ChooseAsync(EntrypointQuestion, allProjects, token).ConfigureAwait(false);

            return result.Project;
        }

        protected override bool IsApplicableImpl(IUpgradeContext context)
            => context is not null && context.EntryPoint is null && !context.IsComplete;
    }
}
