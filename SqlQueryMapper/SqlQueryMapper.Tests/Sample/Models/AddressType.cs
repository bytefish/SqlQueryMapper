// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace SqlQueryMapper.Tests.Sample.Models
{
    /// <summary>
    /// Maps to [Application].[AddressType].
    /// </summary>
    public class AddressType
    {
        /// <summary>
        /// [AddressTypeID].
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// [Name].
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// [Description].
        /// </summary>
        public string Description { get; set; } = null!;

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
