﻿using System;
using Abp.Application.Services.Dto;

namespace Realty.Transactions.Input
{
    public class GetTransactionFormForEditInput: EntityDto<Guid>
    {
        public EntityDto<Guid> Form { get; set; }
    }
}
