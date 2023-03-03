// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace SqlQueryMapper.Tests.Sample.Models
{
    /// <summary>
    /// Maps to [Application].[PersonAddress].
    /// </summary>
    public class PersonAddress
    {
        /// <summary>
        /// [PersonAddressID].
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// [PersonID].
        /// </summary>
        public int PersonId { get; set; }

        /// <summary>
        /// [AddressID].
        /// </summary>
        public int AddressId { get; set; }

        /// <summary>
        /// [AddressTypeID].
        /// </summary>
        public int AddressTypeId { get; set; }

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
