// ***********************************************************************
// Copyright (c) Charlie Poole and TestCentric contributors.
// Licensed under the MIT License. See LICENSE file in root directory.
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using TestCentric.Gui.Model;

namespace TestCentric.Gui.Presenters.Main
{
    public class ProjectEventTests : MainPresenterTestBase
    {
        [Test]
        public void WhenProjectIsCreated_TitleBarIsSet()
        {
            var project = new TestCentricProject(_model, "dummy.dll");
            _model.TestProject.Returns(project);

            FireProjectLoadedEvent();

            _view.Received().Title = "TestCentric - UNNAMED.tcproj";
        }

        [Test]
        public void WhenProjectIsClosed_TitleBarIsSet()
        {
            FireProjectUnloadedEvent();

            _view.Received().Title = "TestCentric Runner for NUnit"; 
        }
    }
}
