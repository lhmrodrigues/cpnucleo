﻿using Cpnucleo.Domain.Interfaces.Repositories;
using Cpnucleo.Infra.Data.Context;
using System;

namespace Cpnucleo.Infra.Data.UoW
{
    internal class UnitOfWork : IUnitOfWork
    {
        private readonly CpnucleoContext _context;

        public UnitOfWork(CpnucleoContext context)
        {
            _context = context;
        }

        public bool Commit()
        {
            return _context.SaveChanges() > 0;
        }

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
