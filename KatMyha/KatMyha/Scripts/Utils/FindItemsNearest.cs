using Godot;
using Godot.Collections;
using KatMyha.Scripts.Items.KillLight;
using PrototipoMyha;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KatMyha.Scripts.Utils
{
    public interface IDistanceToSelf
    {
        /// <summary>
        /// Gets or sets the distance from the current object to itself.
        /// </summary>
        public float DistanceToSelf { get; set; }
    }
    public static class FindItemsNearest
    {
        /// <summary>
        /// Finds all nodes of type <typeparamref name="T"/> within a specified distance from a reference position and
        /// adds them to the provided list after setting their distance to the player.
        /// </summary>
        /// <remarks>The method sets the DistanceToPlayer property for each node added to the list. The
        /// returned list is the same instance as the input <paramref name="values"/> parameter. If no nodes match the
        /// criteria, the returned list will be empty.</remarks>
        /// <typeparam name="T">The node type to search for. Must inherit from Node2D and implement IDistanceToPlayer.</typeparam>
        /// <param name="array">The array of nodes to search for matching targets.</param>
        /// <param name="refPosition">The reference position used to calculate the distance to each node.</param>
        /// <param name="nearestDistance">The maximum distance, in units, within which nodes are considered valid targets.</param>
        /// <param name="values">The list to populate with matching nodes. The list is cleared before adding new items.</param>
        /// <returns>A list containing all nodes of type <typeparamref name="T"/> that are within the specified distance and
        /// satisfy the predicate, if provided.</returns>
        public static List<T> FindAndSetAimTargets<T>(
            Array<Node> array,
            Vector2 refPosition,
            float nearestDistance,
            List<T> values) where T : Node2D, IDistanceToSelf
        {
            values.Clear();
            foreach (var node in array)
            {
                if (node is not T item)
                    continue;

                float dist = item.GlobalPosition.DistanceTo(refPosition);
                if (dist < nearestDistance)
                {
                    item.DistanceToSelf = dist;
                    if(item is null) continue;
                    values.Add(item);
                }
            }
            return values;
        }
    }
}
