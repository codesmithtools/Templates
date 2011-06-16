using System;
using System.Collections.Generic;
using System.Text;
using NHibernate;
using Petshop.Data;

namespace Petshop.Data.Entities
{
	public partial class HelloWorldResult
    {
        public HelloWorldResult(System.String productId, System.String categoryId, System.String name, System.String descn, System.String image)
        {
            ProductId = productId;
            CategoryId = categoryId;
            Name = name;
            Descn = descn;
            Image = image;
        }
        
        public System.String ProductId { get; set; }
        
        public System.String CategoryId { get; set; }
        
        public System.String Name { get; set; }
        
        public System.String Descn { get; set; }
        
        public System.String Image { get; set; }
	}
}