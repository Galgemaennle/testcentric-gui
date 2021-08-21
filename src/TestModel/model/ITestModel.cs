// ***********************************************************************
// Copyright (c) Charlie Poole and TestCentric GUI contributors.
// Licensed under the MIT License. See LICENSE file in root directory.
// ***********************************************************************

using System;
using System.Collections.Generic;
using TestPackage = NUnit.Engine.TestPackage;

namespace TestCentric.Gui.Model
{
    using TestCentric.Engine;
    using Services;
    using Settings;

    public interface ITestModel : IDisposable
    {
        #region General Properties

        // Work Directory
        string WorkDirectory { get; }

        // Event Dispatcher
        ITestEvents Events { get; }

        ITestServices Services { get; }

        UserSettings Settings { get; }

        RecentFiles RecentFiles { get; }

        // Project Support
        bool NUnitProjectSupport { get; }
        bool VisualStudioSupport { get; }

        // List of available runtimes, based on the engine's list
        // but filtered to meet the GUI's requirements
        IList<NUnit.Engine.IRuntimeFramework> AvailableRuntimes { get; }

        IList<TestAgentInfo> AvailableAgents { get; }

        // Result Format Support
        IEnumerable<string> ResultFormats { get; }

        #endregion

        #region Current State of the Model

        TestPackage TestPackage { get; }

        bool IsPackageLoaded { get; }

        List<string> TestFiles { get; }


        IDictionary<string, object> PackageOverrides { get; }

        // TestNode hierarchy representing the discovered tests
        TestNode Tests { get; }

        // See if tests are available
        bool HasTests { get; }

        IList<string> AvailableCategories { get; }

        // See if a test is running
        bool IsTestRunning { get; }

        // Do we have results from running the test?
        bool HasResults { get; }

        ITestItem SelectedTestItem { get; }
        List<string> SelectedCategories { get; }

        bool ExcludeSelectedCategories { get; }

        TestFilter CategoryFilter { get; }

        #endregion

        #region Methods

        // Create a new empty project using a default name
        void NewProject();

        // Create a new project given a filename
        void NewProject(string filename);

        void SaveProject();

        // Load a TestPackage
        void LoadTests(IList<string> files);

        // Unload current TestPackage
        void UnloadTests();

        // Reload current TestPackage
        void ReloadTests();

        // Reload a specific package using the specified config
        void ReloadPackage(TestPackage package, string config);

        // Run all the tests
        void RunAllTests();

        // Debug all tests
        void DebugAllTests();

        // Run selected tests
        void RunSelectedTests();

        // Debug selected tests
        void DebugSelectedTests();

        // Run just the specified ITestItem
        void RunTests(ITestItem testItem);

        // Debug just the specified ITestItem
        void DebugTests(ITestItem testItem);

        // Stop the running test
        void StopTestRun(bool force);

        // Save the results of the last run in the specified format
        void SaveResults(string fileName, string format="nunit3");

        // Get the result for a test if available
        ResultNode GetResultForTest(string id);

        // Get the TestPackage represented by a test,if available
        TestPackage GetPackageForTest(string id);
        IDictionary<string, object> GetPackageSettingsForTest(string id);

        // Get Agents available for a package
        IList<TestAgentInfo> GetAvailableAgents(TestPackage package);

        // Clear the results for all tests
        void ClearResults();

        // Broadcast event when SelectedTestItem changes
        void NotifySelectedItemChanged(ITestItem testItem);

        // Set the category filters for running and tree display
        void SelectCategories(IList<string> categories, bool exclude);

        #endregion
    }
}
