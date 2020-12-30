﻿using System;
using System.Collections.Generic;
using Abp.Application.Services.Dto;
using Realty.Pages.Dto;

namespace Realty.Forms.Dto
{
    public class FormEditDto: AuditedEntityDto<Guid>
    {
        public string Name { get; set; }

        public FormStatus Status { get; set; }

        public IList<PageEditDto> Pages { get; set; }
    }
}
