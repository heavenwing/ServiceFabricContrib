// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace ServiceFabricContrib
{
    using System;
    using System.Runtime.Serialization;
    using Microsoft.ServiceFabric.Services.Client;

    [DataContract]
    public class ItemId : IFormattable, IComparable, IComparable<ItemId>, IEquatable<ItemId>
    {
        [DataMember] private Guid id;

        public ItemId()
        {
            this.id = Guid.NewGuid();
        }

        public ItemId(Guid guid)
        {
            this.id = guid;
        }
        
        public int CompareTo(object obj)
        {
            return this.id.CompareTo(((ItemId)obj).id);
        }

        public int CompareTo(ItemId other)
        {
            return this.id.CompareTo(other.id);
        }

        public bool Equals(ItemId other)
        {
            return this.id.Equals(other.id);
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return this.id.ToString(format, formatProvider);
        }

        public ServicePartitionKey GetPartitionKey()
        {
            return new ServicePartitionKey(HashUtil.getLongHashCode(this.id.ToString()));
        }

        public static bool operator ==(ItemId item1, ItemId item2)
        {
            return item1.Equals(item2);
        }

        public static bool operator !=(ItemId item1, ItemId item2)
        {
            return !item1.Equals(item2);
        }

        public override bool Equals(object obj)
        {
            return (obj is ItemId) ? this.id.Equals(((ItemId)obj).id) : false;
        }

        public override int GetHashCode()
        {
            return this.id.GetHashCode();
        }

        public override string ToString()
        {
            return this.id.ToString();
        }

        public string ToString(string format)
        {
            return this.id.ToString(format);
        }
    }
}