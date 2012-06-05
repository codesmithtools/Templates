using System;
using System.Collections.Generic;
using System.Text;
using NHibernate;
using PetShop.Core.Model;
using Petshop.Data;

namespace Petshop.Data.Entities
{
	public partial class Order
    {
        public virtual CreditCard CreditCard { get; set; }
	}
}