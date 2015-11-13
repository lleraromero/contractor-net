using System.Collections.Generic;
using System.Windows.Forms;
using Contractor.Core;

namespace Contractor.Gui
{
    internal interface ITypesViewer
    {
        TreeNode GetTypesTreeRoot(IAssemblyXXX assembly);
    }

    internal class TypesViewer : ITypesViewer
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
                var namespaceNode = CreateChildNode(assemblyNode, namespaceDefinition.Name(), "namespace");
                foreach (var typeDefinition in namespaceDefinition.Types())
                {
                    var typeNode = CreateChildNode(namespaceNode, typeDefinition.ToString(), "class");
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