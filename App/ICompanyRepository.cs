using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace App
{
    interface ICompanyRepository
    {
        Company GetById(int Id);
    }
}
