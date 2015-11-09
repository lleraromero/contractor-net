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

            foreach (var type in types)
            {
                TreeNode namespaceNode;
                var containingNamespace = "type.ContainingUnitNamespace";

                if (namespaces.ContainsKey(containingNamespace))
                {
                    namespaceNode = namespaces[containingNamespace];
                }
                else
                {
                    var namespaceName = containingNamespace.ToString();
                    namespaceNode = CreateChildNode(assemblyNode, namespaceName, "namespace");
                    namespaces.Add(containingNamespace, namespaceNode);
                }

                var typeName = type;
                var typeNode = CreateChildNode(namespaceNode, typeName.Name, "class");
                typeNode.Tag = type;

                //if (!type.IsPublic)
                //{
                //    typeNode.ForeColor = Color.Gray;
                //}
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
