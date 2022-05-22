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

        IList<string> AvailableAgents { get; }

        // Result Format Support
        IEnumerable<string> ResultFormats { get; }

        #endregion

        #region Current State of the Model

        TestPackage TestPackage { get; }

        bool IsPackageLoaded { get; }

        List<string> TestFiles { get; }


        IDictionary<string, object> PackageOverrides { get; }

        // TestNode hierarchy representing the discovered tests
        TestNode LoadedTests { get; }

        // See if tests are available
        bool HasTests { get; }

        IList<string> AvailableCategories { get; }

        // See if a test is running
        bool IsTestRunning { get; }

        // Summary of last test run
        ResultSummary ResultSummary { get; }

        // Is Resultsummary available?
        bool HasResults { get; }

        /// <summary>
        /// Gets or sets the active test item. This is the item
        /// for which details are displayed in the various views.
        /// </summary>
        ITestItem ActiveTestItem { get; set; }

        /// <summary>
        ///  Gets or sets the list of selected tests.
        /// </summary>
        TestSelection SelectedTests { get; set; }

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

        // Loading and Unloading Tests
        void LoadTests(IList<string> files);
        void UnloadTests();
        void ReloadTests();
        void ReloadPackage(TestPackage package, string config);

        // Running Tests
        void RunAllTests();

        // Run selected tests
        void RunSelectedTests();

        // Repeat Last Test Run
        void RerunTests();
        void RunFailedTests();
        void RunTests(ITestItem testItem);
        void DebugTests(ITestItem testItem);
        void StopTestRun(bool force);

        // Save the results of the last run in the specified format
        void SaveResults(string fileName, string format="nunit3");

        // Get a specific test given its id
        TestNode GetTestById(string id);

        // Get the result for a specific test id if available
        ResultNode GetResultForTest(string id);

        // Get the TestPackage represented by a test,if available
        TestPackage GetPackageForTest(string id);
        IDictionary<string, object> GetPackageSettingsForTest(string id);

        // Get Agents available for a package
        IList<string> GetAgentsForPackage(TestPackage package);

        // Clear the results for all tests
        void ClearResults();

        // Set the category filters for running and tree display
        void SelectCategories(IList<string> categories, bool exclude);

        #endregion
    }
}
