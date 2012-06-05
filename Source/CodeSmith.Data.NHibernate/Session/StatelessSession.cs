using NHibernate;

namespace CodeSmith.Data.NHibernate
{
    public class StatelessSession : StateSession<IStatelessSession>
    {
        public StatelessSession(IStatelessSession session)
        {
            Session = session;
        }
        
        public override void Save(object o)
        {
            Session.Insert(o);
        }

        public override void Refresh(object o)
        {
            Session.Refresh(o);
        }

        public override void Delete(object o)
        {
            Session.Delete(o);
        }

        public override void Close()
        {
            Session.Close();
        }

        public override IQuery GetNamedQuery(string queryName)
        {
            return Session.GetNamedQuery(queryName);
        }

        public override ITransaction BeginTransaction()
        {
            if (Transaction == null)
                Transaction = Session.BeginTransaction();

            return Transaction;
        }

        public override bool IsOpen
        {
            get { return Session.IsOpen; }
        }
    }
}