// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace SqlQueryMapper.Tests.Sample.Models
{
    /// <summary>
    /// Available Address Types.
    /// </summary>
    public enum AddressTypeEnum
    {
        /// <summary>
        /// No Address Type
        /// </summary>
        None = 0,

        /// <summary>
        /// Home Address
        /// </summary>
        Home = 1,

        /// <summary>
        /// Work Address
        /// </summary>
        Work = 2,

        /// <summary>
        /// Billing Address
        /// </summary>
        Billing = 3,

        /// <summary>
        /// Delivery Address
        /// </summary>
        Delivery = 4
    }
}
