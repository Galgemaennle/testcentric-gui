// ***********************************************************************
// Copyright (c) Charlie Poole and TestCentric GUI contributors.
// Licensed under the MIT License. See LICENSE file in root directory.
// ***********************************************************************

using System;
using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace TestCentric.Gui
{
    using Views;
    using Elements;

    /// <summary>
    /// The VisualState class holds the latest visual state for a project.
    /// </summary>
    [Serializable]
    public class VisualState
    {
        #region Fields
        [XmlAttribute]
        public bool ShowCheckBoxes;

        public string TopNode;

        public string SelectedNode;

        public List<string> SelectedCategories;

        public bool ExcludeCategories;

        [XmlArrayItem("Node")]
        public List<VisualTreeNode> Nodes = new List<VisualTreeNode>();
        #endregion

        #region Public Methods

        public static string GetVisualStateFileName(string testFileName)
        {
            return string.IsNullOrEmpty(testFileName)
                ? "VisualState.xml"
                : testFileName + ".VisualState.xml";
        }

        public static VisualState LoadFrom(string fileName)
        {
            using (StreamReader reader = new StreamReader(fileName))
            {
                return LoadFrom(reader);
            }
        }

        public static VisualState LoadFrom(TextReader reader)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(VisualState));
            return (VisualState)serializer.Deserialize(reader);
        }

        public static VisualState LoadFrom(ITestTreeView view)
        {
            var visualState = new VisualState()
            {
                ShowCheckBoxes = view.CheckBoxes,
                TopNode = (string)view.Tree.TopNode?.Tag,
                SelectedNode = (string)view.Tree.SelectedNode?.Tag,
            };

            visualState.ProcessTreeNodes(view.Tree);

            return visualState;
        }

        public void Save(string fileName)
        {
            using (StreamWriter writer = new StreamWriter(fileName))
            {
                Save(writer);
            }
        }

        public void Save(TextWriter writer)
        {
            XmlSerializer serializer = new XmlSerializer(GetType());
            serializer.Serialize(writer, this);
        }

        public void RestoreVisualState(ITestTreeView view, IDictionary<string, TreeNode> treeMap)
        {
            view.CheckBoxes = ShowCheckBoxes;

            foreach (VisualTreeNode visualNode in Nodes)
            {
                if (treeMap.ContainsKey(visualNode.Id))
                {
                    TreeNode treeNode = treeMap[visualNode.Id];

                    if (treeNode.IsExpanded != visualNode.Expanded)
                        treeNode.Toggle();

                    treeNode.Checked = visualNode.Checked;
                }
            }

            if (SelectedNode != null && treeMap.ContainsKey(SelectedNode))
                view.Tree.SelectedNode = treeMap[SelectedNode];

            if (TopNode != null && treeMap.ContainsKey(TopNode))
                view.Tree.TopNode = treeMap[TopNode];

            //view.Tree.Select();
        }

        #endregion

        #region Helper Methods

        private void ProcessTreeNodes(ITreeView tree)
        {
            if (tree.Nodes != null)
                foreach (TreeNode node in tree.Nodes)
                    ProcessTreeNodes(node);
        }

        private void ProcessTreeNodes(TreeNode node)
        {
            if (IsInteresting(node))
                this.Nodes.Add(new VisualTreeNode(node));

            foreach (TreeNode childNode in node.Nodes)
                ProcessTreeNodes(childNode);
        }

        private bool IsInteresting(TreeNode node)
        {
            return node.IsExpanded || node.Checked;
        }

        #endregion
    }

    [Serializable]
    public class VisualTreeNode
    {
        [XmlAttribute]
        public string Id;

        [XmlAttribute, System.ComponentModel.DefaultValue(false)]
        public bool Expanded;

        [XmlAttribute, System.ComponentModel.DefaultValue(false)]
        public bool Checked;

        [XmlArrayItem("Node")]
        public VisualTreeNode[] Nodes;

        public VisualTreeNode() { }

        public VisualTreeNode(TreeNode treeNode)
        {
            Id = (string)treeNode.Tag;
            Expanded = treeNode.IsExpanded;
            Checked = treeNode.Checked;
        }
    }
}
