using UnityEngine;

namespace Core.Cheats
{
    /// <summary>
    /// Interface for all cheat controls created by <see cref="Cheats"/> module. Used for elements layout.
    /// </summary>
    public interface ICheatsElement
    {
        /// <summary>
        /// Control's <see cref="RectTransform"/>.
        /// </summary>
        RectTransform RectTransform { get; }

        /// <summary>
        /// Set control width. Used in auto expanding/shrinking controls during layout.
        /// </summary>
        /// <param name="width">New control width</param>
        void SetWidth(float width);

        /// <summary>
        /// Calculate most appropriate width for cheat control.
        /// </summary>
        /// <returns>Calculated control width</returns>
        float GetWidth();
    }
}