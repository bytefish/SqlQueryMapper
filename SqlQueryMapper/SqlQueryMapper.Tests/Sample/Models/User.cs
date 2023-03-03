// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace SqlQueryMapper.Tests.Sample.Models
{
    /// <summary>
    /// Maps to [Application].[User].
    /// </summary>
    public class User
    {
        /// <summary>
        /// [UserID].
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// [FullName]
        /// </summary>
        public string FullName { get; set; } = null!;

        /// <summary>
        /// [PreferredName]
        /// </summary>
        public string PreferredName { get; set; } = null!;

        /// <summary>
        /// [IsPermittedToLogon]
        /// </summary>
        public bool IsPermittedToLogon { get; set; }

        /// <summary>
        /// [LogonName].
        /// </summary>
        public string? LogonName { get; set; } = null!;

        /// <summary>
        /// [HashedPassword].
        /// </summary>
        public string? HashedPassword { get; set; } = null!;

        /// <summary>
        /// [RowVersion].
        /// </summary>
        public byte[] RowVersion { get; set; } = null!;

        /// <summary>
        /// [LastEditedBy].
        /// </summary>
        public int LastEditedBy { get; set; }

        /// <summary>
        /// [ValidFrom].
        /// </summary>
        public DateTime ValidFrom { get; set; }

        /// <summary>
        /// [ValidTo].
        /// </summary>
        public DateTime ValidTo { get; set; }
    }
}
