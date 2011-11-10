using System;
using System.Data.Entity;
using System.Data.Objects;

namespace EntityFramework.Audit
{
  public static class AuditLoggerExtensions
  {
    public static AuditLogger BeginAudit(this ObjectContext objectContext, AuditConfiguration configuration = null)
    {
      return new AuditLogger(objectContext, configuration);
    }

    public static AuditLogger BeginAudit(this DbContext dbContext, AuditConfiguration configuration = null)
    {
      return new AuditLogger(dbContext, configuration);
    }
  }
}