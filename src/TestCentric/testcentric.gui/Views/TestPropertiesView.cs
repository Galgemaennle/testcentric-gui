// ***********************************************************************
// Copyright (c) Charlie Poole and TestCentric contributors.
// Licensed under the MIT License. See LICENSE file in root directory.
// ***********************************************************************

using System.Drawing;
using System.Windows.Forms;

namespace TestCentric.Gui.Views
{
    using Elements;

    public partial class TestPropertiesView : UserControl, ITestPropertiesView
    {
        public TestPropertiesView()
        {
            InitializeComponent();
        }

        public string Header
        {
            get { return header.Text; }
            set { InvokeIfRequired(() => { header.Text = value; }); }
        }

        public void ShowPackagePanel()
        {
            InvokeIfRequired(() =>
            {
                packageSettingsDisplay.Visible = true;
                splitContainer1.Panel1Collapsed = false;
            });
        }
        public void HidePackagePanel()
        {
            InvokeIfRequired(() =>
            {
                packageSettingsDisplay.Visible = false;
                splitContainer1.Panel1Collapsed = true;
            });
        }

        public void ShowTestPanel()
        {
            InvokeIfRequired(() =>
            {
                testPropertiesDisplay.Visible = true;
            });
        }
        public void HideTestPanel()
        {
            InvokeIfRequired(() =>
            {
                testPropertiesDisplay.Visible = false;
            });
        }

        public void ShowResultPanel()
        {
            InvokeIfRequired(() =>
            {
                resultPanel.Visible = true;
                splitContainer2.Panel2Collapsed = false;
                ClientSize = new Size(ClientSize.Width, splitContainer1.Bottom + 2);
            });
        }
        public void HideResultPanel()
        {
            InvokeIfRequired(() =>
            {
                resultPanel.Visible = false;
                splitContainer2.Panel2Collapsed = true;
                ClientSize = new Size(ClientSize.Width, splitContainer1.Bottom + 2);
            });
        }

        public string TestType
        {
            get { return testPropertiesDisplay.TestType; }
            set { testPropertiesDisplay.TestType = value; }
        }

        public string FullName
        {
            get { return testPropertiesDisplay.FullName; }
            set { testPropertiesDisplay.FullName = value; }
        }

        public string Description
        {
            get { return testPropertiesDisplay.Description; }
            set { testPropertiesDisplay.Description = value; }
        }

        public string Categories
        {
            get { return testPropertiesDisplay.Categories; }
            set { testPropertiesDisplay.Categories = value; }
        }

        public string TestCount
        {
            get { return testPropertiesDisplay.TestCount; }
            set { testPropertiesDisplay.TestCount = value; }
        }

        public string RunState
        {
            get { return testPropertiesDisplay.RunState; }
            set { testPropertiesDisplay.RunState = value; }
        }

        public string SkipReason
        {
            get { return testPropertiesDisplay.SkipReason; }
            set { testPropertiesDisplay.SkipReason = value; }
        }

        public bool DisplayHiddenProperties
        {
            get { return testPropertiesDisplay.DisplayHiddenProperties; }
        }

        public string Properties
        {
            get { return testPropertiesDisplay.Properties; }
            set { testPropertiesDisplay.Properties = value; }
        }

        public string Outcome
        {
            get { return outcome.Text; }
            set { InvokeIfRequired(() => { outcome.Text = value; }); }
        }

        public string ElapsedTime
        {
            get { return elapsedTime.Text; }
            set { InvokeIfRequired(() => { elapsedTime.Text = value; }); }
        }

        public string AssertCount
        {
            get { return assertCount.Text; }
            set { InvokeIfRequired(() => { assertCount.Text = value; }); }
        }

        public string Assertions
        {
            get { return assertions.Text; }
            set { InvokeIfRequired(() => { assertions.Text = value; }); }
        }

        public string Output
        {
            get { return output.Text; }
            set { InvokeIfRequired(() => { output.Text = value; }); }
        }
        public string PackageSettings
        {
            get { return packageSettingsDisplay.Text; }
            set { InvokeIfRequired(() => { packageSettingsDisplay.Text = value; }); }
        }

        #region Helper Methods

        private void InvokeIfRequired(MethodInvoker _delegate)
        {
            if (this.InvokeRequired)
                this.BeginInvoke(_delegate);
            else
                _delegate();
        }

        #endregion
    }
}
