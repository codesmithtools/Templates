using CodeSmith.Data.Rules;

namespace CodeSmith.Data
{
    /// <summary>
    /// A class representing a tracked object.
    /// </summary>
    /// <typeparam name="T">The type of the tracked object.</typeparam>
    public class TrackedObject<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TrackedObject&lt;T&gt;"/> class.
        /// </summary>
        public TrackedObject()
        {
        }

        /// <summary>
        /// Gets the current tracked object.
        /// </summary>
        /// <value>The current.</value>
        public T Current { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is changed.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is changed; otherwise, <c>false</c>.
        /// </value>
        public bool IsChanged { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is deleted.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is deleted; otherwise, <c>false</c>.
        /// </value>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is new.
        /// </summary>
        /// <value><c>true</c> if this instance is new; otherwise, <c>false</c>.</value>
        public bool IsNew { get; set; }

        /// <summary>
        /// Gets the original tracked object.
        /// </summary>
        /// <value>The original.</value>
        public T Original { get; set; }
    }

    /// <summary>
    /// A class representing a tracked object.
    /// </summary>
    public class TrackedObject : TrackedObject<object>
    {}
}