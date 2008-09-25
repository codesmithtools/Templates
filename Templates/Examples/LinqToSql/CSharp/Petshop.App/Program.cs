using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Petshop.Data;

namespace Petshop.App
{
    class Program
    {
        static void Main(string[] args)
        {
            GetTopFiveProducts();
            GetPenguin();
            
            Console.Out.WriteLine("Press enter to exit.");
            Console.In.ReadLine();
        }

        static void GetTopFiveProducts()
        {
            PetshopDataContext context = new PetshopDataContext();
            List<Product> products = context.Product.Take(5).ToList();

            Console.Out.WriteLine("Top 5 Products");
            for (int x = 0; x < 5; x++)
                Console.Out.WriteLine("{0}: {1}", x+1, products[x].Name);
            Console.Out.WriteLine();
        }

        static void GetPenguin()
        {
            string key = "BD-02";

            PetshopDataContext context = new PetshopDataContext();
            Product product = context.Manager.Product.GetByKey(key);

            if (product == null)
            {
                Console.Out.WriteLine("No entry found for key {0}.");
            }
            else
            {
                Console.Out.WriteLine("Found one entry for key {0}:", key);
                Console.Out.WriteLine("{0} - {1}", product.Name, product.Descn);
            }
            Console.Out.WriteLine();
        }
    }
}
