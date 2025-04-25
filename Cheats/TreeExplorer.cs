using FMOD.Studio;
using FMODUnity;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static UnbeatableSongHack.Core;

namespace UnbeatableSongHack.Cheats
{
   public class TreeExplorer
    {

        // This handles drawing the explorer that appears when clicking "Get All Events."
        // It creates a tree structure of all events and strings in the game.
        // It also handles the selection of events and strings, and plays them when selected.
        // Might update the UI in the future.


        public class TreeNode
        {
            public string Name { get; set; }
            public string FullPath { get; set; }
            public bool IsLeaf { get; set; }
            public bool IsExpanded { get; set; }
            public Dictionary<string, TreeNode> Children { get; set; }
            public bool IsString { get; set; }
            public bool IsPlayable { get; set; }

            public TreeNode(string name, string fullPath = "", bool isLeaf = false, bool isString = false, bool isPlayable = false)
            {
                Name = name;
                FullPath = fullPath;
                IsLeaf = isLeaf;
                IsExpanded = false;
                Children = new Dictionary<string, TreeNode>();
                IsString = isString;
                IsPlayable = isPlayable;
            }
        }


        public static void AddPathToTree(string path, bool isString, bool isPlayable, TreeNode root)
        {
            string[] parts;
            string prefix = "";

            if (path.StartsWith("event:/"))
            {
                prefix = "event:/";
                parts = path.Substring(7).Split('/');
            }
            else if (path.StartsWith("snapshot:/"))
            {
                prefix = "snapshot:/";
                parts = path.Substring(10).Split('/');
            }
            else
            {
                parts = path.Split('/');
                if (parts.Length > 0 && parts[0].Contains(":"))
                {
                    prefix = parts[0] + "/";
                    parts = parts.Skip(1).ToArray();
                }
            }

            string currentPath = prefix;
            TreeNode currentNode = root;

            if (!string.IsNullOrEmpty(prefix))
            {
                if (!currentNode.Children.ContainsKey(prefix))
                {
                    currentNode.Children[prefix] = new TreeNode(prefix);
                }
                currentNode = currentNode.Children[prefix];
            }

            for (int i = 0; i < parts.Length; i++)
            {
                if (string.IsNullOrEmpty(parts[i])) continue;

                currentPath += (i > 0 ? "/" : "") + parts[i];

                bool isLeaf = i == parts.Length - 1;

                if (!currentNode.Children.ContainsKey(parts[i]))
                {
                    currentNode.Children[parts[i]] = new TreeNode(parts[i], currentPath, isLeaf, isString && isLeaf, isPlayable && isLeaf);
                }
                else if (isLeaf)
                {
                    currentNode.Children[parts[i]].IsLeaf = true;
                    currentNode.Children[parts[i]].FullPath = currentPath;
                    if (isString)
                    {
                        currentNode.Children[parts[i]].IsString = true;
                        if (isPlayable)
                        {
                            currentNode.Children[parts[i]].IsPlayable = true;
                        }
                    }
                }

                currentNode = currentNode.Children[parts[i]];
            }
        }

        public static void BuildEventTree(out TreeNode rootNode, List<EventDescription> eventDescriptions, List<string> strings)
        {
            rootNode = new TreeNode("Root");

            foreach (var eventDesc in eventDescriptions)
            {
                eventDesc.getPath(out string eventPath);

                AddPathToTree(eventPath, false, false, rootNode);
            }

            foreach (var stringPath in strings)
            {
                bool isPlayable = stringPath.StartsWith("event:/");

                AddPathToTree(stringPath, true, isPlayable, rootNode);
            }
        }

        public static int CalculateTreeHeight(TreeNode node, int depth = 0)
        {
            int height = 0;

            if (depth > 0 || node.Name != "Root")
            {
                height += 30;
            }

            if (node.IsExpanded || node.Name == "Root")
            {
                foreach (var child in node.Children.Values)
                {
                    height += CalculateTreeHeight(child, depth + 1);
                }
            }

            return height;
        }

        public static void DrawTreeNode(TreeNode node, int depth, ref int yPos, GUIStyle folderStyle, GUIStyle leafStyle,
                                  GUIStyle stringStyle, GUIStyle playableStringStyle)
        {
            var LoggerInstance = GetLogger();

            int indentation = depth * 20;
            int buttonWidth = 360 - indentation;

            if (node.Name != "Root")
            {
                string displayName = node.Name;
                string prefix = node.IsLeaf ? "  " : node.IsExpanded ? "▼ " : "► ";

                GUIStyle styleToUse;
                if (!node.IsLeaf)
                {
                    styleToUse = folderStyle;
                }
                else if (node.IsString)
                {
                    if (node.IsPlayable)
                    {
                        styleToUse = playableStringStyle;
                    }
                    else
                    {
                        styleToUse = stringStyle;
                    }
                }
                else
                {
                    styleToUse = leafStyle;
                }

                if (GUI.Button(new Rect(10 + indentation, yPos, buttonWidth, 20), prefix + displayName, styleToUse))
                {
                    if (node.IsLeaf)
                    {
                        if (node.IsString)
                        {
                            if (node.IsPlayable)
                            {
                                RuntimeManager.PlayOneShot(node.FullPath);
                                SongHackUI.lastEventPath = node.FullPath + " (String-Event)";
                                LoggerInstance.Msg("Playing string-event: " + node.FullPath);
                            }
                            else
                            {
                                SongHackUI.lastEventPath = $"String: {node.FullPath}";
                                LoggerInstance.Msg($"Selected string: {node.FullPath}");
                            }
                        }
                        else
                        {
                            RuntimeManager.PlayOneShot(node.FullPath);
                            SongHackUI.lastEventPath = node.FullPath;
                            LoggerInstance.Msg("Playing: " + node.FullPath);
                        }
                    }
                    else
                    {
                        node.IsExpanded = !node.IsExpanded;
                    }
                }

                yPos += 25;
            }

            if (node.IsExpanded || node.Name == "Root")
            {
                foreach (var child in node.Children.Values)
                {
                    DrawTreeNode(child, depth + 1, ref yPos, folderStyle, leafStyle, stringStyle, playableStringStyle);
                }
            }
        }
    }
}
