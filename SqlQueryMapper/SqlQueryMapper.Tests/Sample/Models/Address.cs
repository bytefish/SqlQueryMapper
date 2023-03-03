// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace SqlQueryMapper.Tests.Sample.Models
{
    /// <summary>
    /// Maps to [Application].[Address].
    /// </summary>
    public class Address
    {
        /// <summary>
        /// [AddressID].
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// [AddressLine1].
        /// </summary>
        public string AddressLine1 { get; set; } = null!;

        /// <summary>
        /// [AddressLine2].
        /// </summary>
        public string? AddressLine2 { get; set; }

        /// <summary>
        /// [AddressLine3].
        /// </summary>
        public string? AddressLine3 { get; set; }

        /// <summary>
        /// [AddressLine4].
        /// </summary>
        public string? AddressLine4 { get; set; }

        /// <summary>
        /// [PostalCode].
        /// </summary>
        public string? PostalCode { get; set; }

        /// <summary>
        /// [City].
        /// </summary>
        public string City { get; set; } = null!;

        /// <summary>
        /// [Country].
        /// </summary>
        public string Country { get; set; } = null!;

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
