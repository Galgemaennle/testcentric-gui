// ***********************************************************************
// Copyright (c) Charlie Poole and TestCentric GUI contributors.
// Licensed under the MIT License. See LICENSE file in root directory.
// ***********************************************************************

using System.Windows.Forms;
using NUnit.Framework;
using NSubstitute;

namespace TestCentric.Gui.Presenters.TestTree
{
    using Model;
    using Elements;

    public class CommandTests : TestTreePresenterTestBase
    {
        [Test]
        public void ToolStrip_RunButton_RunsAllTests()
        {
            _model.HasTests.Returns(true);
            _model.IsTestRunning.Returns(false);
            _view.RunButton.Execute += Raise.Event<CommandHandler>();
            _model.Received().RunAllTests();
        }

        [Test]
        public void ToolStrip_RunAllCommand_RunsAllTests()
        {
            _view.RunAllCommand.Execute += Raise.Event<CommandHandler>();
            _model.Received().RunAllTests();
        }

        [Test]
        public void ToolStrip_RunSelectedCommand_RunsSelectedTest()
        {
            var testNode = new TestNode("<test-case id='123'/>");
            var treeNode = new TreeNode("test");
            treeNode.Tag = testNode;

            _view.Tree.SelectedNodeChanged += Raise.Event<TreeNodeActionHandler>(treeNode);
            _view.RunSelectedCommand.Execute += Raise.Event<CommandHandler>();
            _model.Received().RunTests(testNode);
        }

        [Test]
        public void ToolStrip_RunFailedCommand_RunsAllTests()
        {
            _view.RunFailedCommand.Execute += Raise.Event<CommandHandler>();
            _model.Received().RunAllTests();
        }

        [Test]
        public void ToolStrip_DebugButton_DebugsAllTests()
        {
            _model.HasTests.Returns(true);
            _model.IsTestRunning.Returns(false);
            _view.DebugButton.Execute += Raise.Event<CommandHandler>();
            _model.Received().DebugAllTests();
        }

        [Test]
        public void ToolStrip_DebugAllCommand_DebugsAllTests()
        {
            _view.DebugAllCommand.Execute += Raise.Event<CommandHandler>();
            _model.Received().DebugAllTests();
        }

        [Test]
        public void ToolStrip_DebugSelectedCommand_DebugsSelectedTest()
        {
            var testNode = new TestNode("<test-case id='123'/>");
            var treeNode = new TreeNode("test");
            treeNode.Tag = testNode;

            _view.Tree.SelectedNodeChanged += Raise.Event<TreeNodeActionHandler>(treeNode);
            _view.DebugSelectedCommand.Execute += Raise.Event<CommandHandler>();
            _model.Received().DebugTests(testNode);
        }

        [Test]
        public void ToolStrip_DebugFailedCommand_DebugsAllTests()
        {
            _view.DebugFailedCommand.Execute += Raise.Event<CommandHandler>();
            _model.Received().DebugAllTests();
        }

        [Test]
        public void ContextMenu_RunCommand_RunsSpecifiedTest()
        {
            var xmlNode = XmlHelper.CreateXmlNode("<test-case id='5'/>");
            var treeNode = new TreeNode("MyTest");
            treeNode.Tag = new TestNode(xmlNode);

            _view.Tree.SelectedNodeChanged += Raise.Event<TreeNodeActionHandler>(treeNode);
            _view.RunContextCommand.Execute += Raise.Event<CommandHandler>();

            _model.Received().RunTests(Arg.Compat.Is<TestNode>((t) => t.Id == "5"));
        }

        [Test]
        public void ContextMenu_DebugCommand_RunsSpecifiedTest()
        {
            var xmlNode = XmlHelper.CreateXmlNode("<test-case id='5'/>");
            var treeNode = new TreeNode("MyTest");
            treeNode.Tag = new TestNode(xmlNode);

            _view.Tree.SelectedNodeChanged += Raise.Event<TreeNodeActionHandler>(treeNode);
            _view.DebugContextCommand.Execute += Raise.Event<CommandHandler>();

            _model.Received().DebugTests(Arg.Compat.Is<TestNode>((t) => t.Id == "5"));
        }

        [Test]
        public void ContextMenu_ExpandAll_ExpandsTree()
        {
            _view.ExpandAllCommand.Execute += Raise.Event<CommandHandler>();

            _view.Received().ExpandAll();
        }

        [Test]
        public void ContextMenu_CollapseAll_CollapsesTree()
        {
            _view.CollapseAllCommand.Execute += Raise.Event<CommandHandler>();

            _view.Received().CollapseAll();
        }

        [Test]
        public void ContextMenu_CollapseToFixtures()
        {
            // TreeNodeCollection has no constructor!
            //View.Tree.Nodes.Returns(new TreeView().Nodes);
            _view.CollapseToFixturesCommand.Execute += Raise.Event<CommandHandler>();

            // TODO: Develop a real test using mock-fixture
        }
    }
}
