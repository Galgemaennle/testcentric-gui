// ***********************************************************************
// Copyright (c) Charlie Poole and TestCentric contributors.
// Licensed under the MIT License. See LICENSE file in root directory.
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace TestCentric.Gui.Presenters
{
    using System.Diagnostics;
    using System.Drawing;
    using System.Windows.Forms;
    using Model;
    using Views;

    public class TestPropertiesPresenter
    {
        private readonly ITestPropertiesView _view;
        private readonly ITestModel _model;

        private ITestItem _selectedItem;

        public TestPropertiesPresenter(ITestPropertiesView view, ITestModel model)
        {
            _view = view;
            _model = model;

            _view.Visible = false;

            WireUpEvents();
        }

        private void WireUpEvents()
        {
            _model.Events.TestLoaded += (ea) => _view.Visible = true;
            _model.Events.TestReloaded += (ea) => _view.Visible = true;
            _model.Events.TestUnloaded += (ea) => _view.Visible = false;
            _model.Events.RunFinished += (ea) => DisplaySelectedItem();
            _model.Events.SelectedItemChanged += (ea) => OnSelectedItemChanged(ea.TestItem);
            _view.DisplayHiddenPropertiesChanged += () => DisplaySelectedItem();
        }

        private void OnSelectedItemChanged(ITestItem testItem)
        {
            _selectedItem = testItem;
            DisplaySelectedItem();
        }

        private void DisplaySelectedItem()
        {
            TestNode testNode = _selectedItem as TestNode;
            ResultNode resultNode = null;

            // TODO: Insert checks for errors in the XML
            if (_selectedItem != null)
            {
                _view.Header = _selectedItem.Name;

                if (testNode != null)
                {
                    //_view.SuspendLayout();

                    var packageSettings = _model.GetPackageSettingsForTest(testNode.Id);
                    if (packageSettings != null)
                        DisplayPackageSettingsPanel(packageSettings);
                    else
                        HidePackagePanel();

                    DisplayTestPanel(testNode);

                    resultNode = _model.GetResultForTest(testNode.Id);
                    if (resultNode != null)
                        DisplayResultPanel(resultNode);
                    else
                        HideResultPanel();

                    //_view.ResumeLayout();
                }
            }

            // HACK: results won't display on Linux otherwise
            //if (Path.DirectorySeparatorChar == '/') // Running on Linux or Unix
            //    _view.ResultPanelVisible = true;
            //else
            //    _view.ResultPanelVisible = resultNode != null;

            // TODO: We should actually try to set the font for bold items
            // dynamically, since the global application font may be changed.
        }

        private void DisplayPackageSettingsPanel(IDictionary<string, object> settings)
        {
            var sb = new StringBuilder();
            foreach (var key in settings.Keys)
            {
                if (sb.Length > 0)
                    sb.Append(Environment.NewLine);
                sb.Append($"{key} = {settings[key]}");
            }

            _view.PackageSettings = sb.ToString();

            _view.ShowPackagePanel();
        }

        private void HidePackagePanel()
        {
            _view.HidePackagePanel();
        }

        private void DisplayTestPanel(TestNode testNode)
        {
            _view.TestType = GetTestType(testNode);
            _view.FullName = testNode.FullName;
            _view.Description = testNode.GetProperty("Description");
            _view.Categories = testNode.GetPropertyList("Category");
            _view.TestCount = testNode.TestCount.ToString();
            _view.RunState = testNode.RunState.ToString();
            
            StringBuilder reason = new StringBuilder(testNode.GetProperty("_SKIPREASON"));
            
            string message = testNode.Xml.SelectSingleNode("failure/message")?.InnerText;
            if (!string.IsNullOrEmpty(message))
            {
                if (reason.Length > 0)
                    reason.Append("\r\n");
                reason.Append(message);
                string stackTrace = testNode.Xml.SelectSingleNode("failure/stack-trace")?.InnerText;
                if (!string.IsNullOrEmpty(stackTrace))
                    reason.Append($"\r\n{stackTrace}");
            }

            _view.SkipReason = reason.ToString();

            DisplayTestProperties(testNode);

            _view.ShowTestPanel();
        }

        public void HideTestPanel()
        {
            _view.HideTestPanel();
        }

        private void DisplayTestProperties(TestNode testNode)
        {
            var sb = new StringBuilder();
            foreach (string item in testNode.GetAllProperties(_view.DisplayHiddenProperties))
            {
                if (sb.Length > 0)
                    sb.Append(Environment.NewLine);
                sb.Append(item);
            }
            _view.Properties = sb.ToString();
        }

        private void DisplayResultPanel(ResultNode resultNode)
        {
            _view.Outcome = resultNode.Outcome.ToString();

            _view.ElapsedTime = resultNode.Duration.ToString("f3");
            _view.AssertCount = resultNode.AssertCount.ToString();

            DisplayAssertionResults(resultNode);
            DisplayOutput(resultNode);

            _view.ShowResultPanel();
        }

        public void HideResultPanel()
        {
            _view.HideResultPanel();
        }

        private void DisplayAssertionResults(ResultNode resultNode)
        {
            StringBuilder sb;
            var assertionResults = resultNode.Assertions;

            // If there were no actual assertionresult entries, we fake
            // one if there is a message to display
            if (assertionResults.Count == 0)
            {
                if (resultNode.Outcome.Status == TestStatus.Failed)
                {
                    string status = resultNode.Outcome.Label ?? "Failed";
                    XmlNode failure = resultNode.Xml.SelectSingleNode("failure");
                    if (failure != null)
                        assertionResults.Add(new AssertionResult(failure, status));
                }
                else
                {
                    string status = resultNode.Outcome.Label ?? "Skipped";
                    XmlNode reason = resultNode.Xml.SelectSingleNode("reason");
                    if (reason != null)
                        assertionResults.Add(new AssertionResult(reason, status));
                }
            }

            sb = new StringBuilder();
            int index = 0;
            foreach (var assertion in assertionResults)
            {
                sb.AppendLine($"{++index}) {assertion.Status.ToUpper()} {assertion.Message}");
                if (assertion.StackTrace != null)
                    sb.AppendLine(AdjustStackTrace(assertion.StackTrace));

            }

            _view.Assertions = sb.ToString();
        }

        // Some versions of the framework return the stacktrace
        // without leading spaces, so we add them if needed.
        // TODO: Make sure this is valid across various cultures.
        private const string LEADING_SPACES = "   ";

        private static string AdjustStackTrace(string stackTrace)
        {
            // Check if no adjustment needed. We assume that all
            // lines start the same - either with or without spaces.
            if (stackTrace.StartsWith(LEADING_SPACES))
                return stackTrace;

            var sr = new StringReader(stackTrace);
            var sb = new StringBuilder();
            string line = sr.ReadLine();
            while (line != null)
            {
                sb.Append(LEADING_SPACES);
                sb.AppendLine(line);
                line = sr.ReadLine();
            }

            return sb.ToString();
        }

        private void DisplayOutput(ResultNode resultNode)
        {
            var output = resultNode.Xml.SelectSingleNode("output");
            _view.Output = output != null ? output.InnerText : "";
        }

        public static string GetTestType(TestNode testNode)
        {
            if (testNode.RunState == RunState.NotRunnable
                && testNode.Type == "Assembly"
                && !string.IsNullOrEmpty(testNode.FullName))
            {
                var fi = new FileInfo(testNode.FullName);
                string extension = fi.Extension.ToLower();
                if (extension != ".exe" && extension != ".dll")
                    return "Unknown";
            }
            return testNode.Type;
        }

        #region Helper Methods

        // Sometimes, the message may have leading blank lines and/or
        // may be longer than Windows really wants to display.
        private string TrimMessage(string message)
        {
            if (message != null)
            {
                if (message.Length > 64000)
                    message = message.Substring(0, 64000);

                int start = 0;
                for (int i = 0; i < message.Length; i++)
                {
                    switch (message[i])
                    {
                        case ' ':
                        case '\t':
                            break;
                        case '\r':
                        case '\n':
                            start = i + 1;
                            break;

                        default:
                            return start == 0 ? message : message.Substring(start);
                    }
                }
            }

            return message;
        }

        #endregion
    }
}
