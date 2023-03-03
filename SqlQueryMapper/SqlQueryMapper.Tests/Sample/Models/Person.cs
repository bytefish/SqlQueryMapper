// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace SqlQueryMapper.Tests.Sample.Models
{
    /// <summary>
    /// Maps to [Application].[Person].
    /// </summary>
    public class Person
    {
        /// <summary>
        /// [PersonID].
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// [FullName].
        /// </summary>
        public string FullName { get; set; } = null!;

        /// <summary>
        /// [PreferredName].
        /// </summary>
        public string PreferredName { get; set; } = null!;

        /// <summary>
        /// [UserId].
        /// </summary>
        public int? UserId { get; set; }

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
