using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Contractor.Core;
using Microsoft.Cci;

namespace Contractor.Gui
{
    interface ITypesViewer
    {
        TreeNode GetTypesTreeRoot(IAssemblyXXX assembly);
    }

    class TypesViewer : ITypesViewer
    {
        public TreeNode GetTypesTreeRoot(IAssemblyXXX assembly)
        {
            var namespaces = new Dictionary<string, TreeNode>();
            var types = assembly.Types();
            var assemblyNode = new TreeNode
            {
                Text = "_AssemblyInfo.Module.Name.Value",
                ImageKey = "assembly",
                SelectedImageKey = "assembly"
            };

            foreach (var namespaceDefinition in assembly.Namespaces())
            {
                TreeNode namespaceNode = CreateChildNode(assemblyNode, namespaceDefinition.Name(), "namespace");
                foreach (var typeDefinition in namespaceDefinition.Types())
                {
                    var typeNode = CreateChildNode(namespaceNode, typeDefinition.Name, "class");
                    typeNode.Tag = typeDefinition;
                }
            }

            return assemblyNode;
        }

        protected TreeNode CreateChildNode(TreeNode rootNode, string name, string image)
        {
            var node = rootNode.Nodes.Add(name);
            node.ImageKey = image;
            node.SelectedImageKey = image;
            return node;
        }
    }
}
