
using System;
using System.Data.Linq;
using System.Globalization;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Web;

namespace Tracker.Core.Data
{
    public partial class TrackerDataContext
    {
        public System.Data.Linq.IMultipleResults Test()
        {
            var flags = BindingFlags.Instance | BindingFlags.NonPublic;
            
            var type = typeof(TrackerDataContext);

            DynamicMethod testMethod = new DynamicMethod("GetUsersWithRoles", typeof(IMultipleResults), new Type[] {}, type);

            var methodInfo = type.GetMethod("GetUsersWithRoles");
            var expression = Expression.Call(Expression.Constant(this), testMethod);

            PropertyInfo providerProperty = type.GetProperty("Provider", flags);
            if (providerProperty == null)
                return null;

            object provider = providerProperty.GetValue(this, null);

            if (provider == null)
                return null;

            //IExecuteResult Execute(Expression query);
            Type providerType = provider.GetType().GetInterface("IProvider");
            foreach (var info in providerType.GetMethods())
            {
                Console.WriteLine(info.Name);
            }

            MethodInfo executeMethod = providerType.GetMethod("Execute", BindingFlags.Instance | BindingFlags.Public);

            var result = executeMethod.Invoke(provider, new object[] { expression });
            return null;
        }

        #region Extensibility Method Definitions
        //TODO: Uncomment and implement partial method
        //partial void OnCreated()
        //{
        //    
        //}
        #endregion
    }
}