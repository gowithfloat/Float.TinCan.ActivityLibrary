#if NETSTANDARD
using Xamarin.Forms;
#endif

namespace Float.TinCan.ActivityLibrary.Definition
{
    /// <summary>
    /// Interface for all activity viewers.
    /// </summary>
    public interface IActivityViewer
    {
        /// <summary>
        /// Gets this viewer as a Forms page.
        /// </summary>
        /// <value>This viewer as a page.</value>
        Page Page { get; }

        /// <summary>
        /// Gets the lesson title.
        /// </summary>
        /// <value>The lesson title.</value>
        string LessonTitle { get; }

        /// <summary>
        /// Handle the user completing the current lesson.
        /// </summary>
        void OnComplete();
    }
}
