﻿using System;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace Realty.Listings
{
    public class FiltersFeaturesSelected : FullAuditedEntity<Guid>, IMustHaveTenant
    {
        public int TenantId { get; set; }
        public string Feature { get; set; }
        public bool Selected { get; set; }
        public Guid UserFilterId { get; set; }
        [ForeignKey("UserFilterId ")]
        public virtual UsersFilters Filter { get; set; }
    }
}
