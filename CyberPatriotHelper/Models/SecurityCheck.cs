using System;

namespace CyberPatriotHelper.Models
{
    /// <summary>
    /// Represents the status of a security check
    /// </summary>
    public enum CheckStatus
    {
        Pass,
        Fail,
        Warning,
        Unknown
    }

    /// <summary>
    /// Base class for all security checks - the repeatable pattern
    /// </summary>
    public abstract class SecurityCheck
    {
        public string Name { get; protected set; } = string.Empty;
        public string Description { get; protected set; } = string.Empty;
        public string WhyItMatters { get; protected set; } = string.Empty;
        public CheckStatus Status { get; protected set; } = CheckStatus.Unknown;
        public string Evidence { get; protected set; } = string.Empty;
        public string ManualSteps { get; protected set; } = string.Empty;

        /// <summary>
        /// Checks the current status of this security item
        /// </summary>
        public abstract void Check();

        /// <summary>
        /// Attempts to automatically fix the issue
        /// </summary>
        /// <returns>True if fix was successful</returns>
        public abstract bool Fix();

        /// <summary>
        /// Verifies that the fix was applied correctly
        /// </summary>
        /// <returns>True if verified</returns>
        public virtual bool Verify()
        {
            Check();
            return Status == CheckStatus.Pass;
        }
    }
}
